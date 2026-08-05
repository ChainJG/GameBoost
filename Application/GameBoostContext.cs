using GameBoost.Application.Diagnostics;
using GameBoost.Features.Updates;
using GameBoost.Infrastructure.MicrosoftApps.Models;
using GameBoost.SystemInformation.Core;

namespace GameBoost.Application
{
    public static class GameBoostContext
    {
        public static bool IsBeta { get; } = false;
        public static string AppName { get; } = "GameBoost";
        public static DiagnosticService Diagnostic { get; set; } = DiagnosticService.Disabled;
        public static SystemInfo? SystemInfo { get; internal set; }
        public static UpdateReleaseInfo? UpdateInfo { get; internal set; }
        public static bool HasActiveRestorePoint { get; internal set; }
    }
}
