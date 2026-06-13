using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using GameBoost.Shared.Results;
using Microsoft.Win32;

namespace GameBoost.Features.Modules.WindowsModules.PowerPerformance
{
    public sealed class HibernateModule : ShellCommandModuleBase
    {
        public override string Name => "Hibernate";

        public override bool Admin => true;

        #region Recommendation
        public override RecommendationPriority RecommendationPriority => RecommendationPriority.Medium;
        public override object? RecommendedValue => ToggleType.Disabled;
        public override string RecommendationReason =>
             "Hibernate is recommended to be disabled on gaming-focused desktop systems because it can free storage used by the hibernation file and reduce unnecessary power-state behaviour.";
        #endregion

        public override ShellType Shell => ShellType.Cmd;
        public override string Command => "";

        private const string EnabledCommnad = "powercfg /hibernate on";
        private const string DisabledCommand = "powercfg /hibernate off";

        private static RegistryEditInfo HibernateRegistryInfo => new()
        {
            Hive = RegistryHive.LocalMachine,
            Path = @"SYSTEM\CurrentControlSet\Control\Power",
            Key = "HibernateEnabled",
            EnabledValue = 1,
        };


        public override async Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token) =>
            GetStatusResult(GetHibernateState());

        public override async Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            var status = GetHibernateState();
            string command = status == ToggleType.Enabled ? DisabledCommand : EnabledCommnad;

            var result = await RunCommandAsync(Shell, command, token);

            if (!result.Success)
                return ModuleResult.Failed(result.Message);

            return status == ToggleType.Enabled
                ? ModuleResult.Successful("Successfully Disabled Hibernate")
                : ModuleResult.Successful("Successfully Enabled Hibernate");

        }

        private static ToggleType GetHibernateState()
        {
            var result = RegistryHelper.GetValue(HibernateRegistryInfo);

            var match = RegistryHelper.RegistryValuesMatch(result.Value, HibernateRegistryInfo.EnabledValue);

            return match ? ToggleType.Enabled : ToggleType.Disabled;
        }
    }
}
