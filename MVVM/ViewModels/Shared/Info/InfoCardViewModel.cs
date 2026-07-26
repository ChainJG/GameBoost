using GameBoost.MVVM.Core;
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

        public required string Title { get; init; }

        public required PackIconKind Icon { get; init; }

        private string _info = string.Empty;
        public string Info { get => _info; set => Set(ref _info, value);  }


        private string _footer = string.Empty;
        public string Footer { get => _footer; set => Set(ref _footer, value); }

        public string? ToolTip { get; init; }

        public object? Content { get; init; } = null;

        public ICommand? Command { get; init; }

        public void BeginOperation(string footer = "Running...")
        {
            IsBusy = true;
            State = InfoCardState.Running;
            Footer = string.IsNullOrWhiteSpace(footer) ? Footer : footer;
        }

        public void CompleteOperation(string footer = "Completed")
        {
            IsBusy = false;
            State = InfoCardState.Success;
            Footer = string.IsNullOrWhiteSpace(footer) ? Footer : footer;
        }

        public void FailedOperation(string footer = "Failed")
        {
            IsBusy = false;
            State = InfoCardState.Error;
            Footer = string.IsNullOrWhiteSpace(footer) ? Footer : footer;
        }

        public async Task CancelOperation(string footer = "Cancelled")
        {
            IsBusy = false;
            State = InfoCardState.Warning;
            Footer = string.IsNullOrWhiteSpace(footer) ? Footer : footer;
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
