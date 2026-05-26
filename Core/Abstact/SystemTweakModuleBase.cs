using GameBoost.Core.Interfaces;
using GameBoost.Infrastructure.Registry;
using GameBoost.Infrastructure.Services;
using GameBoost.Shared.Results;
using System.Diagnostics;

namespace GameBoost.Core.Abstact
{
    public abstract class SystemTweakModuleBase : IActionModule
    {
        public abstract string Name { get; }

        public string Status { get; set; } = "Unknown";

        public abstract RegistryEditInfo[] RegistryEdits { get; }
        public virtual ServiceEditInfo[]? ServiceEdits { get; set; }

        protected virtual string GetFormattedStatus() => GetToggleStatus().ToString() ?? "";
        public string GetStatus() => GetFormattedStatus();

        protected ToggleType GetToggleStatus()
        {
            if (RegistryEdits.Length > 0 && GetRegistryState() == ToggleType.Enabled) return ToggleType.Enabled;
            if (ServiceEdits.Length > 0 && GetServiceState() == ToggleType.Enabled) return ToggleType.Enabled;

            return ToggleType.Disabled;
        }

        private ToggleType GetServiceState()
        {
            foreach (var service in ServiceEdits)
            {
                if (ServiceHelper.IsRunning(service))
                    return ToggleType.Enabled;
            }

            return ToggleType.Disabled;
        }
        private ToggleType GetRegistryState()
        {
            foreach (var registry in RegistryEdits)
            {
                var value = RegistryHelper.GetValue(registry);

                // Handle deletion case
                if (registry.DisabledValue.Equals(-1) && value != null)
                    return ToggleType.Enabled;

                // Handle missing value case
                if (value == null && !registry.DisabledValue.Equals(-1))
                    return ToggleType.Enabled;

                // Handle enabled value case
                if (value != null && value.Equals(registry.EnabledValue))
                    return ToggleType.Enabled;
            }

            return ToggleType.Disabled;
        }

        public async Task<ModuleResult> ExecuteAsync()
        {
            var shareResult = new ModuleShareResult { Success = true };

            try
            {
                await ApplyRegistryChanges(shareResult);
                await ApplyServiceChanges(shareResult);

                if (shareResult.Errors.Count > 0)
                {
                    return ModuleResult.Failed(string.Join(Environment.NewLine, shareResult.Errors));
                }

                return ModuleResult.Successful($"Successfully {Status} {Name}");
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error in ExecuteAsync: {ex.Message}");
#endif
                return ModuleResult.Failed(ex.Message);
            }
        }

        private async Task ApplyRegistryChanges(ModuleShareResult shareResult)
        {
            var currentDtate = GetServiceState();

            foreach (var service in ServiceEdits)
            {
                var newAction = currentDtate == ToggleType.Enabled
                    ?  ServiceAction.Stop
                    :  ServiceAction.Start;

                var result = ServiceHelper.ChangeState(service, newAction);

                if (!result.Success)
                    shareResult.Errors.Add(result.Message);
            }
        }

        private async Task ApplyServiceChanges(ModuleShareResult shareResult)
        {
            var currentState = GetRegistryState();

            foreach (var registry in RegistryEdits)
            {
                var newValue = currentState == ToggleType.Enabled
                    ? registry.DisabledValue
                    : registry.EnabledValue;

                var result = newValue.Equals(-1)
                    ? RegistryHelper.DeleteKey(registry)
                    : RegistryHelper.SetValue(registry, newValue);

                if (!result.Success)
                    shareResult.Errors.Add(result.Message);
            }
        }
    }
}
