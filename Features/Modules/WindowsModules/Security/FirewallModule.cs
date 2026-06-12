using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using GameBoost.Shared.Results;

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

        private const string FirewallPolicyPath = @"SYSTEM\CurrentControlSet\Services\SharedAccess\Parameters\FirewallPolicy";
        private static IReadOnlyList<RegistryEditInfo> FirewallProfileEdits =>
        [
            new() 
            {
                Hive = Microsoft.Win32.RegistryHive.LocalMachine,
                Path = @$"{FirewallPolicyPath}\DomainProfile",
                Key = "EnableFirewall",
                Kind = Microsoft.Win32.RegistryValueKind.DWord,
                EnabledValue = 1
            },
            new()
            {
                Hive = Microsoft.Win32.RegistryHive.LocalMachine,
                Path = @$"{FirewallPolicyPath}\StandardProfile",
                Key = "EnableFirewall",
                Kind = Microsoft.Win32.RegistryValueKind.DWord,
                EnabledValue = 1
            },
            new()
            {
                Hive = Microsoft.Win32.RegistryHive.LocalMachine,
                Path = @$"{FirewallPolicyPath}\PublicProfile",
                Key = "EnableFirewall",
                Kind = Microsoft.Win32.RegistryValueKind.DWord,
                EnabledValue = 1
            },
        ];

        #region Commands
        public override ShellType Shell => ShellType.PowerShell;

        public override string Command =>
            "Set-NetFirewallProfile -Profile Domain,Private,Public -Enabled True";
        #endregion

        public override Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var status = GetFirewallStatus();

            return Task.FromResult(GetStatusResult(status));
        }


        public override async Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var currentStatus = GetFirewallStatus();

            if (currentStatus == ToggleType.Enabled)
                return ModuleResult.Successful($"{Name} is already enabled");

            var result = await RunCommandAsync(Shell, Command, token);

            if (!result.Success)
                return ModuleResult.Failed($"{result.Message}");

            return ModuleResult.Successful($"{Name} was enabled successfully");
        }

        private static ToggleType GetFirewallStatus()
        {
            return RegistryHelper.GetGroupedEnabledStatus(
                FirewallProfileEdits);
        }
    }
}
