using GameBoost.Features.Modules.Base;
using GameBoost.Shared.Helpers;
using GameBoost.Shared.Results;

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

        private const string ReadTamperProtectionStatusCommand =
            "$status = Get-MpComputerStatus; " +
            "if ($status.IsTamperProtected -eq $true) { 'Enabled' } " +
            "elseif ($status.IsTamperProtected -eq $false) { 'Disabled' } " +
            "else { 'Unknown' }";

        private const string ReadRealTimeProtectionStatusCommand =
            "$status = Get-MpComputerStatus; " +
            "if ($status.RealTimeProtectionEnabled -eq $true) { 'Enabled' } " +
            "elseif ($status.RealTimeProtectionEnabled -eq $false) { 'Disabled' } " +
            "else { 'Unknown' }";
        public override string Command => "Set-MpPreference -DisableRealtimeMonitoring $false";


        public override async Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            var status = await ReadToggleStatusAsync(Shell, ReadRealTimeProtectionStatusCommand, token);

            return GetStatusResult(status);
        }
        public override async Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            var currentStatus = await ReadToggleStatusAsync(Shell, ReadRealTimeProtectionStatusCommand, token);

            if (currentStatus == ToggleType.Enabled)
                return ModuleResult.Successful($"{Name} is already enabled");

            var tamperProtectionStatus = await ReadTamperProtectionStatusAsync(token);

            if (tamperProtectionStatus == ToggleType.Enabled)
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


        private Task<ToggleType> ReadTamperProtectionStatusAsync(CancellationToken token)
        {
            return ReadToggleStatusAsync(Shell,
                ReadTamperProtectionStatusCommand,
                token);
        }
    }
}
