using GameBoost.Core.Interfaces;
using GameBoost.Infrastructure.Registry;
using GameBoost.Infrastructure.Services;
using GameBoost.Shared.Results;
using System.Diagnostics;

namespace GameBoost.Features.Modules.Base
{
    public abstract class SystemTweakModuleBase : IActionModule
    {
        public abstract string Name { get; }
        public virtual RegistryEditInfo[] RegistryEdits { get; } = [];
        public virtual ServiceEditInfo[] ServiceEdits { get; } = [];

        protected virtual string FormatStatus(ToggleType status) => status.ToString();
        public async Task<string> RefreshStatusAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var status = GetToggleStatus();

            return FormatStatus(status);
        }
        protected virtual ToggleType GetToggleStatus()
        {
            var states = new List<ToggleType>();

            states.AddRange(RegistryEdits.Select(GetRegistryState));
            states.AddRange(ServiceEdits.Select(GetServiceState));

            if (states.Count == 0)
                return ToggleType.Unknown;

            if (states.All(state => state == ToggleType.Enabled))
                return ToggleType.Enabled;

            if (states.All(state => state == ToggleType.Disabled))
                return ToggleType.Disabled;

            return ToggleType.Unknown;
        }

        private ToggleType GetRegistryState(RegistryEditInfo edit)
        {
            var result = RegistryHelper.GetValue(edit);

            if (result is null || !result.Success)
                return ToggleType.Unknown;

            var value = result.Value;

            if (ValuesMatch(value, edit.EnabledValue))
                return ToggleType.Enabled;

            if (ValuesMatch(value, edit.DisabledValue))
                return ToggleType.Disabled;

            return ToggleType.Unknown;
        }

        private ToggleType GetServiceState(ServiceEditInfo service)
        {
            try
            {
                return ServiceHelper.IsRunning(service)
                    ? ToggleType.Enabled
                    : ToggleType.Disabled;
            }
            catch
            {
                return ToggleType.Unknown;
            }
        }

        public async Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            var result = new ModuleShareResult { Success = true };

            try
            {
                token.ThrowIfCancellationRequested();

                var currnetStatus = GetToggleStatus();
                var targetStatus = GetTargetStatus(currnetStatus);

                ApplyRegistryChanges(targetStatus, result);
                ApplyServiceChanges(targetStatus, result);

                if (result.Errors.Count > 0)
                    return ModuleResult.Failed(string.Join(Environment.NewLine, result.Errors));

                return ModuleResult.Successful($"Successfully Set {Name} To {FormatStatus(targetStatus)}");
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error in ExecuteAsync: {ex.Message}");
#endif
                return ModuleResult.Failed(ex.Message);
            }
        }

        protected virtual ToggleType GetTargetStatus(ToggleType currentStatus)
        {
            return currentStatus == ToggleType.Enabled
                ? ToggleType.Disabled
                : ToggleType.Enabled;
        }

        private void ApplyServiceChanges(ToggleType targetStatus, ModuleShareResult shareResult)
        {
            foreach (var service in ServiceEdits)
            {
                var newAction = targetStatus == ToggleType.Enabled
                    ?  ServiceAction.Start
                    :  ServiceAction.Stop;

                var result = ServiceHelper.ChangeState(service, newAction);

                if (!result.Success)
                    shareResult.Errors.Add(result.Message);
            }
        }
        private void ApplyRegistryChanges(ToggleType targetStatus, ModuleShareResult shareResult)
        {
            foreach (var registry in RegistryEdits)
            {

                var newValue = targetStatus == ToggleType.Enabled
                    ? registry.EnabledValue
                    : registry.DisabledValue;

                var result = newValue.Equals(-1)
                    ? RegistryHelper.DeleteKey(registry)
                    : RegistryHelper.SetValue(registry, newValue);

                if (!result.Success)
                    shareResult.Errors.Add(result.Message);
            }
        }

        private static bool ValuesMatch(object? currentValue, object? expectedValue)
        {
            if (currentValue is null || expectedValue is null)
                return false;

            return currentValue.ToString() == expectedValue.ToString();
        }
    }
}
