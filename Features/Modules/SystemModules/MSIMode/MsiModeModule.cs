using GameBoost.Core.Interfaces;
using GameBoost.Infrastructure.MSI;
using GameBoost.Infrastructure.Registry;
using GameBoost.Shared.Results;
using Microsoft.Win32;
using System.Diagnostics;

namespace GameBoost.Features.Modules.SystemModules.MSIMode
{
    public sealed class MsiModeModule(MsiModeDeviceInfo device) : IActionModule, IRecommendedActionModule, IRequiredModule
    {
        private const string MsiSupportedValueName = "MSISupported";
        private readonly MsiModeDeviceInfo _device = device;

        public string Name => _device.DisplayName;

        #region IRequiredModule
        public bool Admin => true;
        public bool SystemReboot => true;
        #endregion

        #region IRecommendedActionModule
        public RecommendationPriority RecommendationPriority => RecommendationPriority.Low;
        public object? RecommendedValue => ToggleType.Enabled;
        public string RecommendationReason =>
            $"MSI Mode is recommended for {_device.DisplayName} because it can reduce interrupt overhead and improve device responsiveness. A restart is required for the change to fully apply";
        public bool IsRecommendedValue(object? currentValue) =>
            currentValue is ToggleType.Enabled;
        #endregion

        public async Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var status = GetMsiModeStatus();

            return ActionRefreshResult.ValueOnly(status, status.ToString());
        }

        public Task<ModuleResult> ExecuteRecommendedAsync(CancellationToken token) => ExecuteAsync(token);
        public async Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var currentStatus = GetMsiModeStatus();
                var targetStatus = currentStatus == ToggleType.Enabled ? ToggleType.Disabled : ToggleType.Enabled;

                var edit = CreateMsiSupportedEdit();
                var value = targetStatus == ToggleType.Enabled ? 1 : 0;

                var result = RegistryHelper.SetValue(edit, value);

                if (!result.Success)
                    return ModuleResult.Failed(result.Message);

                return ModuleResult.Successful($"Successfully {targetStatus} {_device.Name} MSI Mode");
            }
            catch (OperationCanceledException)
            {
                return ModuleResult.Failed("MSI Mode change was cancelled");
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Failed to enable MSI Mode for {_device.Name}: {ex.Message}");
#endif
                return ModuleResult.Failed(ex.Message);
            }
        }



        private ToggleType GetMsiModeStatus()
        {
            var result = RegistryHelper.GetValue(
                CreateMsiSupportedEdit());

            if (!result.Success)
                return ToggleType.Disabled;

            return result.Value switch
            {
                int intValue => intValue == 1
                    ? ToggleType.Enabled
                    : ToggleType.Disabled,

                string stringValue when stringValue == "1" => ToggleType.Enabled,
                string stringValue when stringValue == "0" => ToggleType.Disabled,

                _ => ToggleType.Unknown
            };
        }

        private RegistryEditInfo CreateMsiSupportedEdit()
        {
            return new RegistryEditInfo
            {
                Hive = RegistryHive.LocalMachine,
                Path = GetMsiPropertiesRegistryPath(),
                Key = MsiSupportedValueName
            };
        }

        private string GetMsiPropertiesRegistryPath()
        {
            return
                $@"SYSTEM\CurrentControlSet\Enum\{_device.PnpDeviceId}\Device Parameters\Interrupt Management\MessageSignaledInterruptProperties";
        }
    }
}
