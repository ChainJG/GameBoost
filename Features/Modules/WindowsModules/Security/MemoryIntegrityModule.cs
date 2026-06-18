using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using GameBoost.Shared.Helpers;
using GameBoost.Shared.Results;
using Microsoft.Win32;

namespace GameBoost.Features.Modules.WindowsModules.Security
{
    internal class MemoryIntegrityModule : SystemTweakModuleBase
    {
        public override string Name => "Core Integrity";

        #region IRequireModule
        public override bool Admin => true;
        public override bool SystemReboot => true;
        #endregion

        #region IRecommendationModule
        public override RecommendationPriority RecommendationPriority => RecommendationPriority.High;
        public override string RecommendationReason =>
            "Disabling it can reduce virtualisation/security overhead and may improve compatibility with some drivers, anti-cheats, RGB tools, and older hardware software";
        public override object? RecommendedValue => ToggleType.Disabled;
        #endregion

        public override RegistryEditInfo[] RegistryEdits =>
        [
            new()
            {
                Hive = RegistryHive.LocalMachine,
                Path = @"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity",
                Key = "Enabled",
                Kind = RegistryValueKind.DWord,
                EnabledValue = 1,
                DisabledValue = 0
            }
        ];


        public async override Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var currentStatus = GetToggleStatus();
                var targetStatus = GetTargetStatus(currentStatus);

                if (WindowsSettingsHelper.GetTamperProtectionStatus() == ToggleType.Enabled)
                {
                    WindowsSettingsHelper.TryOpenMemoryIntegritySettings();

                    return ModuleResult.Failed($"Failed Tamper Protection is enabled");
                }

                var registry = RegistryEdits.FirstOrDefault() ?? throw new Exception($"{Name} Registry edit not found");
                var value = GetRegistryValue(registry, targetStatus) ?? throw new Exception($"{Name} Value was null for registry Key");

                var result = RegistryHelper.SetValue(registry, value);

                if (!result.Success)
                    return ModuleResult.Failed($"Failed to {targetStatus} {Name}");

                return ModuleResult.Successful($"Successfully {targetStatus} {Name}");

            }
            catch (Exception ex)
            {
                return ModuleResult.Failed($"{ex.Message}");
            }
    }
}
}
