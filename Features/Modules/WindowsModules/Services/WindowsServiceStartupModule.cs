using GameBoost.Core.Interfaces;
using GameBoost.Infrastructure.Services;
using GameBoost.Core.Modules;
using GameBoost.Shared.Results;
using System.Diagnostics;

namespace GameBoost.Features.Modules.WindowsModules.Services
{
    public sealed class WindowsServiceStartupModule(ServiceEditInfo definition) : IInputActionModule<object>, IRecommendedActionModule, IRequiredModule
    {
        private readonly ServiceEditInfo _definition = definition;

        public string Name => _definition.DisplayName;

        #region IRequiredModule
        public bool Admin => _definition.Admin;
        public bool SystemReboot => _definition.SystemReboot;
        #endregion

        public RecommendationPriority RecommendationPriority => _definition.HighRisk ? RecommendationPriority.None : RecommendationPriority.Low;
        public object? RecommendedValue => _definition.RecommendedStartupMode;
        public string? RecommendationReason => _definition?.RecommendationReason;
        public bool IsRecommendedValue(object? currentValue) => currentValue is WindowsServiceStartupMode mode && mode == _definition.RecommendedStartupMode;


        public async Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var currentMode = GetCurrentStartupMode();

            return ActionRefreshResult.OptionsAndValue(CreateOptions(currentMode), currentMode, GetDisplayName(currentMode));
        }
        public Task<ModuleResult> ExecuteRecommendedAsync(CancellationToken token) => ExecuteAsync(_definition.RecommendedStartupMode, token);
        public async Task<ModuleResult> ExecuteAsync(object input, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                if (!TryGetSelectedMode(input, out var selectedMode))
                    return ModuleResult.Failed("Invalid service startup option selected");

                var result = ServiceHelper.ChangeState(_definition, selectedMode);

                if (!result.Success)
                    return ModuleResult.Failed(result.Message);

                return ModuleResult.Successful($"{_definition.DisplayName} changed to {GetDisplayName(selectedMode)}");
            }
            catch (OperationCanceledException)
            {
                return ModuleResult.Failed($"{_definition.DisplayName} change was cancelled");
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Failed to change service {_definition.Name}: {ex.Message}");
#endif

                return ModuleResult.Failed($"Failed to change {_definition.DisplayName}");
            }
        }

        private static bool TryGetSelectedMode(object? input, out WindowsServiceStartupMode mode)
        {
            if (input is WindowsServiceStartupMode selectedMode)
            {
                mode = selectedMode;
                return true;
            }

            if (Enum.TryParse(input?.ToString(), out WindowsServiceStartupMode parsedMode))
            {
                mode = parsedMode;
                return true;
            }

            mode = default;
            return false;
        }

        private WindowsServiceStartupMode GetCurrentStartupMode() => ServiceHelper.GetServiceStartMode(_definition);

        private static string GetDisplayName(WindowsServiceStartupMode mode) =>
            mode switch
            {
                WindowsServiceStartupMode.Automatic => "Automatic",
                WindowsServiceStartupMode.Manual => "Manual",
                WindowsServiceStartupMode.Disabled => "Disabled",
                _ => mode.ToString()
            };
        private static string GetDescription(WindowsServiceStartupMode mode)
        {
            return mode switch
            {
                WindowsServiceStartupMode.Automatic => "Starts automatically with Windows",
                WindowsServiceStartupMode.Manual => "Starts only when Windows or an app needs it",
                WindowsServiceStartupMode.Disabled => "Prevents the service from starting",
                _ => string.Empty
            };
        }

        private List<ActionOption> CreateOptions(WindowsServiceStartupMode currentMode)
        {
            return [.. Enum
                .GetValues<WindowsServiceStartupMode>()
                .Select(mode => new ActionOption
                {
                    DisplayText = $"{(IsRecommendedValue(mode) ? $"{GetDisplayName(mode)} • Recommended" : GetDisplayName(mode))}",
                    Description = GetDescription(mode),
                    Value = mode,
                    IsDefaultSelected = mode == currentMode
                })];
        }
    }
}
