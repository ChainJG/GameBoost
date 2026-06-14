using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using Microsoft.Win32;

namespace GameBoost.Features.Modules.WindowsModules.PowerOptions
{
    public sealed class HibernateModule : ToggleShellCommandModuleBase
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

        public override string EnabledCommand => "powercfg /hibernate on";
        public override string DisableCommand => "powercfg /hibernate off";

        private static RegistryEditInfo HibernateRegistryInfo => new()
        {
            Hive = RegistryHive.LocalMachine,
            Path = @"SYSTEM\CurrentControlSet\Control\Power",
            Key = "HibernateEnabled",
            EnabledValue = 1,
        };

        public override ToggleType GetStatus()
        {
            var result = RegistryHelper.GetValue(HibernateRegistryInfo);

            var match = RegistryHelper.RegistryValuesMatch(result.Value, HibernateRegistryInfo.EnabledValue);

            return match ? ToggleType.Enabled : ToggleType.Disabled;
        }
    }
}
