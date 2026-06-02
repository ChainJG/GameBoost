using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry.DirectXUserGlobal;
using GameBoost.Shared.Results;
using System.Diagnostics;

namespace GameBoost.Features.Modules.Windows.Gaming
{
    public class VariableRefreshRateModule : SystemTweakModuleBase
    {
        public override string Name => "Variable Refresh Rate";

        protected const string FLAG_NAME = "VRROptimizeEnable";

        protected override ToggleType GetToggleStatus()
        {

            var result = DirectXUserGlobalHelper.GetDirectXUserGlobalFlag(FLAG_NAME);

            if (!result.Success || result.Value is null)
                return ToggleType.Unknown;

            return result.Value.Equals("1") ? ToggleType.Enabled : ToggleType.Disabled;
        }

        public override async Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var currnetStatus = GetToggleStatus();
                var targetStatus = GetTargetStatus(currnetStatus);

                var newValue = targetStatus.Equals(ToggleType.Enabled) ? 1 : 0;

                var result = DirectXUserGlobalHelper.SetDirectXUserGlobalFlag(FLAG_NAME, newValue);

                if (!result.Success)
                    return ModuleResult.Failed(result.Message);

                return ModuleResult.Successful($"Successfully Set {Name} To {FormatStatus(targetStatus)}");
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error {Name} Execution: {ex.Message}");
#endif
                return ModuleResult.Failed(ex.Message);
            }
        }
    }
}
