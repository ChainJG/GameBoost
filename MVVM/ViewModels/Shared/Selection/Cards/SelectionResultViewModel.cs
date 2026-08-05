using GameBoost.Core.Interfaces;
using GameBoost.MVVM.Core;
using GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions;
using GameBoost.Shared.Results;
using MaterialDesignThemes.Wpf;

namespace GameBoost.MVVM.ViewModels.Shared.Selection.Cards
{
    public class SelectionResultViewModel(SelectionActionCardViewModelBase action) : ObservableObject
    {
        public string Title { get; set; } = action.Title;
        public PackIconKind Icon { get; set; } = action.Icon;

        private string? _status;
        public string? Status { get => _status; set => Set(ref _status, value); }

        private ModuleResult? _result;
        public ModuleResult? Result { get => _result; set => Set(ref _result, value); }

        private ResultButtonState _state;
        public ResultButtonState State { get => _state; set => Set(ref _state, value); }
    }

    public enum ResultButtonState
    {
        Running,
        Result,
    }
}
