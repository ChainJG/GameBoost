using GameBoost.Features.Modules.Base;

namespace GameBoost.Features.Modules.System.NetworkTroubleshoot
{
    public sealed class ResetWinsockModule : SystemTweakModuleBase
    {
        public override string Name => "Restart Winsock";

        protected override string FormatStatus(ToggleType status) => string.Empty;
    }
}
