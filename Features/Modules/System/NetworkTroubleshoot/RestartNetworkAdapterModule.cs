using GameBoost.Features.Modules.Base;

namespace GameBoost.Features.Modules.System.NetworkTroubleshoot
{
    public sealed class RestartNetworkAdapterModule : SystemTweakModuleBase
    {
        public override string Name => "Restart Network Adapter";

        protected override string FormatStatus(ToggleType status) => string.Empty;
    }
}
