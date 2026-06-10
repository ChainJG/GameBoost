using GameBoost.Features.Modules.Base;
using GameBoost.Shared.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameBoost.Features.Modules.WindowsModules.Security
{
    public sealed class FirewallModule : ShellCommandModuleBase
    {
        public override string Name => "Firewall";

        public override bool Admin => true;

        #region Recommendation
        public override RecommendationPriority RecommendationPriority => RecommendationPriority.High;
        public override object? RecommendedValue => ToggleType.Enabled;
        public override string RecommendationReason =>
            "Windows Firewall is recommended to be enabled because it helps block unwanted inbound network traffic and protects the system from unsafe network access.";
        #endregion

        #region Commands
        public override ShellType Shell => ShellType.PowerShell;

        private const string ReadFirewallStatusCommand =
            "$profiles = Get-NetFirewallProfile; " +
            "$disabledProfiles = $profiles | Where-Object { $_.Enabled -eq $false }; " +
            "if ($disabledProfiles.Count -gt 0) { 'Disabled' } " +
            "elseif ($profiles.Count -gt 0) { 'Enabled' } " +
            "else { 'Unknown' }";

        public override string Command =>
            "Set-NetFirewallProfile -Profile Domain,Private,Public -Enabled True";
        #endregion

        public override async Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            var status = await ReadToggleStatusAsync(
                Shell,
                ReadFirewallStatusCommand,
                token);

            return ToggleStatusResult(status);
        }
        public override async Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            var currentStatus = await ReadToggleStatusAsync(Shell, ReadFirewallStatusCommand, token);

            if (currentStatus == ToggleType.Enabled)
                return ModuleResult.Successful($"{Name} is already enabled");

            var result = await RunCommandAsync(Shell, Command, token);

            if (!result.Success)
                return ModuleResult.Failed($"{result.Message}");

            return ModuleResult.Successful($"{Name} was enabled successfully");
        }
    }
}
