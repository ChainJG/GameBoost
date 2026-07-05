using GameBoost.MVVM.Core;
using GameBoost.Shared.Results;
using MaterialDesignThemes.Wpf;
using System.Windows.Input;

namespace GameBoost.MVVM.ViewModels.Shared.Info
{
    public sealed class InfoCardViewModel : ObservableObject
    {
        private bool _isBusy;
        public bool IsBusy { get => _isBusy; set => Set(ref _isBusy, value); }

        private InfoCardState _state = InfoCardState.Display;
        public InfoCardState State
        {
            get => _state;
            set
            {
                Set(ref _state, value);
                OnPropertyChanged(nameof(StateIcon));
            }
        }

        public IProgress<ProgressResult> Progress { get; }

        public InfoCardViewModel()
        {
            Progress = new Progress<ProgressResult>(UpdateProgress);
        }

        private void UpdateProgress(ProgressResult result)
        {
            Footer = result.Status;
        }

        public required PackIconKind Icon { get; init; }

        public required string Title { get; init; }

        private string _info = string.Empty;
        public string Info { get => _info; set => Set(ref _info, value);  }


        private string _footer = string.Empty;
        public string Footer { get => _footer; set => Set(ref _footer, value); }

        public string? ToolTip { get; init; }

        public object? Content { get; init; } = null;

        public ICommand? Command { get; init; }

        public void BeginOperation()
        {
            IsBusy = true;
            State = InfoCardState.Running;
        }

        public void CompleteOperation()
        {
            IsBusy = false;
            State = InfoCardState.Info;
        }

        public void FailedOperation()
        {
            IsBusy = false;
            State = InfoCardState.Error;
        }

        public async Task CancelOperation()
        {
            IsBusy = false;
            State = InfoCardState.Warning;
            await Task.Delay(1000);
            State = InfoCardState.Info;
        }

        public PackIconKind StateIcon => State switch
        {
            InfoCardState.Recommended => PackIconKind.CheckCircleOutline,
            InfoCardState.Success => PackIconKind.AlertCircleCheckOutline,
            InfoCardState.Warning => PackIconKind.AlertOutline,
            InfoCardState.Error => PackIconKind.AlertCircleOutline,
            InfoCardState.Performance => PackIconKind.Speedometer,
            InfoCardState.Disabled => PackIconKind.MinusCircleOutline,
            InfoCardState.Unknown => PackIconKind.HelpCircleOutline,
            _ => PackIconKind.InformationOutline
        };
    }
}
