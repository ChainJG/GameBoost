using GameBoost.Features.Modules.SystemModules.Cleanup.Options;
using GameBoost.Shared.Results;
using System.Collections.Concurrent;
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

        public static async Task<CleanupScanResult> ScanDeletableFilesAsync(
            IEnumerable<DirectoryInfo> rootDirectories,
            CleanupScanOptions options,
            CancellationToken token)
        {
            var roots = rootDirectories
                .Where(directory => directory.Exists)
                .GroupBy(directory => NormalizePath(directory.FullName))
                .Select(group => new DirectoryInfo(group.Key))
                .ToList();

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

            if (DebugOutput)
            {
                Debug.WriteLine(
                    $"Cleanup Scan | " +
                    $"EstimatedDeletable={MathHelper.FormatBytes(result.EstimatedDeletableBytes)} | " +
                    $"Files={result.ScannedFiles} | " +
                    $"Deletable={result.DeletableFiles} | " +
                    $"Skipped={result.SkippedFiles} | " +
                    $"Locked={result.LockedFiles} | " +
                    $"Inaccessible={result.InaccessibleFiles}");
            }

            return result;
        }

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

                IEnumerable<FileInfo> files;

                try
                {
                    files = directory.EnumerateFiles("*", EnumerationOptions);
                }
                catch
                {
                    continue;
                }

                foreach (var file in files)
                {
                    token.ThrowIfCancellationRequested();
                    yield return file;
                }

                IEnumerable<DirectoryInfo> subDirectories;

                try
                {
                    subDirectories = directory.EnumerateDirectories("*", EnumerationOptions);
                }
                catch
                {
                    continue;
                }

                foreach (var subDirectory in subDirectories)
                {
                    token.ThrowIfCancellationRequested();
                    pending.Push(subDirectory);
                }
            }
        }

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


        private static string NormalizePath(string path)
        {
            return Path.GetFullPath(path)
                .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

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
    }
}