using GameBoost.Core.Interfaces;
using GameBoost.MVVM.Core;
using GameBoost.Shared.Results;
using MaterialDesignThemes.Wpf;

namespace GameBoost.MVVM.ViewModels.Shared.Selection
{
    public class SelectionResultViewModel(SelectionActionViewModel action) : ObservableObject, ISelectionButton
    {
        public string Title { get; set; } = action.Title;
        public PackIconKind Icon { get; set; } = action.Icon;
        public IActionModule? Module { get; set; } = action.Module ?? null;

        private string? _status;
        public string? Status { get => _status; set => Set(ref _status, value); }

        public ModuleResult? Result { get; set; }

        private ResultButtonState _state;
        public ResultButtonState State { get => _state; set => Set(ref _state, value); }
    }

    public enum ResultButtonState
    {
        Running,
        Result,
    }
}
