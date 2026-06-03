using GameBoost.Core.Interfaces;
using GameBoost.Infrastructure.Registry;
using GameBoost.Infrastructure.Services;
using GameBoost.Shared.Results;
using System.Diagnostics;

namespace GameBoost.Features.Modules.Base
{
    public abstract class SystemTweakModuleBase : IActionModule, IRecommendedActionModule
    {
        public abstract string Name { get; }

        #region Recommended Actions
        public virtual ToggleType RecommendedStatus => ToggleType.Enabled;

        public virtual object? RecommendedValue => RecommendedStatus;

        public virtual string RecommendedText => FormatStatus(RecommendedStatus);

        public virtual string RecommendationReason =>
            $"{Name} is recommended to be {RecommendedText}.";

        public virtual bool IsRecommendedValue(object? currentValue)
        {
            return currentValue is ToggleType toggleType &&
                   toggleType == RecommendedStatus;
        }
        #endregion

        public virtual RegistryEditInfo[] RegistryEdits { get; } = [];
        public virtual ServiceEditInfo[] ServiceEdits { get; } = [];

        protected virtual string FormatStatus(ToggleType status) => status.ToString();
        public async Task<ActionRefreshResult> RefreshStatusAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var status = GetToggleStatus();

            return await Task.FromResult(
            ActionRefreshResult.ValueOnly(
                status,
                FormatStatus(status)));
        }
        protected virtual ToggleType GetToggleStatus()
        {
            var states = new List<ToggleType>();

            states.AddRange(
                RegistryEdits
                .Where(CanUseRegistryEditForStatus)
                .Select(GetRegistryState));

            states.AddRange(ServiceEdits.Select(GetServiceState));

            if (states.Count == 0)
                return ToggleType.Unknown;

            if (states.All(state => state == ToggleType.Enabled))
                return ToggleType.Enabled;

            if (states.All(state => state == ToggleType.Disabled))
                return ToggleType.Disabled;

            return ToggleType.Unknown;
        }

        #region Get State Methods
        private ToggleType GetRegistryState(RegistryEditInfo edit)
        {
            var result = RegistryHelper.GetValue(edit);

            if (result is null || !result.Success)
            {
#if DEBUG
                WriteRegistryStateDebug(Name, edit, result ?? RegistryResult.Failed("Registry result was null"), ToggleType.Unknown);
#endif
                return ToggleType.Unknown;
            }

            var currentValue = result.Value;
            var valueExists = currentValue is not null;

            var resolvedState = ToggleType.Unknown;

            if (RegistryStateMatches(
                    currentValue,
                    valueExists,
                    edit.EnabledAction,
                    edit.EnabledValue))
            {
                resolvedState = ToggleType.Enabled;
            }
            else if (RegistryStateMatches(
                         currentValue,
                         valueExists,
                         edit.DisabledAction,
                         edit.DisabledValue))
            {
                resolvedState = ToggleType.Disabled;
            }
#if DEBUG
            if (edit.Debug)
                WriteRegistryStateDebug(Name, edit, result, resolvedState);
#endif

            return resolvedState;
        }
        private ToggleType GetServiceState(ServiceEditInfo service)
                => ServiceHelper.IsRunning(service)
                    ? ToggleType.Enabled
                    : ToggleType.Disabled;
        #endregion

        public virtual async Task<ModuleResult> ExecuteAsync(CancellationToken token)
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
            => currentStatus == ToggleType.Enabled
                ? ToggleType.Disabled
                : ToggleType.Enabled;

        #region Apply Changes
        protected void ApplyServiceChanges(ToggleType targetStatus, ModuleShareResult shareResult)
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
        protected void ApplyRegistryChanges(ToggleType targetStatus, ModuleShareResult shareResult)
        {
            foreach (var registry in RegistryEdits)
            {
                var action = GetRegistryAction(registry, targetStatus);
                var value = GetRegistryValue(registry, targetStatus);

                var result = action switch
                {
                    RegistryValueAction.Set => RegistryHelper.SetValue(registry, value),
                    RegistryValueAction.Delete => RegistryHelper.DeleteKey(registry),
                    _ => RegistryResult.Failed($"Unsupported registry action '{action}' for {registry.Key}")
                };

                if (!result.Success)
                    shareResult.Errors.Add(result.Message);
            }
        }
        #endregion

        #region Get Registry Helpers
        private static RegistryValueAction GetRegistryAction(
            RegistryEditInfo edit,
            ToggleType targetStatus)
            => targetStatus == ToggleType.Enabled
                ? edit.EnabledAction
                : edit.DisabledAction;
        private static object? GetRegistryValue(
            RegistryEditInfo edit,
            ToggleType targetStatus)
            => targetStatus == ToggleType.Enabled
                ? edit.EnabledValue
                : edit.DisabledValue;
        #endregion

        #region Registry Matching Helpers
        private static bool RegistryStateMatches(
            object? currentValue,
            bool valueExists,
            RegistryValueAction expectedAction,
            object? expectedValue)
        {
            return expectedAction switch
            {
                RegistryValueAction.Set => valueExists && ValuesMatch(currentValue, expectedValue),
                RegistryValueAction.Delete => !valueExists,
                RegistryValueAction.Ignore => false,

                _ => false
            };
        }
        private static bool ValuesMatch(object? currentValue, object? expectedValue)
        {
            if (currentValue is null || expectedValue is null)
                return false;

            return currentValue.ToString() == expectedValue.ToString();
        }
        #endregion

        private static bool CanUseRegistryEditForStatus(RegistryEditInfo edit)
        {
            if (edit.EnabledAction != edit.DisabledAction)
                return true;

            if (edit.EnabledAction == RegistryValueAction.Set)
                return !ValuesMatch(edit.EnabledValue, edit.DisabledValue);

            return false;
        }

        #region Debug
#if DEBUG
        [Conditional("DEBUG")]
        private static void WriteRegistryStateDebug(
            string Name,
            RegistryEditInfo edit,
            RegistryResult result,
            ToggleType resolvedState)
        {
            var currentValue = FormatRegistryValue(result.Value);
            var valueExists = result.Value is not null ? "Yes" : "No";

            Debug.WriteLine("");
            Debug.WriteLine("┌────────────────────────────────────────────────────────────");
            Debug.WriteLine($"│ Module: {Name}");
            Debug.WriteLine("┌────────────────────────────────────────────────────────────");
            Debug.WriteLine($"│ Registry State Check: {edit.Key}");
            Debug.WriteLine("├────────────────────────────────────────────────────────────");
            Debug.WriteLine($"│ Hive:             {edit.Hive}");
            Debug.WriteLine($"│ Path:             {edit.Path}");
            Debug.WriteLine($"│ Key:              {edit.Key}");
            Debug.WriteLine("├────────────────────────────────────────────────────────────");
            Debug.WriteLine($"│ Read Success:     {result.Success}");
            Debug.WriteLine($"│ Value Exists:     {valueExists}");
            Debug.WriteLine($"│ Current Value:    {currentValue}");
            Debug.WriteLine("├────────────────────────────────────────────────────────────");
            Debug.WriteLine($"│ Enabled Action:   {FormatRegistryTarget(edit.EnabledAction, edit.EnabledValue)}");
            Debug.WriteLine($"│ Disabled Action:  {FormatRegistryTarget(edit.DisabledAction, edit.DisabledValue)}");
            Debug.WriteLine("├────────────────────────────────────────────────────────────");
            Debug.WriteLine($"│ Resolved State:   {resolvedState}");
            Debug.WriteLine("└────────────────────────────────────────────────────────────");
            Debug.WriteLine("");
        }

        private static string FormatRegistryTarget(
            RegistryValueAction action,
            object? value)
        {
            return action switch
            {
                RegistryValueAction.Set =>
                    $"SetValue -> {FormatRegistryValue(value)}",

                RegistryValueAction.Delete =>
                    "DeleteValue -> <missing / deleted>",

                RegistryValueAction.Ignore =>
                    "Ignore -> <not checked / not applied>",

                _ =>
                    $"Unknown Action -> {action}"
            };
        }

        private static string FormatRegistryValue(object? value)
        {
            return value is null
                ? "<missing>"
                : $"{value} ({value.GetType().Name})";
        }
#endif
        #endregion

    }
}
