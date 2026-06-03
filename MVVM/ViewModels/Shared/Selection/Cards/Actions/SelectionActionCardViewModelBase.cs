using GameBoost.Core.Interfaces;
using GameBoost.MVVM.Core;
using GameBoost.Shared.Results;
using MaterialDesignThemes.Wpf;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Linq;

namespace GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions
{
    public abstract class SelectionActionCardViewModelBase : ObservableObject, ISelectionButton
    {
        public required string Title { get; init; }
        public required PackIconKind Icon { get; init; }

        protected virtual IRecommendedActionModule? RecommendationModule => null;

        private object? _currentValue;
        public object? CurrentValue
        {
            get => _currentValue;
            private set => Set(ref _currentValue, value);
        }

        public bool HasRecommendation =>
            RecommendationModule is not null;

        public object? RecommendedValue =>
            RecommendationModule?.RecommendedValue;

        public string RecommendationToolTip =>
            RecommendationModule?.RecommendationReason ?? "No recommendation available.";

        public bool IsRecommendedState =>
            RecommendationModule?.IsRecommendedValue(CurrentValue) ?? false;

        protected void SetCurrentValue(object? value)
        {
            if (!Set(ref _currentValue, value, nameof(CurrentValue)))
                return;

            OnPropertyChanged(nameof(HasRecommendation));
            OnPropertyChanged(nameof(RecommendedValue));
            OnPropertyChanged(nameof(RecommendationToolTip));
            OnPropertyChanged(nameof(IsRecommendedState));
        }

        public bool RequireReboot { get; init; }
        public bool RequireAdmin { get; init; }


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

        public async Task<ModuleResult> ExecuteSafeAsync(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                LastResult = await ExecuteAsync(token);

                await RefreshAndApplyStatusAsync(token);

                return LastResult;
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error Executing {Title}: {ex.Message}");
#endif
                LastResult = ModuleResult.Failed(ex.Message);

                return LastResult;
            }

        }

        private async Task RefreshAndApplyStatusAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var refreshResult = await RefreshStatusAsync(token);

            ApplyRefreshResult(refreshResult);
        }

        protected virtual void ApplyRefreshResult(ActionRefreshResult refreshResult)
        {
            if (!string.IsNullOrWhiteSpace(refreshResult.StatusText))
                Status = refreshResult.StatusText;

            SetCurrentValue(refreshResult.Value);
        }

        protected abstract Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token);

        protected abstract Task<ModuleResult> ExecuteAsync(CancellationToken token);
    }
}
