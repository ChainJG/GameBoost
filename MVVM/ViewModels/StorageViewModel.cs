using GameBoost.Application;
using GameBoost.Application.Diagnostics;
using GameBoost.Features.Storage.Services;
using GameBoost.MVVM.Core;
using GameBoost.MVVM.ViewModels.Storage;
using GameBoost.Shared.Helpers;
using GameBoost.Shared.Results;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;

namespace GameBoost.MVVM.ViewModels
{
    public class StorageViewModel : ObservableObject
    {
        private readonly GameBoostUIServices _uiServices;
        private readonly StorageScanService _storageScanService = new();

        private CancellationTokenSource? _scanCancellation;

        public StorageViewModel(GameBoostUIServices uiServices)
        {
            ScanCommand = new AsyncRelayCommand(ScanSelectedDriveAsync, CanScan);
            CancelScanCommand = new RelayCommand(CancelScan, CanCancelScan);

            NavigateToPathCommand = new AsyncRelayCommand<StoragePathSegmentViewModel>(NavigateToPathSegmentAsync, CanNavigateToPathSegment);

            _uiServices = uiServices;

            LoadDrives();
        }

        private bool _isChangingSelectionInternally;

        private const int MaximumCachedFolderSnapshots = 32;
        private const int ScanDisplayDelayTime = 500;


        private readonly Dictionary<string, StorageFolderNavigationSnapshot> _folderSnapshotCache = new(StringComparer.OrdinalIgnoreCase);

        public ObservableCollection<StoragePathSegmentViewModel> PathSegments { get; } = [];

        #region Commands
        public ICommand ScanCommand { get; }
        public ICommand CancelScanCommand { get; }
        public ICommand NavigateToPathCommand { get; }
        #endregion

        #region Drives ObservableCollection
        public ObservableCollection<StorageDriveCardViewModel> Drives { get; } = [];

        private StorageDriveCardViewModel? _selectedDrive;
        public StorageDriveCardViewModel? SelectedDrive
        {
            get => _selectedDrive;
            set
            {
                if (!Set(ref _selectedDrive, value))
                    return;

                OnPropertyChanged(nameof(HasSelectedDrive));

                if (!IsScanning)
                    StatusText = HasSelectedDrive
                        ? $"Ready to scan {value!.Name}" : "Select a drive to scan";

                RaiseCommandStates();
            }
        }
        public bool HasSelectedDrive => SelectedDrive is not null;
        #endregion

        #region Folders Nodes ObservableCollection
        public ObservableCollection<StorageFolderNodeViewModel> Folders { get; } = [];

        private StorageFolderNodeViewModel? _selectedFolder;
        public StorageFolderNodeViewModel? SelectedFolder
        {
            get => _selectedFolder;
            set
            {
                if (!Set(ref _selectedFolder, value))
                    return;

                if (_isChangingSelectionInternally)
                    return;

                if (value is null)
                    return;

                _ = NavigateToFolderAsync(value);
            }
        }
        #endregion

        #region Display Properties
        private StorageFolderUiState _folderUiState = StorageFolderUiState.Initial;
        public StorageFolderUiState FolderUiState
        {
            get => _folderUiState;
            set => Set(ref _folderUiState, value);
        }

        private bool _showDelayedScanningState;
        public bool ShowDelayedScanningState
        {
            get => _showDelayedScanningState;
            set
            {
                if (!Set(ref _showDelayedScanningState, value))
                    return;

                _uiServices?.GlobalOperations.SetOperationBoolean(value);
            }
        }

        private string _currentFolderDisplayName = "No folder selected";
        public string CurrentFolderDisplayName
        {
            get => _currentFolderDisplayName;
            set => Set(ref _currentFolderDisplayName, value);
        }

        private string _currentFolderPathText = string.Empty;
        public string CurrentFolderPathText
        {
            get => _currentFolderPathText;
            set => Set(ref _currentFolderPathText, value);
        }

        private CancellationTokenSource? _scanVisualDelayCancellation;
        #endregion

        private bool _isScanning;
        public bool IsScanning
        {
            get => _isScanning;
            set
            {
                if (!Set(ref _isScanning, value))
                    return;

                RaiseCommandStates();
            }
        }

        private string _statusText = "Select a drive to scan";
        public string StatusText
        {
            get => _statusText;
            set => Set(ref _statusText, value);
        }


        #region Drive Methods
        private void LoadDrives()
        {
            try
            {
                var previousRootPath = SelectedDrive?.RootPath;

                Drives.Clear();

                foreach (var drive in _storageScanService.GetReadyDrives())
                {
                    Drives.Add(new StorageDriveCardViewModel(drive));
                }

                SelectedDrive =
                    Drives.FirstOrDefault(drive =>
                        string.Equals(
                            drive.RootPath,
                            previousRootPath,
                            StringComparison.OrdinalIgnoreCase))
                    ?? Drives.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Drives.Clear();
                SelectedDrive = null;
                StatusText = "Failed to load storage drives";

#if DEBUG
                Debug.WriteLine($"Failed to load drives: {ex.Message}");
#endif
            }
        }
        private async Task ScanSelectedDriveAsync()
        {
            if (SelectedDrive is null)
                return;

            ClearSelectedFolder();

            PathSegments.Clear();
            _folderSnapshotCache.Clear();

            await OpenFolderPathAsync(SelectedDrive.RootPath, openedFolder: null, forceRefresh: true);
        }
        #endregion


        private async Task OpenFolderPathAsync(string folderPath, StorageFolderNodeViewModel? openedFolder, bool forceRefresh = false)
        {
            if (IsScanning)
                return;

            if (string.IsNullOrWhiteSpace(folderPath))
                return;

            folderPath = DirectoryHelper.NormalizeFolderPath(folderPath);

            if (!Directory.Exists(folderPath))
            {
                StatusText = "Selected folder no longer exists";
                return;
            }

            if (!forceRefresh && _folderSnapshotCache.TryGetValue(folderPath, out var cachedSnapshot))
            {
                LoadFolderSnapshot(cachedSnapshot);
                return;
            }

            await ScanFolderPathAsync(folderPath, openedFolder);
        }
        private async Task ScanFolderPathAsync(string folderPath, StorageFolderNodeViewModel? openedFolder)
        {

            if (string.IsNullOrWhiteSpace(folderPath))
                return;

            folderPath = DirectoryHelper.NormalizeFolderPath(folderPath);

            _scanCancellation?.Dispose();
            _scanCancellation = new CancellationTokenSource();

            var token = _scanCancellation.Token;

            IsScanning = true;
            ShowDelayedScanningState = false;

            CurrentFolderPathText = folderPath;
            CurrentFolderDisplayName = DirectoryHelper.GetFolderDisplayName(folderPath);

            StartDelayedScanningState(token);

            Folders.Clear();
            StatusText = $"Scanning {folderPath}...";

            try
            {
                var progress = new Progress<ProgressResult>(UpdateScanProgress);

                var folders = await GameBoostContext.Diagnostic.TrackAsync(
                    category: "Scan",
                    operationType: DiagnosticOperationType.FolderScan,
                    name: $"Folder: {folderPath}",
                    source: GetType().Name,
                    operation: innerToken => _storageScanService.ScanTopFoldersAsync(
                        folderPath,
                        progress,
                        innerToken),
                    token: token);

                if (token.IsCancellationRequested)
                {
                    HandleScanCancelled();
                    return;
                }

                var totalSizeBytes = folders.Sum(item => item.SizeBytes);

                var folderViewModels = folders
                    .Select(folder => new StorageFolderNodeViewModel(
                        folder,
                        totalSizeBytes,
                        RemoveDeletedFolderNode))
                    .ToList();

                foreach (var folder in folderViewModels)
                    Folders.Add(folder);

                var snapshot = new StorageFolderNavigationSnapshot
                {
                    FullPath = folderPath,
                    OpenedFolder = openedFolder,
                    Folders = folderViewModels,
                    TotalSizeBytes = totalSizeBytes,
                };

                SaveFolderSnapshot(snapshot);

                BuildPathSegments(folderPath, openedFolder);

                FolderUiState = Folders.Count > 0
                    ? StorageFolderUiState.Results
                    : StorageFolderUiState.EmptyFolder;

                StatusText = Folders.Count > 0
                    ? $"Found {Folders.Count} folders"
                    : "No folders found in this location";
            }
            catch (OperationCanceledException)
            {
                HandleScanCancelled();
            }
            catch (Exception ex)
            {
                StatusText = "Storage scan failed";
                FolderUiState = StorageFolderUiState.Failed;

#if DEBUG
                Debug.WriteLine($"Storage scan failed: {ex.Message}");
#endif
            }
            finally
            {
                IsScanning = false;
                StopDelayedScanningState();
            }
        }

        private void HandleScanCancelled()
        {
            Folders.Clear();
            PathSegments.Clear();

            StatusText = "Storage scan cancelled";
            FolderUiState = StorageFolderUiState.Initial;
            ShowDelayedScanningState = false;
        }

        #region Navigation Methods
        private async Task NavigateToFolderAsync(StorageFolderNodeViewModel folder)
        {
            if (IsScanning)
                return;

            if (string.IsNullOrWhiteSpace(folder.FullPath))
                return;

            if (!Directory.Exists(folder.FullPath))
            {
                StatusText = "Selected folder no longer exists";
                return;
            }

            ClearSelectedFolder();

            await OpenFolderPathAsync(folder.FullPath, openedFolder: folder);
        }
        private async Task NavigateToPathSegmentAsync(StoragePathSegmentViewModel? segment)
        {
            if (segment is null)
                return;

            if (IsScanning)
                return;

            if (segment.CachedSnapshot is not null)
            {
                LoadFolderSnapshot(segment.CachedSnapshot);
                return;
            }

            await OpenFolderPathAsync(segment.FullPath, openedFolder: segment.OpenedFolder, forceRefresh: false);
        }
        #endregion

        #region Segments Methods
        private void BuildPathSegments(string folderPath, StorageFolderNodeViewModel? openedFolder)
        {
            PathSegments.Clear();

            if (string.IsNullOrWhiteSpace(folderPath))
                return;

            var fullPath = DirectoryHelper.NormalizeFolderPath(folderPath);
            var root = Path.GetPathRoot(fullPath);

            if (string.IsNullOrWhiteSpace(root))
                return;

            var normalizedRoot = DirectoryHelper.NormalizeFolderPath(root);

            AddPathSegment(
                displayName: normalizedRoot,
                fullPath: normalizedRoot,
                isRoot: true,
                isCurrent: string.Equals(
                    normalizedRoot,
                    fullPath,
                    StringComparison.OrdinalIgnoreCase),
                openedFolder: openedFolder);

            var relativePath = Path.GetRelativePath(
                normalizedRoot,
                fullPath);

            if (string.IsNullOrWhiteSpace(relativePath) ||
                relativePath == ".")
            {
                return;
            }

            var currentPath = normalizedRoot;

            foreach (var part in relativePath.Split(
                         Path.DirectorySeparatorChar,
                         Path.AltDirectorySeparatorChar))
            {
                if (string.IsNullOrWhiteSpace(part))
                    continue;

                currentPath = DirectoryHelper.NormalizeFolderPath(
                    Path.Combine(currentPath, part));

                var isCurrent = string.Equals(
                    currentPath,
                    fullPath,
                    StringComparison.OrdinalIgnoreCase);

                AddPathSegment(
                    displayName: part,
                    fullPath: currentPath,
                    isRoot: false,
                    isCurrent: isCurrent,
                    openedFolder: isCurrent ? openedFolder : null);
            }
        }
        private void AddPathSegment(string displayName, string fullPath, bool isRoot, bool isCurrent, StorageFolderNodeViewModel? openedFolder)
        {
            _folderSnapshotCache.TryGetValue(
                fullPath,
                out var cachedSnapshot);

            PathSegments.Add(new StoragePathSegmentViewModel
            {
                DisplayName = displayName,
                FullPath = fullPath,
                IsRoot = isRoot,
                IsCurrent = isCurrent,
                OpenedFolder = cachedSnapshot?.OpenedFolder ?? openedFolder,
                CachedSnapshot = cachedSnapshot
            });
        }
        #endregion

        #region Snapshot Methods
        private void LoadFolderSnapshot(StorageFolderNavigationSnapshot snapshot)
        {
            Folders.Clear();

            foreach (var folder in snapshot.Folders)
                Folders.Add(folder);

            CurrentFolderPathText = snapshot.FullPath;
            CurrentFolderDisplayName = DirectoryHelper.GetFolderDisplayName(snapshot.FullPath);

            BuildPathSegments(
                snapshot.FullPath,
                snapshot.OpenedFolder);

            FolderUiState = Folders.Count > 0 ? StorageFolderUiState.Results : StorageFolderUiState.EmptyFolder;

            StatusText = Folders.Count > 0 ? $"Found {Folders.Count} folders" : "No folders found in this location";
        }
        private void SaveFolderSnapshot(StorageFolderNavigationSnapshot snapshot)
        {
            _folderSnapshotCache[snapshot.FullPath] = snapshot;

            if (_folderSnapshotCache.Count <= MaximumCachedFolderSnapshots)
                return;

            var oldestSnapshot = _folderSnapshotCache
                .OrderBy(pair => pair.Value.CreatedAtUtc)
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(oldestSnapshot.Key))
                _folderSnapshotCache.Remove(oldestSnapshot.Key);
        }
        private void RemoveDeletedFolderFromSnapshot(string deletedFolderPath)
        {
            if (string.IsNullOrWhiteSpace(deletedFolderPath))
                return;

            var normalizedDeletedPath = DirectoryHelper.NormalizeFolderPath(deletedFolderPath);

            var cacheKeysToRemove = _folderSnapshotCache.Keys
                .Where(cachePath =>
                    IsSamePathOrChildPath(
                        cachePath,
                        normalizedDeletedPath))
                .ToList();

            foreach (var cacheKey in cacheKeysToRemove)
                _folderSnapshotCache.Remove(cacheKey);

            var snapshotsToUpdate = _folderSnapshotCache
                .Where(pair =>
                    pair.Value.Folders.Any(folder =>
                        IsSamePathOrChildPath(
                            folder.FullPath,
                            normalizedDeletedPath)))
                .ToList();

            foreach (var snapshotPair in snapshotsToUpdate)
            {
                var oldSnapshot = snapshotPair.Value;

                var updatedFolders = oldSnapshot.Folders
                    .Where(folder =>
                        !IsSamePathOrChildPath(
                            folder.FullPath,
                            normalizedDeletedPath))
                    .ToList();

                var updatedSnapshot = new StorageFolderNavigationSnapshot
                {
                    FullPath = oldSnapshot.FullPath,
                    OpenedFolder = oldSnapshot.OpenedFolder,
                    Folders = updatedFolders,
                    TotalSizeBytes = updatedFolders.Sum(folder => folder.SizeBytes),
                };

                _folderSnapshotCache[snapshotPair.Key] = updatedSnapshot;
            }
        }
        #endregion

        #region Helper Methods
        private void ClearSelectedFolder()
        {
            _isChangingSelectionInternally = true;

            try
            {
                SelectedFolder = null;
            }
            finally
            {
                _isChangingSelectionInternally = false;
            }
        }
        private void RemoveDeletedFolderNode(StorageFolderNodeViewModel deletedFolder)
        {
            Folders.Remove(deletedFolder);

            RemoveDeletedFolderFromSnapshot(deletedFolder.FullPath);

            if (!string.IsNullOrWhiteSpace(CurrentFolderPathText))
            {
                var normalizedCurrentPath = DirectoryHelper.NormalizeFolderPath(CurrentFolderPathText);

                var currentSnapshot = new StorageFolderNavigationSnapshot
                {
                    FullPath = normalizedCurrentPath,
                    OpenedFolder = null,
                    Folders = Folders.ToList(),
                    TotalSizeBytes = Folders.Sum(folder => folder.SizeBytes),
                };

                SaveFolderSnapshot(currentSnapshot);

                BuildPathSegments(
                    normalizedCurrentPath,
                    currentSnapshot.OpenedFolder);
            }

            FolderUiState = Folders.Count > 0
                ? StorageFolderUiState.Results
                : StorageFolderUiState.EmptyFolder;

            StatusText = $"Deleted {deletedFolder.Name}";
        }
        private static bool IsSamePathOrChildPath(string path,string parentPath)
        {
            var normalizedPath = DirectoryHelper.NormalizeFolderPath(path);
            var normalizedParentPath = DirectoryHelper.NormalizeFolderPath(parentPath);

            if (string.Equals(
                    normalizedPath,
                    normalizedParentPath,
                    StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return normalizedPath.StartsWith(
                normalizedParentPath + Path.DirectorySeparatorChar,
                StringComparison.OrdinalIgnoreCase);
        }
        #endregion

        #region CanExecute Methods
        private bool CanNavigateToPathSegment(StoragePathSegmentViewModel? segment)
        {
            return !IsScanning &&
                   segment is not null &&
                   !string.IsNullOrWhiteSpace(segment.FullPath) &&
                   Directory.Exists(segment.FullPath);
        }
        private bool CanScan() => !IsScanning && SelectedDrive is not null;
        private bool CanCancelScan() => IsScanning;
        private void CancelScan()
        {
            if (!IsScanning)
                return;

            _scanCancellation?.Cancel();
        }
        #endregion

        #region Scanning Delay Methods
        private void StartDelayedScanningState(CancellationToken scanToken)
        {
            _scanVisualDelayCancellation?.Cancel();
            _scanVisualDelayCancellation?.Dispose();

            _scanVisualDelayCancellation = CancellationTokenSource.CreateLinkedTokenSource(scanToken);

            _ = ShowScanningStateAfterDelayAsync(_scanVisualDelayCancellation.Token);
        }
        private async Task ShowScanningStateAfterDelayAsync(CancellationToken token)
        {
            try
            {
                FolderUiState = StorageFolderUiState.None;

                await Task.Delay(ScanDisplayDelayTime, token);

                if (!token.IsCancellationRequested && IsScanning)
                    ShowDelayedScanningState = true;
            }
            catch (OperationCanceledException)
            {
                // Expected when scan completes before the delay.
            }
        }
        private void StopDelayedScanningState()
        {
            _scanVisualDelayCancellation?.Cancel();
            ShowDelayedScanningState = false;
        }
        #endregion

        private void UpdateScanProgress(ProgressResult result)
        {
            StatusText = result.Status;
        }

        private void RaiseCommandStates()
        {
            if (ScanCommand is AsyncRelayCommand scanCommand)
                scanCommand.RaiseCanExecuteChanged();

            if (CancelScanCommand is RelayCommand cancelCommand)
                cancelCommand.RaiseCanExecuteChanged();

            if (NavigateToPathCommand is AsyncRelayCommand<StoragePathSegmentViewModel> navigateCommand)
                navigateCommand.RaiseCanExecuteChanged();
        }
    }

    public enum StorageFolderUiState
    {
        None,
        Initial,
        Results,
        EmptyFolder,
        Failed
    }
}
