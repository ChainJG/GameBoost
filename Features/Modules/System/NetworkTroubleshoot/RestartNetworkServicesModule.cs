using GameBoost.Features.Modules.Base;

namespace GameBoost.Features.Modules.System.NetworkTroubleshoot
{
    internal class RestartNetworkServicesModule : SystemTweakModuleBase
    {
        public override string Name => "Restart Network Services";

        protected override string FormatStatus(ToggleType status) => string.Empty;
    }
}
