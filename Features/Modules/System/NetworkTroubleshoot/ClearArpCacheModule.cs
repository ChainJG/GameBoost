using GameBoost.Features.Modules.Base;

namespace GameBoost.Features.Modules.System.NetworkTroubleshoot
{
    public sealed class ClearArpCacheModule : SystemTweakModuleBase
    {
        public override string Name => "Clear ARP Cache";

        protected override string FormatStatus(ToggleType status) => string.Empty;
    }
}
