using GameBoost.Features.Modules.Base;

namespace GameBoost.Features.Modules.System.NetworkTroubleshoot
{
    public sealed class ReleaseAndRenewIPModule : SystemTweakModuleBase
    {
        public override string Name => "Release and Renew IP";

        protected override string FormatStatus(ToggleType status) => string.Empty;
    }
}
