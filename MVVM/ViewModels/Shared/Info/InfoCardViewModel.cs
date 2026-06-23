using GameBoost.MVVM.Core;
using MaterialDesignThemes.Wpf;
using System.Windows.Input;

namespace GameBoost.MVVM.ViewModels.Shared.Info
{
    public sealed class InfoCardViewModel : ObservableObject
    {
        public bool IsBusy { get; set; } = false;
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

        public required PackIconKind Icon { get; init; }

        public required string Title { get; init; }

        public required string Info { get; init; }

        private string _footer = string.Empty;
        public string Footer { get => _footer; set => Set(ref _footer, value); }

        public string? ToolTip { get; init; }

        public object? Content { get; init; } = null;

        public ICommand? Command { get; init; }

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
