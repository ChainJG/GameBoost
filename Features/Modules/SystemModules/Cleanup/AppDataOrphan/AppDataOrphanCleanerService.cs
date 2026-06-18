using GameBoost.Shared.Helpers;
using GameBoost.Shared.Helpers.ProcessHelpers;
using GameBoost.Shared.Results;
using Microsoft.VisualBasic.FileIO;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Xml.Linq;

namespace GameBoost.Features.Modules.SystemModules.Cleanup.AppDataOrphan
{
    public sealed class AppDataOrphanCleanerService
    {

        private static readonly bool IsDebug = false;

        private static readonly HashSet<string> ProtectedFolderNames = new(StringComparer.OrdinalIgnoreCase)
        {
            "Microsoft",
            "Microsoft.Windows",
            "Microsoft_Corporation",
            "Windows",
            "Packages",
            "Package Cache",
            "PackageManagement",
            "Programs",
            "Apps",
            "Temp",
            "CrashDumps",
            "ConnectedDevicesPlatform",
            "Comms",
            "D3DSCache",
            "DigitalEntitlements",
            "INetCache",
            "INetHistory",
            "IsolatedStorage",
            "VirtualStore",
            "Runtime",
            "Publishers",

            "GameBoost",

            "AMD",
            "AMD_Common",
            "AMDIdentifyWindow",
            "AMDInstallManager",
            "AMDSoftwareInstaller",

            "NVIDIA",
            "NVIDIA Corporation",

            "Google",

            "Intel"
        };


        #region Delete Methods
        public Task<CleanupResult> DeleteAsync(IReadOnlyList<DirectoryScanCandidate> candidates, CancellationToken token) =>
            Task.Run(() => Delete(candidates, token), token);
        private static CleanupResult Delete(IReadOnlyList<DirectoryScanCandidate> candidates, CancellationToken token)
        {
            var result = new CleanupResult();

            foreach (var candidate in candidates)
            {
                token.ThrowIfCancellationRequested();

                try
                {
                    if (!candidate.Directory.Exists)
                    {
                        result.SkippedDirectories++;
                        continue;
                    }

                    FileSystem.DeleteDirectory(
                        candidate.Directory.FullName,
                        UIOption.OnlyErrorDialogs,
                        RecycleOption.SendToRecycleBin);

                    result.DeletedDirectories++;
                    result.DeletedBytes += candidate.SizeBytes;
                }
                catch
                {
                    result.FaildedDirectories++;
                }
            }

            return result;
        }
        #endregion

        public Task<DirectoryScanResult> ScanAsync(CancellationToken token) =>
            Task.Run(() => Scan(token), token);

        private static DirectoryScanResult Scan(CancellationToken token)
        {
            var roots = GetAppDataRoots();
            var installedPrograms = InstalledProgramSnapshot.Create();

            var candidates = new List<DirectoryScanCandidate>();

            var scannedFolders = 0;
            var skippedFolder = 0;

            var foldersToScan = roots
                .Where(root => root.Exists)
                .SelectMany(DirectoryHelper.GetDirectoriesSafe)
                .ToList();

            var parallelOptions = new ParallelOptions
            {
                CancellationToken = token,
                MaxDegreeOfParallelism = Math.Clamp(Environment.ProcessorCount / 2, 2, 6)
            };

            Parallel.ForEach(
                foldersToScan,
                parallelOptions,
                folder =>
                {
                    parallelOptions.CancellationToken.ThrowIfCancellationRequested();

                    Interlocked.Increment(ref scannedFolders);

                    var candidate = TryCreateCandidate(
                        folder,
                        installedPrograms,
                        parallelOptions.CancellationToken);

                    if (candidate is null)
                    {
                        Interlocked.Increment(ref skippedFolder);
                        return;
                    }

                    candidates.Add(candidate);
                });

            return new DirectoryScanResult
            {
                Candidates = [.. candidates
                .OrderByDescending(candidate => candidate.Confidence)
                .ThenByDescending(candidate => candidate.SizeBytes)],

                TotalBytes = candidates.Sum(candidate => candidate.SizeBytes),
                ScannedFolders = scannedFolders,
                SkippedFolders = skippedFolder
            };
        }


        private static IReadOnlyList<DirectoryInfo> GetAppDataRoots() =>
            [
                new(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)),
                new(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData))
            ];

        private static DirectoryScanCandidate? TryCreateCandidate(DirectoryInfo folder, InstalledProgramSnapshot installedPrograms, CancellationToken token)
        {
            DebugCandidateStart(folder);

            if (IsProtectedFolder(folder))
            {
                DebugCandidateDecision(folder, "SKIP", "Folder is protected or unsafe to scan/delete");
                return null;
            }

            var rule = FindMatchingRule(folder);

            if (rule is null)
            {
                DebugCandidateDecision(folder, "NO RULE", "No matching AppData orphan rule found; checking if folder is empty only");

                var emptyCandidate = TryCreateEmptyFolderCandidate(folder, token);

                if (emptyCandidate is null)
                {
                    DebugCandidateDecision(folder, "SKIP", "Folder is not a known orphan and is not an old empty folder");
                    return null;
                }

                DebugCandidateDecision(folder, "ALLOW", "Old empty AppData folder selected for cleanup");
                return emptyCandidate;
            }

            DebugCandidateDecision(folder, "RULE MATCH", $"Matched rule '{rule.DisplayName}'");

            if (IsInstalled(rule, installedPrograms, out var matchedProgramName, out var matchedSearchValue))
            {
                DebugCandidateDecision(folder, "SKIP", $"'{rule.DisplayName}' appears to still be installed. Matched '{matchedProgramName}' using search value '{matchedSearchValue}'");

                return null;
            }

            if (IsAnyRelatedProcessRunning(rule))
            {
                DebugCandidateDecision(folder, "SKIP", $"Related process for '{rule.DisplayName}' is currently running");
                return null;
            }

            var folderInfo = ScanFolderInfo(folder, token);

            if (!IsOldEnough(folderInfo.LastWriteTimeUtc, rule.MinimumAge))
            {
                DebugCandidateDecision(
                    folder,
                    "SKIP",
                    rule,
                    folderInfo,
                    DirectoryScanConfidence.None,
                    $"Folder is too recent. Minimum age is {rule.MinimumAge.TotalDays:0} days");

                return null;
            }

            if (rule.DeleteWhenEmptyOnly && !folderInfo.IsEmpty)
            {
                DebugCandidateDecision(
                    folder,
                    "SKIP",
                    rule,
                    folderInfo,
                    DirectoryScanConfidence.None,
                    "Rule only allows empty folders, but this folder still contains files or subfolders");

                return null;
            }

            var confidence = GetConfidence(rule, folderInfo);

            if (confidence != DirectoryScanConfidence.High)
            {
                DebugCandidateDecision(
                    folder,
                    "SKIP",
                    rule,
                    folderInfo,
                    confidence,
                    "Confidence is not high enough for automatic cleanup");

                return null;
            }

            var reason = BuildReason(rule, folderInfo);

            DebugCandidateDecision(
                folder,
                "ALLOW",
                rule,
                folderInfo,
                confidence,
                reason);

            return new DirectoryScanCandidate
            {
                Directory = folder,
                DisplayName = rule.DisplayName,
                Reason = reason,
                Confidence = confidence,
                SizeBytes = folderInfo.SizeBytes,
                FileCount = folderInfo.FileCount,
                DirectoryCount = folderInfo.DirectoryCount,
                LastWriteTimeUtc = folderInfo.LastWriteTimeUtc
            };
        }

        private static bool IsProtectedFolder(DirectoryInfo folder)
        {
            if (ProtectedFolderNames.Contains(folder.Name))
                return false;

            try
            {
                if ((folder.Attributes & FileAttributes.ReparsePoint) != 0)
                    return true;

                if ((folder.Attributes & FileAttributes.System) != 0)
                    return true;
            }
            catch
            {
                return true;
            }

            return false;
        }

        private static AppDataOrphanDefinition? FindMatchingRule(DirectoryInfo folder) =>
            AppDataOrphanCatalog.Rules
                .Where(rule => rule.FolderNames.Count > 0)
                .FirstOrDefault(rule =>
                    rule.FolderNames.Any(folderName =>
                        string.Equals(folder.Name, folderName, StringComparison.OrdinalIgnoreCase)));

        private static DirectoryScanCandidate? TryCreateEmptyFolderCandidate(DirectoryInfo folder, CancellationToken token)
        {
            var folderInfo = ScanFolderInfo(folder, token);

            if (!folderInfo.IsEmpty)
                return null;

            if (!IsOldEnough(folderInfo.LastWriteTimeUtc, TimeSpan.FromDays(30)))
                return null;

            return new DirectoryScanCandidate
            {
                Directory = folder,
                DisplayName = folder.Name,
                Reason = "Empty AppData folder with no recent activity.",
                Confidence = DirectoryScanConfidence.High,
                SizeBytes = 0,
                FileCount = 0,
                DirectoryCount = 0,
                LastWriteTimeUtc = folderInfo.LastWriteTimeUtc
            };
        }

        private static bool IsInstalled(AppDataOrphanDefinition rule, InstalledProgramSnapshot installedPrograms, out string matchedProgramName, out string matchedSearchValue)
        {
            matchedProgramName = string.Empty;
            matchedSearchValue = string.Empty;

            foreach (var installedProgramName in rule.InstalledProgramNames)
            {
                if (!installedPrograms.TryFindMatch(installedProgramName, out var match))
                    continue;

                matchedProgramName = match;
                matchedSearchValue = installedProgramName;
                return true;
            }

            return false;
        }

        private static bool IsAnyRelatedProcessRunning(AppDataOrphanDefinition rule)
        {
            foreach(var processName in rule.ProcessNames)
            {
                if (ProcessHelper.IsProcessRunning(processName))
                    return true;
            }

            return false;
        }

        private static bool IsOldEnough(DateTime lastWriteTimeUtc, TimeSpan minimumAge) =>
            lastWriteTimeUtc <= DateTime.UtcNow.Subtract(minimumAge);

        private static FolderInfo ScanFolderInfo(DirectoryInfo root, CancellationToken token)
        {
            long sizeBytes = 0;
            var fileCount = 0;
            var directoryCount = 0;
            var newestWriteTimeUtc = root.LastWriteTimeUtc;

            var pending = new Stack<DirectoryInfo>();
            pending.Push(root);

            while (pending.Count > 0)
            {
                token.ThrowIfCancellationRequested();

                var directory = pending.Pop();

                foreach (var file in DirectoryHelper.GetFilesSafe(directory))
                {
                    token.ThrowIfCancellationRequested();

                    try
                    {
                        sizeBytes += file.Length;
                        fileCount++;

                        if (file.LastWriteTimeUtc > newestWriteTimeUtc)
                            newestWriteTimeUtc = file.LastWriteTimeUtc;
                    }
                    catch
                    {
                        // Ignore inaccessible file size/date.
                    }
                }

                foreach (var childDirectory in DirectoryHelper.GetDirectoriesSafe(directory))
                {
                    token.ThrowIfCancellationRequested();

                    directoryCount++;

                    try
                    {
                        if (childDirectory.LastWriteTimeUtc > newestWriteTimeUtc)
                            newestWriteTimeUtc = childDirectory.LastWriteTimeUtc;
                    }
                    catch
                    {
                        // Ignore inaccessible directory date.
                    }

                    pending.Push(childDirectory);
                }
            }

            return new FolderInfo(
                SizeBytes: sizeBytes,
                FileCount: fileCount,
                DirectoryCount: directoryCount,
                LastWriteTimeUtc: newestWriteTimeUtc);
        }

        private static DirectoryScanConfidence GetConfidence(AppDataOrphanDefinition rule, FolderInfo folderInfo)
        {
            if (rule.HighRisk)
            {
                if (folderInfo.IsEmpty)
                    return DirectoryScanConfidence.High;

                if (folderInfo.SizeBytes <= MathHelper.MegabytesToBytes(100))
                    return DirectoryScanConfidence.Medium;

                return DirectoryScanConfidence.Low;
            }

            if (folderInfo.IsEmpty)
                return DirectoryScanConfidence.High;

            return DirectoryScanConfidence.High;
        }

        private static string BuildReason(AppDataOrphanDefinition rule, FolderInfo folderInfo)
        {
            if (folderInfo.IsEmpty)
                return $"{rule.DisplayName} AppData folder appears unused and is empty";

            return $"{rule.DisplayName} AppData folder appears unused, is not linked to an installed program, has no running related process, and has no recent activity";
        }

        private readonly record struct FolderInfo(
            long SizeBytes,
            int FileCount,
            int DirectoryCount,
            DateTime LastWriteTimeUtc)
        {
            public bool IsEmpty => FileCount == 0 && DirectoryCount == 0;
        }

        #region Debug 
        [Conditional("DEBUG")]
        private static void DebugCandidateDecision(DirectoryInfo folder, string decision, string reason)
        {
            if (!IsDebug)
                return;

            Debug.WriteLine($"│ Status:       {decision}");
            Debug.WriteLine($"│ Reason:       {reason}");
            Debug.WriteLine("├────────────────────────────────────────────────────────────");
        }

        [Conditional("DEBUG")]
        private static void DebugCandidateDecision(
            DirectoryInfo folder,
            string decision,
            AppDataOrphanDefinition rule,
            FolderInfo folderInfo,
            DirectoryScanConfidence confidence,
            string reason)
        {
            if (!IsDebug)
                return;

            Debug.WriteLine($"│ Decision:     {decision}");
            Debug.WriteLine($"│ Rule:         {rule.DisplayName}");
            Debug.WriteLine($"│ Confidence:   {confidence}");
            Debug.WriteLine($"│ Size:         {MathHelper.FormatBytes(folderInfo.SizeBytes)}");
            Debug.WriteLine($"│ Files:        {folderInfo.FileCount}");
            Debug.WriteLine($"│ Directories:  {folderInfo.DirectoryCount}");
            Debug.WriteLine($"│ LastWriteUtc: {folderInfo.LastWriteTimeUtc}");
            Debug.WriteLine("├────────────────────────────────────────────────────────────");
            Debug.WriteLine($"│ Reason:       {reason}");
            Debug.WriteLine("└────────────────────────────────────────────────────────────");

        }
        [Conditional("DEBUG")]
        private static void DebugCandidateStart(DirectoryInfo folder)
        {
            if (!IsDebug)
                return;

            Debug.WriteLine("");
            Debug.WriteLine("┌─────────────-──────────────────────────────────────────────");
            Debug.WriteLine($"│ Folder:       {folder.Name}");
            Debug.WriteLine($"│ Path:         {folder.FullName}");
            Debug.WriteLine("├────────────────────────────────────────────────────────────");
        }
        #endregion
    }
}
