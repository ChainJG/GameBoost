using GameBoost.Shared.Results;
using System.Diagnostics;
using System.IO;

namespace GameBoost.Shared.Helpers.ProcessHelpers
{
    public static class ProcessHelper
    {
        private static readonly HashSet<string> ProtectedProcessesList = new(StringComparer.OrdinalIgnoreCase)
        {
            "system",
            "idle",
            "registry",
            "smss",
            "csrss",
            "wininit",
            "services",
            "lsass",
            "winlogon",
            "svchost",
            "dwm",
            "explorer",
            "sihost",
            "fontdrvhost",
            "conhost",
            "ctfmon",
            "runtimebroker",
            "searchhost",
            "startmenuexperiencehost",
            "shellexperiencehost",
            "securityhealthservice",
            "securityhealthsystray",
            "msmpeng",
            "smartscreen",
            "audiodg",
            "nvcontainer",
            "amdow",
            "atiesrxx",
            "atieclxx",
        };

        public static ModuleResult TryCloseProcess(string processName)
        {
            try
            {
                string normalizedName = NormalizeProcessName(processName);

                if (string.IsNullOrWhiteSpace(normalizedName))
                    return ModuleResult.Failed("Process name was empty");

                if (IsProtectedProcess(normalizedName))
                    return ModuleResult.Failed($"Process {normalizedName} is protected and cannot be closed");

                Process[] processes = Process.GetProcessesByName(normalizedName);

                if (processes.Length == 0)
                    return ModuleResult.Successful($"No running processes found matching {normalizedName}");

                int closeRequestedCount = 0;
                int closedCount = 0;
                int failedCount = 0;

                foreach (Process process in processes)
                {
                    try
                    {
                        if (process.HasExited)
                        {
                            closedCount++;
                            continue;
                        }

                        if (process.MainWindowHandle == IntPtr.Zero)
                        {
                            failedCount++;
                            continue;
                        }

                        bool closeRequested = process.CloseMainWindow();

                        if (!closeRequested)
                        {
                            failedCount++;
                            continue;
                        }

                        closeRequestedCount++;

                        bool exited = process.WaitForExit(3000);

                        if (exited || process.HasExited)
                            closedCount++;
                        else
                            failedCount++;
                    }
                    catch
                    {
                        failedCount++;
                    }
                }

                bool stillRunning = Process.GetProcessesByName(normalizedName).Length > 0;

                if (!stillRunning)
                    return ModuleResult.Successful($"Closed all {closedCount} process(es) matching {normalizedName}");

                if (closeRequestedCount > 0)
                {
                    return ModuleResult.Successful(
                        $"Close request sent to {closeRequestedCount} process(es) matching {normalizedName} " +
                        $"{failedCount} process(es) may still be running");
                }

                return ModuleResult.Failed(
                    $"Failed to close processes matching {normalizedName}. They may not have normal windows to close");
            }
            catch (Exception ex)
            {
                return ModuleResult.Failed($"Failed to close matching processes for {processName}: {ex.Message}");
            }
        }

        public static void TryEndProcess(Process process)
        {
            try
            {
                if (!process.HasExited)
                    process.Kill(entireProcessTree: true);
            }
            catch
            {
            }
        }
        public static ModuleResult TryEndProcess(string processName)
        {
            try
            {
                string normalizedName = NormalizeProcessName(processName);

                if (string.IsNullOrWhiteSpace(normalizedName))
                    return ModuleResult.Failed("Process name was empty");

                if (IsProtectedProcess(normalizedName))
                    return ModuleResult.Failed($"Process {normalizedName} is protected and cannot be ended");

                Process[] processes = Process.GetProcessesByName(normalizedName);

                if (processes.Length == 0)
                    return ModuleResult.Successful($"No running processes found matching {normalizedName}");

                int endedCount = 0;
                int failedCount = 0;

                foreach (Process process in processes)
                {
                    try
                    {
                        using (process)
                        {
                            if (process.HasExited)
                            {
                                endedCount++;
                                continue;
                            }

                            process.Kill();

                            bool exited = process.WaitForExit(3000);

                            if (exited || process.HasExited)
                                endedCount++;
                            else
                                failedCount++;
                        }
                    }
                    catch
                    {
                        failedCount++;
                    }
                }

                bool stillRunning = Process.GetProcessesByName(normalizedName).Length > 0;

                if (!stillRunning)
                    return ModuleResult.Successful($"Ended all {endedCount} process(es) matching {normalizedName}");

                if (endedCount > 0)
                    return ModuleResult.Successful($"Ended {endedCount} process(es) matching {normalizedName}. {failedCount} process(es) could not be ended");

                return ModuleResult.Failed($"Failed to end any processes matching {normalizedName}");
            }
            catch (Exception ex)
            {
                return ModuleResult.Failed($"Failed to end matching processes for {processName}: {ex.Message}");
            }
        }

        public static bool IsProcessRunning(string processName)
        {
            string normalizedName = NormalizeProcessName(processName);
            return Process.GetProcessesByName(normalizedName).Length != 0;
        }

        public static bool CanTouchProcess(Process process)
        {
            try
            {
                var processName = process.ProcessName;
                var currnetProcessId = Environment.ProcessId;

                if (process.Id == currnetProcessId)
                    return false;

                if (string.IsNullOrWhiteSpace(processName))
                    return false;

                if (IsProtectedProcess(processName))
                    return false;

                if (process.SessionId == 0)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool HasMainWindow(Process process)
        {
            try
            {
                process.Refresh();

                return !IsProcessSuspended(process) && process.MainWindowHandle != IntPtr.Zero;
            }
            catch
            {
                return false;
            }
        }
        public static bool IsProtectedProcess(string processName) => ProtectedProcessesList.Contains(NormalizeProcessName(processName));
        public static bool IsProcessSuspended(Process process)
        {
            try
            {
                process.Refresh();

                if (process.HasExited)
                    return false;

                var checkedThreads = 0;
                var suspendedThreads = 0;

                foreach (ProcessThread thread in process.Threads)
                {
                    try
                    {
                        if (thread.ThreadState == System.Diagnostics.ThreadState.Terminated)
                            continue;

                        checkedThreads++;

                        if (thread.ThreadState == System.Diagnostics.ThreadState.Wait &&
                            thread.WaitReason == ThreadWaitReason.Suspended)
                        {
                            suspendedThreads++;
                        }
                    }
                    catch
                    {
                        // Some system/protected threads may not expose state cleanly.
                    }
                }

                return checkedThreads > 0 &&
                       suspendedThreads == checkedThreads;
            }
            catch
            {
                return false;
            }
        }



        private static string NormalizeProcessName(string processName)
        {
            if (string.IsNullOrWhiteSpace(processName))
                return string.Empty;

            string trimmedName = processName.Trim();

            if (trimmedName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                trimmedName = Path.GetFileNameWithoutExtension(trimmedName);

            return trimmedName;
        }
    }
}