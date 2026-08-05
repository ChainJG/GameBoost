using GameBoost.Application.Diagnostics;
using GameBoost.Core;
using GameBoost.Core.Interfaces;
using GameBoost.Core.Modules;
using GameBoost.Shared.Results;
using MaterialDesignThemes.Wpf;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace GameBoost.Application.Modules
{
    /// <summary>
    /// Application-layer runtime for a single optimisation action. Owns the module
    /// reference, the refreshed module state (current value, status text, options),
    /// recommendation evaluation, and the execution pipeline (admin gate, diagnostics
    /// tracking, background execution, refresh-after-execute).
    ///
    /// This type has no dependency on WPF controls or ViewModels, so the same action
    /// instance can back the Selection UI, the Home recommendation cards, and future
    /// automation workflows without duplicating the optimisation logic.
    /// (PackIconKind is display metadata carried for consumers; it is an enum only.)
    /// </summary>
    public sealed class OptimizationAction : INotifyPropertyChanged
    {
        private static readonly TimeSpan DefaultRefreshCacheDuration = TimeSpan.FromMinutes(5);

        private readonly SemaphoreSlim _refreshLock = new(1, 1);

        private readonly IActionModule? _actionModule;
        private readonly IInputActionModule<object>? _objectInputModule;
        private readonly IInputActionModule<double>? _doubleInputModule;
        private readonly object _module;

        private OptimizationAction(
            string title,
            PackIconKind icon,
            object module,
            IActionModule? actionModule,
            IInputActionModule<object>? objectInputModule,
            IInputActionModule<double>? doubleInputModule)
        {
            Title = title;
            Icon = icon;
            _module = module;
            _actionModule = actionModule;
            _objectInputModule = objectInputModule;
            _doubleInputModule = doubleInputModule;

            Requirements = module as IRequiredModule;
            Recommendation = module as IRecommendedActionModule;
        }

        public static OptimizationAction ForModule(string title, PackIconKind icon, IActionModule module) =>
            new(title, icon, module, module, null, null);

        public static OptimizationAction ForObjectInput(string title, PackIconKind icon, IInputActionModule<object> module) =>
            new(title, icon, module, null, module, null);

        public static OptimizationAction ForDoubleInput(string title, PackIconKind icon, IInputActionModule<double> module) =>
            new(title, icon, module, null, null, module);

        #region Identity / Metadata
        public string Title { get; }
        public PackIconKind Icon { get; }

        /// <summary>Title of the feature group this action belongs to (set when the action is grouped).</summary>
        public string? FeatureTitle { get; internal set; }
        #endregion

        #region Capabilities
        public IRequiredModule? Requirements { get; }
        public IRecommendedActionModule? Recommendation { get; }

        public bool RequiresAdmin => Requirements?.Admin ?? false;
        public bool RequiresReboot => Requirements?.SystemReboot ?? false;
        public bool HasRecommendation => Recommendation is not null;
        #endregion

        #region Current / Desired State
        private object? _currentValue;

        /// <summary>The value the system currently reports for this module (set by refresh).</summary>
        public object? CurrentValue
        {
            get => _currentValue;
            private set
            {
                if (Equals(_currentValue, value))
                    return;

                _currentValue = value;
                Raise();
                RaiseRecommendationStateChanged();
            }
        }

        private object? _desiredValue;

        /// <summary>
        /// The value the user (or an automation workflow) wants to apply. Used as the
        /// input for modules that take one. Falls back to <see cref="CurrentValue"/>
        /// when evaluating the recommendation state so UI selection is reflected live.
        /// </summary>
        public object? DesiredValue
        {
            get => _desiredValue;
            set
            {
                if (Equals(_desiredValue, value))
                    return;

                _desiredValue = value;
                Raise();
                RaiseRecommendationStateChanged();
            }
        }

        /// <summary>The pending value if one is set, otherwise the system's current value.</summary>
        public object? EffectiveValue => DesiredValue ?? CurrentValue;

        private string _status = string.Empty;
        public string Status
        {
            get => _status;
            private set
            {
                if (_status == value)
                    return;

                _status = value;
                Raise();
            }
        }

        private IReadOnlyList<ActionOption>? _options;

        /// <summary>The selectable options the module reported on its last refresh, if any.</summary>
        public IReadOnlyList<ActionOption>? Options
        {
            get => _options;
            private set
            {
                _options = value;
                Raise();
            }
        }

        public ModuleResult? LastResult { get; private set; }
        public bool LastExecutionSuccessful => LastResult?.Success ?? false;
        #endregion

        #region Recommendation State
        public RecommendationPriority RecommendationPriority => Recommendation?.RecommendationPriority ?? RecommendationPriority.None;
        public object? RecommendedValue => Recommendation?.RecommendedValue;
        public string RecommendationReason => Recommendation?.RecommendationReason ?? string.Empty;

        public bool IsRecommendedState => Recommendation?.IsRecommendedValue(EffectiveValue) ?? false;

        public bool ShouldShowAsRecommendation =>
            HasRecommendation &&
            !IsRecommendedState &&
            RecommendationPriority != RecommendationPriority.None;

        private void RaiseRecommendationStateChanged()
        {
            Raise(nameof(RecommendationPriority));
            Raise(nameof(RecommendedValue));
            Raise(nameof(RecommendationReason));
            Raise(nameof(IsRecommendedState));
            Raise(nameof(ShouldShowAsRecommendation));
        }
        #endregion

        #region Refresh Cache
        private DateTimeOffset? _lastRefreshCompletedAtUtc;
        public DateTimeOffset? LastRefreshCompletedAtUtc => _lastRefreshCompletedAtUtc;

        public bool HasRefreshed => _lastRefreshCompletedAtUtc is not null;

        public bool IsRefreshStale(TimeSpan? cacheDuration = null)
        {
            if (_lastRefreshCompletedAtUtc is null)
                return true;

            var duration = cacheDuration ?? DefaultRefreshCacheDuration;

            return DateTimeOffset.UtcNow - _lastRefreshCompletedAtUtc.Value > duration;
        }
        #endregion

        #region Refresh
        public async Task RefreshStatusSafeAsync(CancellationToken token, ActionRefreshMode mode = ActionRefreshMode.UseCache, TimeSpan? cacheDuration = null)
        {
            try
            {
                if (mode == ActionRefreshMode.UseCache && !IsRefreshStale(cacheDuration))
                    return;

                await _refreshLock.WaitAsync(token);

                try
                {
                    if (mode == ActionRefreshMode.UseCache && !IsRefreshStale(cacheDuration))
                        return;

                    await RefreshAndApplyStatusAsync(token);
                }
                finally
                {
                    _refreshLock.Release();
                }
            }
            catch (OperationCanceledException)
            {
                // Ignore
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
                source: _module.GetType().Name,
                operation: _ => Task.Run(() => RefreshModuleAsync(token), token),
                token: token,
                metadata: CreateDiagnosticMetadata("RefreshStatusAsync"));

            ApplyRefreshResult(result);

            _lastRefreshCompletedAtUtc = DateTimeOffset.UtcNow;
        }

        private Task<ActionRefreshResult> RefreshModuleAsync(CancellationToken token)
        {
            if (_actionModule is not null)
                return _actionModule.RefreshStatusAsync(token);

            if (_objectInputModule is not null)
                return _objectInputModule.RefreshStatusAsync(token);

            if (_doubleInputModule is not null)
                return _doubleInputModule.RefreshStatusAsync(token);

            throw new InvalidOperationException($"{Title} does not have a module");
        }

        private void ApplyRefreshResult(ActionRefreshResult refreshResult)
        {
            if (!string.IsNullOrWhiteSpace(refreshResult.StatusText))
                Status = refreshResult.StatusText;

            CurrentValue = refreshResult.Value;

            if (refreshResult.Options is not null)
                Options = refreshResult.Options;
        }
        #endregion

        #region Execution
        public async Task<ModuleResult> ExecuteSafeAsync(CancellationToken token)
        {
            return await ExecuteWithPipelineAsync(
                execute: ExecuteModuleAsync,
                operationType: DiagnosticOperationType.ModuleExecute,
                token: token);
        }

        public async Task<ModuleResult> ExecuteRecommendedAsync(CancellationToken token)
        {
            if (Recommendation is null)
                return SetLastResult(ModuleResult.Failed("No recommendation module available"));

            return await ExecuteWithPipelineAsync(
                execute: innerToken => Recommendation.ExecuteRecommendedAsync(innerToken),
                operationType: DiagnosticOperationType.ModuleRecommendedExecute,
                token: token);
        }

        private Task<ModuleResult> ExecuteModuleAsync(CancellationToken token)
        {
            if (_actionModule is not null)
                return _actionModule.ExecuteAsync(token);

            if (_objectInputModule is not null)
            {
                if (DesiredValue is null)
                    throw new InvalidOperationException($"{Title} requires a selected option");

                return _objectInputModule.ExecuteAsync(DesiredValue, token);
            }

            if (_doubleInputModule is not null)
                return _doubleInputModule.ExecuteAsync(Convert.ToDouble(DesiredValue ?? 0d), token);

            throw new InvalidOperationException($"{Title} does not have a module");
        }

        private async Task<ModuleResult> ExecuteWithPipelineAsync(
            Func<CancellationToken, Task<ModuleResult>> execute,
            DiagnosticOperationType operationType,
            CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                if (RequiresAdmin && !GameBoostServices.IsAdministrator())
                    return SetLastResult(ModuleResult.Failed("Requires Administrator Privileges"));

                var result = await GameBoostContext.Diagnostic.TrackAsync(
                    category: "Module",
                    operationType: operationType,
                    name: Title,
                    source: _module.GetType().Name,
                    operation: innerToken => Task.Run(() => execute(innerToken), innerToken),
                    token: token,
                    metadata: CreateDiagnosticMetadata(
                        operationType == DiagnosticOperationType.ModuleRecommendedExecute
                            ? "ExecuteRecommendedAsync"
                            : "ExecuteAsync"));

                SetLastResult(result);

                token.ThrowIfCancellationRequested();

                await RefreshAndApplyStatusAsync(token);

                return result;
            }
            catch (OperationCanceledException)
            {
                return SetLastResult(ModuleResult.Failed("Operation Canceled"));
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error Executing {Title}: {ex.Message}");
#endif
                return SetLastResult(ModuleResult.Failed(ex.Message));
            }
        }

        private ModuleResult SetLastResult(ModuleResult result)
        {
            LastResult = result;
            Raise(nameof(LastResult));
            Raise(nameof(LastExecutionSuccessful));
            return result;
        }
        #endregion

        private IReadOnlyDictionary<string, string?> CreateDiagnosticMetadata(
            string methodName)
        {
            return new Dictionary<string, string?>
            {
                ["Method"] = methodName,
                ["Feature"] = FeatureTitle,
                ["Action"] = Title,
                ["ModuleType"] = _module.GetType().Name,
                ["RequiresAdmin"] = RequiresAdmin.ToString(),
                ["RequiresReboot"] = RequiresReboot.ToString(),
                ["CurrentValue"] = CurrentValue?.ToString(),
                ["RecommendedValue"] = RecommendedValue?.ToString(),
                ["RecommendationPriority"] = RecommendationPriority.ToString(),
                ["IsRecommendedState"] = IsRecommendedState.ToString()
            };
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void Raise([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
