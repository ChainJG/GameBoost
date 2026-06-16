using GameBoost.Application;
using GameBoost.Application.Diagnostics;
using GameBoost.Features.Storage.Services;
using GameBoost.MVVM.Core;
using GameBoost.MVVM.ViewModels.Storage;
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
            RefreshCommand = new AsyncRelayCommand(RefreshAsync, CanScan);

            NavigateToPathCommand = new AsyncRelayCommand<string>(NavigateToPathFromBreadcrumbAsync);

            _uiServices = uiServices;

            LoadDrives();
        }

        private bool _isChangingSelectionInternally;
        private string? _currentFolderPath;

        public ObservableCollection<StoragePathSegmentViewModel> PathSegments { get; } = [];
        public ObservableCollection<StorageDriveCardViewModel> Drives { get; } = [];
        public ObservableCollection<StorageFolderNodeViewModel> Folders { get; } = [];

        public ICommand ScanCommand { get; }
        public ICommand CancelScanCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand NavigateToPathCommand { get; }

        private StorageDriveCardViewModel? _selectedDrive;
        public StorageDriveCardViewModel? SelectedDrive
        {
            get => _selectedDrive;
            set
            {
                if (!Set(ref _selectedDrive, value))
                    return;

                StatusText = value is null
                    ? "Select a drive to scan"
                    : $"Ready to scan {value.Name}";

                RaiseCommandStates();
            }
        }

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

        private bool _isScanning;
        public bool IsScanning
        {
            get => _isScanning;
            set
            {
                if (!Set(ref _isScanning, value))
                    return;

                _uiServices?.GlobalOperations.SetOperationBoolean(value);
                RaiseCommandStates();
            }
        }

        private string _statusText = "Select a drive to scan";
        public string StatusText
        {
            get => _statusText;
            set => Set(ref _statusText, value);
        }

        private string _currentLocationText = "No folder selected";
        public string CurrentLocationText
        {
            get => _currentLocationText;
            set => Set(ref _currentLocationText, value);
        }

        private void LoadDrives()
        {
            Drives.Clear();

            foreach (var drive in _storageScanService.GetReadyDrives())
            {
                Drives.Add(new StorageDriveCardViewModel(drive));
            }

            SelectedDrive = Drives.FirstOrDefault();
        }
        private async Task ScanSelectedDriveAsync()
        {
            if (SelectedDrive is null)
                return;

            ClearSelectedFolder();

            await ScanFolderPathAsync(SelectedDrive.RootPath);
        }

        private async Task ScanFolderPathAsync(string folderPath)
        {

            if (string.IsNullOrWhiteSpace(folderPath))
                return;

            _scanCancellation?.Dispose();
            _scanCancellation = new CancellationTokenSource();

            IsScanning = true;

            Folders.Clear();

            _currentFolderPath = NormalizeFolderPath(folderPath);
            CurrentLocationText = _currentFolderPath;

            BuildPathSegments(_currentFolderPath);

            StatusText = $"Scanning {folderPath}...";

            try
            {
                var progress = new Progress<ProgressResult>(UpdateScanProgress);

                var folders = await GameBoostContext.Diagnostic.TrackAsync(
                    category: "Scan",
                    operationType: DiagnosticOperationType.FolderScan,
                    name: $"Folder: {folderPath}",
                    source: GetType().Name,
                    operation: _ => Task.Run(() => _storageScanService.ScanTopFoldersAsync(folderPath, progress, _scanCancellation.Token), _scanCancellation.Token),
                    token: _scanCancellation.Token);

                var totalSizeBytes = folders.Sum(item => item.SizeBytes);

                foreach (var folder in folders)
                {
                    Folders.Add(new StorageFolderNodeViewModel(
                        folder,
                        totalSizeBytes));
                }

                StatusText = $"Found {Folders.Count} folders";
            }
            catch (OperationCanceledException)
            {
                StatusText = "Storage scan cancelled";
            }
            catch (Exception ex)
            {
                StatusText = "Storage scan failed";

#if DEBUG
                Debug.WriteLine($"Storage scan failed: {ex.Message}");
#endif
            }
            finally
            {
                IsScanning = false;
            }
        }

        private async Task NavigateToFolderAsync(StorageFolderNodeViewModel folder)
        {
            if (IsScanning)
                return;

            if (string.IsNullOrWhiteSpace(folder.FullPath))
                return;

            if (!Directory.Exists(folder.FullPath))
            {
                StatusText = "Selected folder no longer exists.";
                return;
            }

            ClearSelectedFolder();

            await ScanFolderPathAsync(folder.FullPath);
        }



        private async Task RefreshAsync()
        {
            LoadDrives();

            if (!string.IsNullOrWhiteSpace(_currentFolderPath) &&
                Directory.Exists(_currentFolderPath))
            {
                await ScanFolderPathAsync(_currentFolderPath);
                return;
            }

            await ScanSelectedDriveAsync();
        }




        private Task NavigateToPathFromBreadcrumbAsync(string? folderPath)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                return Task.CompletedTask;

            return ScanFolderPathAsync(folderPath);
        }

        private void BuildPathSegments(string folderPath)
        {
            PathSegments.Clear();

            if (string.IsNullOrWhiteSpace(folderPath))
                return;

            var fullPath = NormalizeFolderPath(folderPath);
            var root = Path.GetPathRoot(fullPath);

            if (string.IsNullOrWhiteSpace(root))
                return;

            var normalizedRoot = NormalizeFolderPath(root);

            PathSegments.Add(new StoragePathSegmentViewModel
            {
                DisplayName = normalizedRoot,
                FullPath = normalizedRoot,
                IsRoot = true,
                IsCurrent = string.Equals(normalizedRoot, fullPath, StringComparison.OrdinalIgnoreCase),
            });

            var relativePath = Path.GetRelativePath(
                normalizedRoot,
                fullPath);

            if (string.IsNullOrWhiteSpace(relativePath) ||
                relativePath == ".")
            {
                return;
            }

            var currentPath = normalizedRoot;
            var parts = relativePath.Split(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar);

            foreach (var part in parts)
            {
                if (string.IsNullOrWhiteSpace(part))
                    continue;

                currentPath = NormalizeFolderPath(
                    Path.Combine(currentPath, part));

                var isCurrent = string.Equals(
                    currentPath,
                    fullPath,
                    StringComparison.OrdinalIgnoreCase);

                PathSegments.Add(new StoragePathSegmentViewModel
                {
                    DisplayName = part,
                    FullPath = currentPath,
                    IsCurrent = isCurrent,
                });
            }
        }
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

        private static string NormalizeFolderPath(string folderPath)
        {
            var fullPath = Path.GetFullPath(folderPath);

            var root = Path.GetPathRoot(fullPath);

            if (string.Equals(fullPath, root, StringComparison.OrdinalIgnoreCase))
                return fullPath;

            return fullPath.TrimEnd(
                Path.DirectorySeparatorChar,
                Path.AltDirectorySeparatorChar);
        }

        private bool CanScan() => !IsScanning && SelectedDrive is not null;
        private bool CanCancelScan() => IsScanning;
        private void CancelScan()
        {
            if (!IsScanning)
                return;

            _scanCancellation?.Cancel();
        }

        private void UpdateScanProgress(ProgressResult result)
        {
            StatusText = result.Status;
        }

        private void RaiseCommandStates()
        {
            if (ScanCommand is AsyncRelayCommand scanCommand)
                scanCommand.RaiseCanExecuteChanged();

            if (RefreshCommand is AsyncRelayCommand refreshCommand)
                refreshCommand.RaiseCanExecuteChanged();

            if (CancelScanCommand is RelayCommand cancelCommand)
                cancelCommand.RaiseCanExecuteChanged();
        }
    }
}
