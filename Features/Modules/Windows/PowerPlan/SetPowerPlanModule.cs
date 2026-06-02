using GameBoost.Core.Interfaces;
using GameBoost.Infrastructure.Shell;
using GameBoost.MVVM.ViewModels.Shared.Selection.Actions.Misc;
using GameBoost.Shared.Results;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace GameBoost.Features.Modules.Windows.PowerPlan
{
    public sealed class SetPowerPlanModule : IInputActionModule<object>
    {
        public string Name => "Set Power Plan";

        private static readonly Regex PowerPlanRegex = new(
            @"Power Scheme GUID:\s*(?<guid>[a-fA-F0-9\-]+)\s*\((?<name>.*?)\)\s*(?<active>\*)?",
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public async Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            return await Task.FromResult(ActionRefreshResult.OptionsOnly(await GetOptionsAsync(token)));
        }

        public async Task<ModuleResult> ExecuteAsync(object input, CancellationToken token)
        {
            var plan = GetPowerPlan(input);

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

        public async Task<IReadOnlyList<ActionOptionViewModel<object>>> GetOptionsAsync(
            CancellationToken token)
        {
            var plans = await GetInstalledPowerPlansAsync(token);

            return plans
                .Select(plan => new ActionOptionViewModel<object>
                {
                    DisplayText = plan.IsActive
                        ? $"{plan.Name} • Active"
                        : plan.Name,

                    Value = plan,
                    IsDefaultSelected = plan.IsActive,
                    Description = plan.Guid
                })
                .ToList();
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
    }
}