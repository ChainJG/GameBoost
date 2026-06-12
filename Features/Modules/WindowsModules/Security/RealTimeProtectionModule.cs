using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using GameBoost.Shared.Helpers;
using GameBoost.Shared.Results;
using Microsoft.Win32;
using System.ComponentModel.DataAnnotations;
using System.Management;

namespace GameBoost.Features.Modules.WindowsModules.Security
{
    public sealed class RealTimeProtectionModule : ShellCommandModuleBase
    {
        public override string Name => "Real Time Protection";

        public override ShellType Shell => ShellType.PowerShell;
        public override bool Admin => true;

        #region Recommendation
        public override RecommendationPriority RecommendationPriority => RecommendationPriority.High;
        public override object? RecommendedValue => ToggleType.Enabled;
        public override string RecommendationReason =>
            "Real-time protection is recommended to be enabled because it helps block malware and unsafe files while the system is running";
        #endregion

        public override string Command => "Set-MpPreference -DisableRealtimeMonitoring $false";

        private static RegistryEditInfo TamperProtectionEdits => new()
        {
            Hive = RegistryHive.LocalMachine,
            Path = @"SOFTWARE\Microsoft\Windows Defender\Features",
            Key = "TamperProtection",
            Kind = RegistryValueKind.DWord,
            EnabledValue = 5,
            DisabledValue = 4
        };
        private static RegistryEditInfo RealTimeProtectionEdits => new()
        {
            Hive = RegistryHive.LocalMachine,
            Path = @"SOFTWARE\Microsoft\Windows Defender\Real-Time Protection",
            Key = "DisableRealtimeMonitoring",
            Kind = RegistryValueKind.DWord,
        };


        public override async Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token) =>
             GetStatusResult(GetRealTimeProtectionStatus());
        public override async Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            if (GetRealTimeProtectionStatus() == ToggleType.Enabled)
                return ModuleResult.Successful($"{Name} is already enabled");

            if (GetTamperProtectionStatus() == ToggleType.Enabled)
            {
                WindowsSettingsHelper.TryOpenRealTimeProtectionSettings();

                return ModuleResult.Failed(
                    $"Failed because Tamper Protection is enabled");
            }

            var result = await RunCommandAsync(Shell, Command, token);

            if (!result.Success)
            {
                WindowsSettingsHelper.TryOpenRealTimeProtectionSettings();

                return ModuleResult.Failed(
                    $"{result.Message}");
            }

            return ModuleResult.Successful($"{Name} was enabled successfully");
        }


        private static ToggleType GetRealTimeProtectionStatus()
        {
            var result = RegistryHelper.GetValue(RealTimeProtectionEdits);

            if (result.Value is null)
                return ToggleType.Enabled;

            return result.Value switch
            {
                int value when value == 0 => ToggleType.Enabled,
                int value when value == 1 => ToggleType.Disabled,

                string value when value == "0" => ToggleType.Enabled,
                string value when value == "1" => ToggleType.Disabled,

                _ => ToggleType.Unknown
            };
        }
        private static ToggleType GetTamperProtectionStatus()
        {
            var result = RegistryHelper.GetValue(TamperProtectionEdits);

            if (!result.Success)
                return ToggleType.Unknown;

            return result.Value switch
            {
                int value when value == 5 => ToggleType.Enabled,
                int value when value == 4 => ToggleType.Disabled,

                string value when value == "5" => ToggleType.Enabled,
                string value when value == "4" => ToggleType.Disabled,

                _ => ToggleType.Unknown
            };
        }
    }
}
