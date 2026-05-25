using GameBoost.Core.Interfaces;
using GameBoost.SystemInformation.Core;
using GameBoost.Update;

namespace GameBoost.Core.Services
{
    public static class AppServices
    {
        public static string AppName { get; } = "GameBoost";
        public static IDockController? Dock { get; set; }
        public static SystemInfo? SystemInfo { get; internal set; }
        public static UpdateReleaseInfo UpdateInfo { get; internal set; }
        public static bool HasActiveRestorePoint { get; internal set; }
    }
}
