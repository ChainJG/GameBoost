using GameBoost.Features.Modules.SystemModules.Cleanup.Options;
using GameBoost.Shared.Results;
using System.Diagnostics;
using System.IO;

namespace GameBoost.Shared.Helpers
{
    public static class DirectoryCleanupHelper
    {
        public static bool DebugOutput { get; set; }

        private static readonly EnumerationOptions EnumerationOptions = new()
        {
            IgnoreInaccessible = true,
            RecurseSubdirectories = false,
            ReturnSpecialDirectories = false,
            AttributesToSkip = FileAttributes.ReparsePoint
        };

        #region Scan

        public static async Task<CleanupScanResult> ScanDeletableFilesAsync(
            IEnumerable<DirectoryInfo> rootDirectories,
            CleanupScanOptions options,
            CancellationToken token)
        {
            var roots = GetDistinctExistingDirectories(rootDirectories);

            if (roots.Count == 0)
                return CleanupScanResult.Empty;

            long estimatedDeletableBytes = 0;

            int scannedFiles = 0;
            int deletableFiles = 0;
            int skippedFiles = 0;
            int inaccessibleFiles = 0;
            int lockedFiles = 0;
            int scannedDirectories = 0;

            var newestAllowedWriteTimeUtc = options.IgnoreFilesNewerThan is null
                ? (DateTime?)null
                : DateTime.UtcNow.Subtract(options.IgnoreFilesNewerThan.Value);

            var files = EnumerateFilesSafe(
                roots,
                onDirectoryScanned: () => Interlocked.Increment(ref scannedDirectories),
                token);

            var parallelOptions = new ParallelOptions
            {
                CancellationToken = token,
                MaxDegreeOfParallelism = options.MaxDegreeOfParallelism
            };

            await Parallel.ForEachAsync(
                files,
                parallelOptions,
                (file, cancellationToken) =>
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    Interlocked.Increment(ref scannedFiles);

                    var status = TryGetDeletableFileSize(
                        file,
                        options.ProbeDeleteAccess,
                        newestAllowedWriteTimeUtc);

                    switch (status.Result)
                    {
                        case DeletableFileScanResult.Deletable:
                            Interlocked.Add(ref estimatedDeletableBytes, status.Size);
                            Interlocked.Increment(ref deletableFiles);
                            break;

                        case DeletableFileScanResult.Locked:
                            Interlocked.Increment(ref lockedFiles);
                            Interlocked.Increment(ref skippedFiles);
                            break;

                        case DeletableFileScanResult.Inaccessible:
                            Interlocked.Increment(ref inaccessibleFiles);
                            Interlocked.Increment(ref skippedFiles);
                            break;

                        default:
                            Interlocked.Increment(ref skippedFiles);
                            break;
                    }

                    return ValueTask.CompletedTask;
                });

            var result = new CleanupScanResult
            {
                EstimatedDeletableBytes = estimatedDeletableBytes,
                ScannedFiles = scannedFiles,
                DeletableFiles = deletableFiles,
                SkippedFiles = skippedFiles,
                InaccessibleFiles = inaccessibleFiles,
                LockedFiles = lockedFiles,
                ScannedDirectories = scannedDirectories
            };

#if DEBUG
            if (DebugOutput)
            {
                Debug.WriteLine(
                    $"Cleanup Scan | " +
                    $"EstimatedDeletable={MathHelper.FormatBytes(result.EstimatedDeletableBytes)} | " +
                    $"Files={result.ScannedFiles} | " +
                    $"Deletable={result.DeletableFiles} | " +
                    $"Skipped={result.SkippedFiles} | " +
                    $"Locked={result.LockedFiles} | " +
                    $"Inaccessible={result.InaccessibleFiles} | " +
                    $"Directories={result.ScannedDirectories}");
            }
#endif

            return result;
        }

        #endregion

        #region Delete

        public static CleanupResult DeleteDeletableFiles(
            IEnumerable<DirectoryInfo> rootDirectories,
            CancellationToken token,
            TimeSpan? ignoreFilesNewerThan = null)
        {
            var roots = GetDistinctExistingDirectories(rootDirectories);

            if (roots.Count == 0)
                return CleanupResult.Empty;

            long deletedBytes = 0;

            int deletedFiles = 0;
            int failedFiles = 0;
            int deletedDirectories = 0;

            foreach (var rootDirectory in roots)
            {
                token.ThrowIfCancellationRequested();

                var result = DeleteDeletableFiles(
                    rootDirectory,
                    token,
                    ignoreFilesNewerThan);

                deletedBytes += result.DeletedBytes;
                deletedFiles += result.DeletedFiles;
                failedFiles += result.FailedFiles;
                deletedDirectories += result.DeletedDirectories;
            }

            return new CleanupResult
            {
                DeletedBytes = deletedBytes,
                DeletedFiles = deletedFiles,
                FailedFiles = failedFiles,
                DeletedDirectories = deletedDirectories
            };
        }

        public static CleanupResult DeleteDeletableFiles(
            DirectoryInfo rootDirectory,
            CancellationToken token,
            TimeSpan? ignoreFilesNewerThan = null)
        {
            if (!rootDirectory.Exists)
                return CleanupResult.Empty;

            long deletedBytes = 0;

            int deletedFiles = 0;
            int failedFiles = 0;
            int deletedDirectories = 0;

            var newestAllowedWriteTimeUtc = ignoreFilesNewerThan is null
                ? (DateTime?)null
                : DateTime.UtcNow.Subtract(ignoreFilesNewerThan.Value);

            var pendingDirectories = new Stack<DirectoryInfo>();
            var visitedDirectories = new List<DirectoryInfo>();

            pendingDirectories.Push(rootDirectory);

            while (pendingDirectories.Count > 0)
            {
                token.ThrowIfCancellationRequested();

                var directory = pendingDirectories.Pop();
                visitedDirectories.Add(directory);

                var files = GetFilesSafe(directory);

                foreach (var file in files)
                {
                    token.ThrowIfCancellationRequested();

                    if (ShouldSkipFile(file, newestAllowedWriteTimeUtc))
                        continue;

                    try
                    {
                        var fileSize = file.Length;

                        if (file.IsReadOnly)
                            file.IsReadOnly = false;

                        file.Delete();

                        deletedBytes += fileSize;
                        deletedFiles++;
                    }
                    catch
                    {
                        failedFiles++;
                    }
                }

                var subDirectories = GetDirectoriesSafe(directory);

                foreach (var subDirectory in subDirectories)
                {
                    token.ThrowIfCancellationRequested();

                    pendingDirectories.Push(subDirectory);
                }
            }

            foreach (var directory in visitedDirectories
                         .OrderByDescending(directory => directory.FullName.Length))
            {
                token.ThrowIfCancellationRequested();

                if (SameDirectory(directory, rootDirectory))
                    continue;

                try
                {
                    if (!directory.EnumerateFileSystemInfos().Any())
                    {
                        directory.Delete();
                        deletedDirectories++;
                    }
                }
                catch
                {
                    // Ignore non-empty, locked, or inaccessible directories.
                }
            }

            if (DebugOutput)
            {
                Debug.WriteLine(
                    $"Cleanup Delete: {rootDirectory.FullName} | " +
                    $"Deleted={MathHelper.FormatBytes(deletedBytes)} | " +
                    $"Files={deletedFiles} | " +
                    $"Failed={failedFiles} | " +
                    $"Directories={deletedDirectories}");
            }

            return new CleanupResult
            {
                DeletedBytes = deletedBytes,
                DeletedFiles = deletedFiles,
                FailedFiles = failedFiles,
                DeletedDirectories = deletedDirectories
            };
        }

        public static Task<CleanupResult> DeleteDeletableFilesAsync(
            IEnumerable<DirectoryInfo> rootDirectories,
            CancellationToken token,
            TimeSpan? ignoreFilesNewerThan = null)
        {
            return Task.Run(
                () => DeleteDeletableFiles(
                    rootDirectories,
                    token,
                    ignoreFilesNewerThan),
                token);
        }

        #endregion

        #region Enumeration

        private static IEnumerable<FileInfo> EnumerateFilesSafe(
            IEnumerable<DirectoryInfo> rootDirectories,
            Action onDirectoryScanned,
            CancellationToken token)
        {
            var pending = new Stack<DirectoryInfo>();

            foreach (var rootDirectory in rootDirectories)
                pending.Push(rootDirectory);

            while (pending.Count > 0)
            {
                token.ThrowIfCancellationRequested();

                var directory = pending.Pop();

                onDirectoryScanned();

                foreach (var file in GetFilesSafe(directory))
                {
                    token.ThrowIfCancellationRequested();

                    yield return file;
                }

                foreach (var subDirectory in GetDirectoriesSafe(directory))
                {
                    token.ThrowIfCancellationRequested();

                    pending.Push(subDirectory);
                }
            }
        }

        private static IReadOnlyList<FileInfo> GetFilesSafe(
            DirectoryInfo directory)
        {
            try
            {
                return directory
                    .EnumerateFiles("*", EnumerationOptions)
                    .ToList();
            }
            catch
            {
                return [];
            }
        }

        private static IReadOnlyList<DirectoryInfo> GetDirectoriesSafe(
            DirectoryInfo directory)
        {
            try
            {
                return directory
                    .EnumerateDirectories("*", EnumerationOptions)
                    .ToList();
            }
            catch
            {
                return [];
            }
        }

        #endregion

        #region File Checks

        private static DeletableFileStatus TryGetDeletableFileSize(
            FileInfo file,
            bool probeDeleteAccess,
            DateTime? newestAllowedWriteTimeUtc)
        {
            try
            {
                if (ShouldSkipFile(file, newestAllowedWriteTimeUtc))
                    return DeletableFileStatus.Skipped();

                var size = file.Length;

                if (!probeDeleteAccess)
                    return DeletableFileStatus.Deletable(size);

                try
                {
                    using var stream = new FileStream(
                        file.FullName,
                        FileMode.Open,
                        FileAccess.ReadWrite,
                        FileShare.None,
                        bufferSize: 1,
                        FileOptions.SequentialScan);

                    return DeletableFileStatus.Deletable(size);
                }
                catch (IOException)
                {
                    return DeletableFileStatus.Locked();
                }
                catch (UnauthorizedAccessException)
                {
                    return DeletableFileStatus.Inaccessible();
                }
            }
            catch (UnauthorizedAccessException)
            {
                return DeletableFileStatus.Inaccessible();
            }
            catch (IOException)
            {
                return DeletableFileStatus.Locked();
            }
            catch
            {
                return DeletableFileStatus.Skipped();
            }
        }

        private static bool ShouldSkipFile(
            FileInfo file,
            DateTime? newestAllowedWriteTimeUtc)
        {
            try
            {
                if ((file.Attributes & FileAttributes.ReparsePoint) != 0)
                    return true;

                if ((file.Attributes & FileAttributes.Directory) != 0)
                    return true;

                if (newestAllowedWriteTimeUtc is not null &&
                    file.LastWriteTimeUtc > newestAllowedWriteTimeUtc.Value)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return true;
            }
        }

        #endregion

        #region Path Helpers

        private static IReadOnlyList<DirectoryInfo> GetDistinctExistingDirectories(
            IEnumerable<DirectoryInfo> directories)
        {
            return directories
                .Where(directory => directory.Exists)
                .GroupBy(directory => NormalizePath(directory.FullName))
                .Select(group => new DirectoryInfo(group.Key))
                .ToList();
        }

        private static bool SameDirectory(
            DirectoryInfo first,
            DirectoryInfo second)
        {
            return string.Equals(
                NormalizePath(first.FullName),
                NormalizePath(second.FullName),
                StringComparison.OrdinalIgnoreCase);
        }

        private static string NormalizePath(string path)
        {
            return Path.GetFullPath(path)
                .TrimEnd(
                    Path.DirectorySeparatorChar,
                    Path.AltDirectorySeparatorChar);
        }

        #endregion

        #region Internal Types

        private enum DeletableFileScanResult
        {
            Deletable,
            Locked,
            Inaccessible,
            Skipped
        }

        private readonly record struct DeletableFileStatus(
            DeletableFileScanResult Result,
            long Size)
        {
            public static DeletableFileStatus Deletable(long size) =>
                new(DeletableFileScanResult.Deletable, size);

            public static DeletableFileStatus Locked() =>
                new(DeletableFileScanResult.Locked, 0);

            public static DeletableFileStatus Inaccessible() =>
                new(DeletableFileScanResult.Inaccessible, 0);

            public static DeletableFileStatus Skipped() =>
                new(DeletableFileScanResult.Skipped, 0);
        }

        #endregion
    }
}