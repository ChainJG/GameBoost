using GameBoost.Application.FileLookup;
using GameBoost.MVVM.Core;
using GameBoost.Shared.Helpers;
using GameBoost.Shared.Helpers.ProcessHelpers;
using GameBoost.Shared.Results;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace GameBoost.MVVM.ViewModels
{
    public class ProcessLookupViewModel : ObservableObject
    {
        private readonly string _filePath;

        public string SelectedFileText { get; set; }

        private string _processResult;
        public string ProcessResult
        {
            get => _processResult;
            set => Set(ref _processResult, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            private set
            {
                if (!Set(ref _isBusy, value))
                    return;

                RaiseCommandCanExecuteChanged();
                OnPropertyChanged(nameof(CanRunProcessAction));
            }
        }

        #region RelayCommands
        public bool CanRunProcessAction => !IsBusy && FileLockCards.Any(card => card.IsChecked);

        public RelayCommand LocateCommand { get; }
        public AsyncRelayCommand EndAllProcessesCommand { get; }
        public AsyncRelayCommand RefreshCommand { get; }
        public AsyncRelayCommand CloseSelectedProcessCommand { get; }
        public AsyncRelayCommand EndSelectedProcessCommand { get; }
        #endregion

        private ObservableCollection<FileLockInfoViewModel> _fileLockCards = [];

        public ObservableCollection<FileLockInfoViewModel> FileLockCards
        {
            get => _fileLockCards;
            private set
            {
                UnsubscribeFromCardEvents(_fileLockCards);

                if (!Set(ref _fileLockCards, value))
                    return;

                SubscribeToCardEvents(_fileLockCards);
                RaiseSelectionStateChanged();
            }
        }
        public ProcessLookupViewModel(string filePath)
        {
            _filePath = filePath;

            SelectedFileText = Path.GetFileName(filePath);

            LocateCommand = new RelayCommand(LocateFile);

            EndAllProcessesCommand = new AsyncRelayCommand(EndAllProcessesAsync, CanRunRefresh);

            RefreshCommand = new AsyncRelayCommand(
                RefreshProcessListAsync,
                CanRunRefresh);

            CloseSelectedProcessCommand = new AsyncRelayCommand(
                CloseSelectedProcessesAsync,
                CanRunSelectedProcessAction);

            EndSelectedProcessCommand = new AsyncRelayCommand(
                EndSelectedProcessesAsync,
                CanRunSelectedProcessAction);

            _ = RefreshProcessListAsync();
        }

        private void LocateFile()
        {
            DirectoryHelper.OpenFileInExplorer(_filePath);
        }

        private bool CanRunRefresh() => !IsBusy;

        private bool CanRunSelectedProcessAction() => !IsBusy && FileLockCards.Any(card => card.IsChecked);

        private async Task EndAllProcessesAsync()
        {
            if (IsBusy)
                return;

            foreach (var process in FileLockCards)
                process.IsChecked = true;

            MessageBoxResult boxResult = MessageBox.Show(
                "This may cause unsaved work to be lost\nDo you want to end all processes?",
                "Confirm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (boxResult != MessageBoxResult.Yes)
                return;

            await RunProcessActionAsync(
                actionName: "end",
                action: ProcessHelper.EndProcessByName);
        }

        private async Task RefreshProcessListAsync()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                await LoadLockingProcessesCoreAsync();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadLockingProcessesCoreAsync()
        {
            try
            {
                IsBusy = true;

                ProcessResult = "Checking for processes using this file...";

                var processes = await Task.Run(() => FileLockLookupService.GetLockingProcesses(_filePath));

                FileLockCards = new ObservableCollection<FileLockInfoViewModel>(processes.Select(process => new FileLockInfoViewModel(process)));

                ProcessResult = FileLockCards.Count switch
                {
                    0 => "No processes are currently using this file",
                    1 => $"Found {FileLockCards.Count} process using this file",
                    _ => $"Found {FileLockCards.Count} processes using this file"
                };
            }
            catch (Exception ex)
            {
                ProcessResult = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CloseSelectedProcessesAsync() => await RunProcessActionAsync("close", ProcessHelper.CloseProcessByName);
        private async Task EndSelectedProcessesAsync()
        {
            var selectedCards = GetSelectedFileLockCards();

            if (selectedCards.Count == 0)
            {
                ProcessResult = "Select at least one process first.";
                return;
            }

            MessageBoxResult boxResult = MessageBox.Show(
                "This may cause unsaved work to be lost\nDo you want to end the selected processes?",
                "Confirm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (boxResult != MessageBoxResult.Yes)
                return;

            await RunProcessActionAsync(
                actionName: "end",
                action: ProcessHelper.EndProcessByName);
        }

        private async Task RunProcessActionAsync(string actionName, Func<string, ModuleResult> action)
        {
            if (IsBusy)
                return;

            var selectedCards = GetSelectedFileLockCards();

            if (selectedCards.Count == 0)
            {
                ProcessResult = "Select at least one process first";
                return;
            }

            try
            {
                IsBusy = true;

                ProcessResult = $"Typing to {actionName} {selectedCards.Count} processes...";

                var results = await Task.Run(() =>
                    selectedCards
                        .Select(card =>
                        {
                            FileLockInfo processInfo = card.FileLockInfo;
                            ModuleResult result = action(processInfo.ProcessName);

                            return new ProcessActionResult(processInfo.ProcessName, processInfo.ProcessId, result);
                        }).ToList());

                int successCount = results.Count(result => result.Result.Success);
                int failedCount = results.Count - successCount;

                await LoadLockingProcessesCoreAsync();

                ProcessResult = BuildProcessActionSummary(
                    actionName,
                    successCount,
                    failedCount,
                    FileLockCards.Count);
            }
            finally
            {
                IsBusy = false;

            }

        }

        private IReadOnlyList<FileLockInfoViewModel> GetSelectedFileLockCards() => [.. FileLockCards.Where(card => card.IsChecked)];


        private static string BuildProcessActionSummary(string actionName, int successCount, int failedCount, int remainingLockCount)
        {
            string actionPastText = actionName switch
            {
                "close" => "closed",
                "end" => "ended",
                _ => "processed"
            };

            if (failedCount == 0)

                return remainingLockCount == 0
                    ? $"Successfully {actionPastText} {successCount} processes. No processes are currently using this file"
                    : $"Successfully {actionPastText} {successCount} process(es). {remainingLockCount} process(es) still appear to be using this file.";

            return $"{successCount} process(es) {actionPastText}. {failedCount} process(es) failed. {remainingLockCount} process(es) still appear to be using this file";
        }


        private void SubscribeToCardEvents(ObservableCollection<FileLockInfoViewModel> cards)
        {
            foreach (FileLockInfoViewModel card in cards)
                card.PropertyChanged += FileLockCard_PropertyChanged;
        }

        private void UnsubscribeFromCardEvents(ObservableCollection<FileLockInfoViewModel> cards)
        {
            foreach (FileLockInfoViewModel card in cards)
                card.PropertyChanged -= FileLockCard_PropertyChanged;
        }


        private void FileLockCard_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(FileLockInfoViewModel.IsChecked))
                return;

            RaiseSelectionStateChanged();
        }

        private void RaiseSelectionStateChanged()
        {
            OnPropertyChanged(nameof(CanRunProcessAction));
            RaiseCommandCanExecuteChanged();
        }

        private void RaiseCommandCanExecuteChanged()
        {
            EndAllProcessesCommand.RaiseCanExecuteChanged();
            RefreshCommand.RaiseCanExecuteChanged();
            CloseSelectedProcessCommand.RaiseCanExecuteChanged();
            EndSelectedProcessCommand.RaiseCanExecuteChanged();
        }

        private sealed record ProcessActionResult(
            string ProcessName,
            int ProcessId,
            ModuleResult Result);

    }
}