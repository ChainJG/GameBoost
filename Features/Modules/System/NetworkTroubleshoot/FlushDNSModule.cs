using GameBoost.Features.Modules.Base;

namespace GameBoost.Features.Modules.System.NetworkTroubleshoot
{
    public sealed class FlushDNSModule : SystemTweakModuleBase
    {
        public override string Name => "Flush DNS";

        protected override string FormatStatus(ToggleType status) => string.Empty;
    }
}
