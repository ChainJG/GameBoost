using GameBoost.Core.Interfaces;
using GameBoost.Features.Updates;
using GameBoost.SystemInformation.Core;

namespace GameBoost.Application
{
    public static class GameBoostContext
    {
        public static string AppName { get; } = "GameBoost";
        public static IDockController? Dock { get; set; }
        public static SystemInfo? SystemInfo { get; internal set; }
        public static UpdateReleaseInfo UpdateInfo { get; internal set; }
        public static bool HasActiveRestorePoint { get; internal set; }
    }
}
