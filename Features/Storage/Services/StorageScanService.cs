using GameBoost.Features.Storage.Models;
using GameBoost.Shared.FileSystem.Scanning;
using GameBoost.Shared.Helpers;
using GameBoost.Shared.Results;
using System.Collections.Concurrent;
using System.IO;

namespace GameBoost.Features.Storage.Services
{
    public sealed class StorageScanService
    {
        public IReadOnlyList<StorageDriveInfo> GetReadyDrives()
        {
            return [.. DriveInfo.GetDrives()
                .Where(drive => drive.IsReady && drive.DriveType == DriveType.Fixed)
                .Select(drive => new StorageDriveInfo
                {
                    Name = drive.Name,
                    RootPath = drive.RootDirectory.FullName,
                    TotalBytes = drive.TotalSize,
                    FreeBytes = drive.AvailableFreeSpace
                })];
        }

        public async Task<IReadOnlyList<StorageFolderNode>> ScanTopFoldersAsync(string rootPath, IProgress<ProgressResult>? progress, CancellationToken token, FileSystemScanOptions? options = null)
        {
            return await Task.Run(() =>
            {
                options ??= FileSystemScanProfiles.StorageTopFolders;
                var root = new DirectoryInfo(rootPath);

                var allTopLevelDirectories = SafeEnumerateDirectories(root).ToList();

                var topLevelDirectories = allTopLevelDirectories
                    .Where(directory => !FileSystemScanFilter.ShouldSkipDirectory(
                        directory,
                        options,
                        depth: 0,
                        isTopLevel: true)).ToList();

                var completedTopLevelFolders = 0;

                var results = new ConcurrentBag<StorageFolderNode>();

                var parallelOptions = new ParallelOptions
                {
                    CancellationToken = token,
                    MaxDegreeOfParallelism = options.MaxDegreeOfParallelism,
                };

                Parallel.ForEach(topLevelDirectories, parallelOptions, directory =>
                {
                    token.ThrowIfCancellationRequested();

                    var node = ScanFolder(
                       directory: directory,
                        options: options,
                        depth: 1,
                        token);

                    results.Add(node);

                    var complete = Interlocked.Increment(ref completedTopLevelFolders);

                    progress?.Report(
                        new ProgressResult(
                            $"Scanning {complete} of {topLevelDirectories.Count} folders",
                            MathHelper.ToPercentageInt(complete, topLevelDirectories.Count)));
                    });

                return results.OrderByDescending(folder => folder.SizeBytes).ToList();

            }, token);
        }

        private StorageFolderNode ScanFolder(DirectoryInfo directory, FileSystemScanOptions options, int depth, CancellationToken cancellationToken)
        {
            var node = new StorageFolderNode
            {
                Name = directory.Name,
                FullPath = directory.FullName
            };

            try
            {
                foreach (var file in SafeEnumerateFiles(directory))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (FileSystemScanFilter.ShouldSkipFile(file, options))
                        continue;

                    try
                    {
                        node.SizeBytes += file.Length;
                        node.FileCount++;
                    }
                    catch
                    {
                        // Ignore files that cannot be read.
                    }
                }

                foreach (var childDirectory in SafeEnumerateDirectories(directory))
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (FileSystemScanFilter.ShouldSkipDirectory(childDirectory, options, depth + 1, isTopLevel: false))
                        continue;

                    var childNode = ScanFolder(childDirectory, options, depth + 1, cancellationToken);

                    node.SizeBytes += childNode.SizeBytes;
                    node.FileCount += childNode.FileCount;
                    node.FolderCount += childNode.FolderCount + 1;

                    node.Children.Add(childNode);
                }

                node.Children.Sort((left, right) =>
                    right.SizeBytes.CompareTo(left.SizeBytes));
            }
            catch
            {
                node.IsAccessible = false;
            }

            return node;
        }

        private static IEnumerable<DirectoryInfo> SafeEnumerateDirectories(DirectoryInfo directory)
        {
            try
            {
                return directory.EnumerateDirectories();
            }
            catch
            {
                return [];
            }
        }

        private static IEnumerable<FileInfo> SafeEnumerateFiles(DirectoryInfo directory)
        {
            try
            {
                return directory.EnumerateFiles();
            }
            catch
            {
                return [];
            }
        }
    }
}