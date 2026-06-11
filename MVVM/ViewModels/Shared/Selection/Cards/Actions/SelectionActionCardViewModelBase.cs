using GameBoost.Core;
using GameBoost.Core.Interfaces;
using GameBoost.MVVM.Core;
using GameBoost.Shared.Results;
using MaterialDesignThemes.Wpf;
using System.Diagnostics;

namespace GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions
{
    public abstract class SelectionActionCardViewModelBase : ObservableObject, ISelectionButton
    {
        public required string Title { get; init; }
        public required PackIconKind Icon { get; init; }
        public string InfoToolTip { get; init; } = string.Empty;
        public PackIconKind InfoIcon { get; init; } = PackIconKind.HelpRhombus;

        #region Required Module
        protected virtual IRequiredModule? RquiredModule => null;
        public bool RequiresAdmin => RquiredModule?.Admin ?? false;
        public bool RequiresReboot => RquiredModule?.SystemReboot ?? false;
        #endregion

        #region Recommendation Module
        protected virtual IRecommendedActionModule? RecommendationModule => null;

        private object? _currentValue;
        public object? CurrentValue
        {
            get => _currentValue;
            private set => Set(ref _currentValue, value);
        }

        public RecommendationPriority RecommendationPriority => RecommendationModule?.RecommendationPriority ?? RecommendationPriority.None;
        public object? RecommendedValue => RecommendationModule?.RecommendedValue;
        public string RecommendationToolTip => RecommendationModule?.RecommendationReason ?? string.Empty;

        protected void SetCurrentValue(object? value)
        {
            if (!Set(ref _currentValue, value, nameof(CurrentValue)))
                return;

            OnPropertyChanged(nameof(RecommendationPriority));
            OnPropertyChanged(nameof(RecommendedValue));
            OnPropertyChanged(nameof(RecommendationToolTip));

            OnPropertyChanged(nameof(ShouldShowAsHomeRecommendation));
            OnPropertyChanged(nameof(HasRecommendation));
            OnPropertyChanged(nameof(IsRecommendedState));
        }

        public bool IsRecommendedState => RecommendationModule?.IsRecommendedValue(CurrentValue) ?? false;
        public bool HasRecommendation => RecommendationModule is not null;
        public bool ShouldShowAsHomeRecommendation => HasRecommendation && !IsRecommendedState && RecommendationPriority != RecommendationPriority.None;
        #endregion

        private string _status = string.Empty;
        public string Status { get => _status; set => Set(ref _status, value); }

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
        public SelectionFeatureViewModel? Parent { get; internal set; }

        private ModuleResult? LastResult;


        #region Refresh Methods
        public async Task RefreshStatusSafeAsync(CancellationToken token)
        {
            try
            {
                await RefreshAndApplyStatusAsync(token);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error Refreshing {Title} Status: {ex.Message}");
#endif
            }
        }
        private async Task RefreshAndApplyStatusAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var result = await Task.Run(() => RefreshStatusAsync(token), token);

            ApplyRefreshResult(result);
        }

        protected virtual void ApplyRefreshResult(ActionRefreshResult refreshResult)
        {
            if (!string.IsNullOrWhiteSpace(refreshResult.StatusText))
                Status = refreshResult.StatusText;

            SetCurrentValue(refreshResult.Value);
        }
        #endregion

        #region Execution Methods
        public async Task<ModuleResult> ExecuteSafeAsync(CancellationToken token)
        {
            return await ExecuteWithPipeLineAsync(token => ExecuteAsync(token), token);
        }

        public async Task<ModuleResult> ExecuteRecommendedAsync(CancellationToken token)
        {
            if (RecommendationModule is null)
                return ModuleResult.Failed("No recommendation module available");


            return await ExecuteWithPipeLineAsync(token => RecommendationModule.ExecuteRecommendedAsync(token), token);
        }

        private async Task<ModuleResult> ExecuteWithPipeLineAsync(Func<CancellationToken, Task<ModuleResult>> execute, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                if (RequiresAdmin && !GameBoostServices.IsAdministrator())
                    return LastResult = ModuleResult.Failed("Requires Administrator Privileges");

                LastResult = await execute(token);

                token.ThrowIfCancellationRequested();

                // Refresh status
                await RefreshAndApplyStatusAsync(token);

                // Uncheck if the action was successful
                if (LastResult.Success)
                    IsChecked = false;

                return LastResult;
            }
            catch (OperationCanceledException)
            {
                 return LastResult = ModuleResult.Failed("Task Canceled");
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error Executing {Title}: {ex.Message}");
#endif
                return ModuleResult.Failed(ex.Message);
            }
        }
        #endregion

        #region Abstract Methods
        protected abstract Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token);
        protected abstract Task<ModuleResult> ExecuteAsync(CancellationToken token);
        #endregion
    }
}
