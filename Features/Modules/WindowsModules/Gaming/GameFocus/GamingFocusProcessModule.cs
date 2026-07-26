using GameBoost.Core.Debugger;
using GameBoost.Shared.Helpers.ProcessHelpers;
using GameBoost.Shared.Results;
using System.Diagnostics;

namespace GameBoost.Features.Modules.WindowsModules.Gaming.GameFocus
{
    public sealed class GamingFocusProcessModule
    {
        public static ModuleResult ApplyFocusAction(GamingFocusProcessDefinition definition)
        {
            if (definition.TryGracefulClose && TryCloseProcess(definition))
                return ModuleResult.Successful("Process closed gracefully");

            if (definition.AllowForceKill && TryKillProcess(definition))
                return ModuleResult.Successful("Process killed forcefully");

            return ModuleResult.Failed("Process could not be closed or killed");
        }

        private static bool TryCloseProcess(GamingFocusProcessDefinition definition)
        {
            GameBoostDebug.Info($"Trying to close ({definition.ProcessName})");

            var closeResult = ProcessHelper.TryCloseProcess(definition.ProcessName);

            if (!closeResult.Success)
                return false;

            return true;
        }

        private static bool TryKillProcess(GamingFocusProcessDefinition definition)
        {
            GameBoostDebug.Info($"Trying to kill ({definition.ProcessName})");

            var killResult = ProcessHelper.TryEndProcess(definition.ProcessName);

            if (!killResult.Success)
                return false;

            return true;
        }

        #region Detection

        public IReadOnlyList<GamingFocusProcessDefinition> GetDetectedProcessGroups() =>
             [.. GamingFocusProcessCatalog.Processes
                .Where(definition => definition.EnabledByDefault)
                .Where(definition => IsProcessGroupRunning(definition))];

        private static bool IsProcessGroupRunning(GamingFocusProcessDefinition definition) =>
             Process
                .GetProcessesByName(definition.ProcessName)
                .Any(process =>
                {
                    using (process)
                    {
                        return IsDetectableProcess(
                            process,
                            definition);
                    }
                });

        private static bool IsDetectableProcess(Process process, GamingFocusProcessDefinition definition)
        {
            if (!ProcessHelper.CanTouchProcess(process))
                return false;

            // If force kill is allowed, the process can be detected even without a window
            if (definition.AllowForceKill)
                return true;

            // If force kill is NOT allowed, only detect it when it can be gracefully closed
            if (!definition.TryGracefulClose)
                return false;

            return ProcessHelper.HasMainWindow(process);
        }
        #endregion
    }
}
