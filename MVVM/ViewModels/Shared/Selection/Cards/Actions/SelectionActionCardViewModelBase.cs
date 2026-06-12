using GameBoost.Application;
using GameBoost.Application.Diagnostics;
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

            var result = await GameBoostContext.Diagnostic.TrackAsync(
                category: "Module",
                operationType: DiagnosticOperationType.ModuleRefresh,
                name: Title,
                source: GetType().Name,
                operation: _ => Task.Run(() => RefreshStatusAsync(token), token),
                token: token,
                metadata: CreateDiagnosticMetadata("RefreshStatusAsync"));

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
            return await ExecuteWithPipeLineAsync(
                execute: innerToken => ExecuteAsync(innerToken),
                operationType: DiagnosticOperationType.ModuleExecute,
                token: token);
        }

        public async Task<ModuleResult> ExecuteRecommendedAsync(CancellationToken token)
        {
            if (RecommendationModule is null)
                return LastResult = ModuleResult.Failed("No recommendation module available");

            return await ExecuteWithPipeLineAsync(
                execute: innerToken => RecommendationModule.ExecuteRecommendedAsync(innerToken),
                operationType: DiagnosticOperationType.ModuleRecommendedExecute,
                token: token);
        }

        private async Task<ModuleResult> ExecuteWithPipeLineAsync(
            Func<CancellationToken, Task<ModuleResult>> execute,
            DiagnosticOperationType operationType,
            CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                if (RequiresAdmin && !GameBoostServices.IsAdministrator())
                    return LastResult = ModuleResult.Failed("Requires Administrator Privileges");

                LastResult = await GameBoostContext.Diagnostic.TrackAsync(
                    category: "Module",
                    operationType: operationType,
                    name: Title,
                    source: GetType().Name,
                    operation: execute,
                    token: token,
                    metadata: CreateDiagnosticMetadata(
                        operationType == DiagnosticOperationType.ModuleRecommendedExecute
                            ? "ExecuteRecommendedAsync"
                            : "ExecuteAsync"));

                token.ThrowIfCancellationRequested();

                await RefreshAndApplyStatusAsync(token);

                if (LastResult.Success)
                    IsChecked = false;

                return LastResult;
            }
            catch (OperationCanceledException)
            {
                return LastResult = ModuleResult.Failed("Operation Canceled");
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error Executing {Title}: {ex.Message}");
#endif
                return LastResult = ModuleResult.Failed(ex.Message);
            }
        }
        #endregion

        #region Abstract Methods
        protected abstract Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token);
        protected abstract Task<ModuleResult> ExecuteAsync(CancellationToken token);
        #endregion

        private IReadOnlyDictionary<string, string?> CreateDiagnosticMetadata(
            string methodName)
        {
            return new Dictionary<string, string?>
            {
                ["Method"] = methodName,
                ["Feature"] = Parent?.Title,
                ["Action"] = Title,
                ["ActionCardType"] = GetType().Name,
                ["RequiresAdmin"] = RequiresAdmin.ToString(),
                ["RequiresReboot"] = RequiresReboot.ToString(),
                ["CurrentValue"] = CurrentValue?.ToString(),
                ["RecommendedValue"] = RecommendedValue?.ToString(),
                ["RecommendationPriority"] = RecommendationPriority.ToString(),
                ["IsRecommendedState"] = IsRecommendedState.ToString()
            };
        }
    }
}
