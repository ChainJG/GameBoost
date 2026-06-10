using GameBoost.Core.Interfaces;
using GameBoost.Infrastructure.Shell;
using GameBoost.MVVM.ViewModels.Shared.Selection.Actions.Misc;
using GameBoost.Shared.Results;
using System.Diagnostics;
using System.Numerics;
using System.Text.RegularExpressions;

namespace GameBoost.Features.Modules.WindowsModules.PowerPlan
{
    public sealed class SetPowerPlanModule : IInputActionModule<object>, IRequireModule, IRecommendedActionModule
    {
        public string Name => "Set Power Plan";

        private PowerPlanInfo? _activePowerPlan;
        private IReadOnlyList<PowerPlanInfo>? _installedPowerPlans;

        #region IRecommendedActionModule
        public RecommendationPriority RecommendationPriority => RecommendationPriority.High;
        public object? RecommendedValue => "Ultimate Performance";
        public string RecommendationReason =>
            $"{RecommendedValue} is recommended to be enabled for gaming-focused desktop systems because it reduces power-saving behaviour, keeps hardware more responsive, and can help prevent small latency or performance drops caused by aggressive power management";
        public bool IsRecommendedValue(object? currentValue)
        {
            return currentValue is PowerPlanInfo plan &&
                   !IsBlockedPowerPlan(plan);
        }
        #endregion

        #region IRequireModule
        public bool SystemReboot => false;
        public bool Admin => false;
        #endregion

        private static readonly Regex PowerPlanRegex = new(
            @"Power Scheme GUID:\s*(?<guid>[a-fA-F0-9\-]+)\s*\((?<name>.*?)\)\s*(?<active>\*)?",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public async Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            _installedPowerPlans = await GetInstalledPowerPlansAsync(token);

            _activePowerPlan = _installedPowerPlans.FirstOrDefault(plan => plan.IsActive);

            var options = _installedPowerPlans
                .Select(plan => new ActionOptionViewModel<object>
                {
                    DisplayText = plan.IsActive? $"{plan.Name} • Active" : plan.Name,
                    Value = plan,
                    IsDefaultSelected = plan.IsActive,
                    Description = plan.Guid
                })
                .ToList();

            return ActionRefreshResult.OptionsAndValue(
                options,
                _activePowerPlan,
                _activePowerPlan?.Name ?? "Unknown");
        }

        public async Task<ModuleResult> ExecuteAsync(object input, CancellationToken token)
        {
            var plan = GetPowerPlan(input);

            Debug.WriteLine($"Selected power plan: {plan?.Name}");

            if (plan is null)
                return ModuleResult.Failed("No power plan selected");

            var result = await PowerShellService.RunAsync($"powercfg /setactive {plan.Guid}");

            return result.ExitCode == 0
                ? ModuleResult.Successful("Power plan changed successfully.")
                : ModuleResult.Failed($"Failed to change power plan. Exit code: {result.ExitCode}");
        }

        private static PowerPlanInfo? GetPowerPlan(object input)
        {
            if (input is PowerPlanInfo plan)
                return plan;

            return null;
        }
        private static async Task<IReadOnlyList<PowerPlanInfo>> GetInstalledPowerPlansAsync(
            CancellationToken token)
        {
            var result = await PowerShellService.RunAsync("powercfg /list");

            return ParsePowerPlans(result.Output);
        }

        private static IReadOnlyList<PowerPlanInfo> ParsePowerPlans(string output)
        {
            var plans = new List<PowerPlanInfo>();

            foreach (Match match in PowerPlanRegex.Matches(output))
            {
                var guid = match.Groups["guid"].Value.Trim();
                var name = match.Groups["name"].Value.Trim();
                var isActive = match.Groups["active"].Success;

                if (string.IsNullOrWhiteSpace(guid) ||
                    string.IsNullOrWhiteSpace(name))
                {
                    continue;
                }

                plans.Add(new PowerPlanInfo
                {
                    Guid = guid,
                    Name = name,
                    IsActive = isActive
                });
            }

            return plans;
        }


        private static bool IsBlockedPowerPlan(PowerPlanInfo? plan)
        {
            if (plan is null)
                return false;

            return IsBlockedPowerPlanName(plan.Name);
        }

        private static bool IsBlockedPowerPlanName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return false;

            return name.Equals("Balanced", StringComparison.OrdinalIgnoreCase) ||
                   name.Equals("Power saver", StringComparison.OrdinalIgnoreCase) ||
                   name.Equals("Power Saver", StringComparison.OrdinalIgnoreCase);
        }

        public async Task<ModuleResult> ExecuteRecommendedAsync(CancellationToken token)
        {
            PowerPlanInfo? recommendedPlan = _installedPowerPlans?
                .First(plan => plan.Name.Contains(RecommendedValue as string ?? string.Empty, StringComparison.OrdinalIgnoreCase));

            if (recommendedPlan is null)
                return ModuleResult.Failed("Recommended power plan not found");

            var result = await PowerShellService.RunAsync($"powercfg /setactive {recommendedPlan?.Guid}");

            return result.ExitCode == 0
                ? ModuleResult.Successful("Power plan changed successfully.")
                : ModuleResult.Failed($"Failed to change power plan. Exit code: {result.ExitCode}");
        }
    }
}