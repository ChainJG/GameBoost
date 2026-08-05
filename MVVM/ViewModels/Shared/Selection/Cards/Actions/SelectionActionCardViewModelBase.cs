using GameBoost.Application.Modules;
using GameBoost.MVVM.Core;
using GameBoost.Shared.Results;
using MaterialDesignThemes.Wpf;
using System.ComponentModel;

namespace GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions
{
    /// <summary>
    /// Presentation adapter over an <see cref="OptimizationAction"/>. Owns only UI
    /// concerns (selection state, tooltips, parent feature wiring) and forwards the
    /// action's state changes as bindable property notifications. The optimisation
    /// behaviour itself lives on the action and its module.
    /// </summary>
    public abstract class SelectionActionCardViewModelBase : ObservableObject
    {
        protected SelectionActionCardViewModelBase(OptimizationAction action)
        {
            Action = action;
            Action.PropertyChanged += OnActionPropertyChanged;
        }

        public OptimizationAction Action { get; }

        public string Title => Action.Title;
        public PackIconKind Icon => Action.Icon;

        public string? InfoToolTip { get; set; }
        public PackIconKind InfoIcon { get; init; } = PackIconKind.HelpRhombus;

        #region Forwarded Action State
        public bool RequiresAdmin => Action.RequiresAdmin;
        public bool RequiresReboot => Action.RequiresReboot;

        public object? CurrentValue => Action.CurrentValue;
        public string Status => Action.Status;

        public RecommendationPriority RecommendationPriority => Action.RecommendationPriority;
        public object? RecommendedValue => Action.RecommendedValue;
        public string RecommendationToolTip => Action.RecommendationReason;
        public bool IsRecommendedState => Action.IsRecommendedState;
        public bool HasRecommendation => Action.HasRecommendation;
        #endregion

        #region Selection State
        private bool _isChecked = false;
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (!Set(ref _isChecked, value))
                    return;

                Parent?.OnActionSelectionChanged(this);
            }
        }

        private SelectionFeatureViewModel? _parent;
        public SelectionFeatureViewModel? Parent
        {
            get => _parent;
            internal set
            {
                _parent = value;
                Action.FeatureTitle = value?.Title;
            }
        }
        #endregion

        #region Execution
        public async Task<ModuleResult> ExecuteSafeAsync(CancellationToken token)
        {
            var result = await Action.ExecuteSafeAsync(token);

            if (result.Success)
                IsChecked = false;

            return result;
        }
        #endregion

        private void OnActionPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(OptimizationAction.Status):
                    OnPropertyChanged(nameof(Status));
                    break;

                case nameof(OptimizationAction.CurrentValue):
                    OnPropertyChanged(nameof(CurrentValue));
                    OnActionValueChanged();
                    break;

                case nameof(OptimizationAction.Options):
                    OnActionOptionsChanged();
                    break;

                case nameof(OptimizationAction.RecommendationPriority):
                    OnPropertyChanged(nameof(RecommendationPriority));
                    break;

                case nameof(OptimizationAction.RecommendedValue):
                    OnPropertyChanged(nameof(RecommendedValue));
                    break;

                case nameof(OptimizationAction.RecommendationReason):
                    OnPropertyChanged(nameof(RecommendationToolTip));
                    break;

                case nameof(OptimizationAction.IsRecommendedState):
                    OnPropertyChanged(nameof(IsRecommendedState));
                    break;
            }
        }

        /// <summary>Called when the action's refreshed <see cref="OptimizationAction.CurrentValue"/> changes.</summary>
        protected virtual void OnActionValueChanged() { }

        /// <summary>Called when the action's refreshed <see cref="OptimizationAction.Options"/> change.</summary>
        protected virtual void OnActionOptionsChanged() { }
    }
}
