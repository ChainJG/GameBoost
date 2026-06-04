using GameBoost.Shared.Results;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace GameBoost.Shared.Helpers.ProcessHelpers
{
    public static class ProcessHelper
    {
        private static readonly HashSet<string> ProtectedProcessesList = new(StringComparer.OrdinalIgnoreCase)
        {
            "system",
            "idle",
            "smss",
            "csrss",
            "wininit",
            "services",
            "lsass",
            "winlogon",
            "registry",
            "dwm",
            "sihost",
            "sppsvc",
            "svchost",
            "conhost",
            "ctfmon",
            "searchhost",
            "textinputhost",
            "widgetservice",
            "shellhost",
            "securityhealthservice",
            "securityhealthsystray",
            "wmiprvse",
            "vssvc",
            "msmpeng",
            "nissrv",
            "mpdefendercoreservice",
            "sechealthui",
            "smartscreen",
        };

        public static ModuleResult CloseProcessByName(string processName)
        {
            try
            {
                string normalizedName = NormalizeProcessName(processName);

                if (string.IsNullOrWhiteSpace(normalizedName))
                    return ModuleResult.Failed("Process name was empty.");

                if (IsProtectedProcess(normalizedName))
                    return ModuleResult.Failed($"Process {normalizedName} is protected and cannot be closed.");

                Process[] processes = Process.GetProcessesByName(normalizedName);

                if (processes.Length == 0)
                    return ModuleResult.Successful($"No running processes found matching {normalizedName}.");

                int closeRequestedCount = 0;
                int closedCount = 0;
                int failedCount = 0;

                foreach (Process process in processes)
                {
                    try
                    {
                        using (process)
                        {
                            if (process.HasExited)
                            {
                                closedCount++;
                                continue;
                            }

                            bool closeRequested = process.CloseMainWindow();

                            if (!closeRequested)
                            {
                                failedCount++;
                                continue;
                            }

                            closeRequestedCount++;

                            bool exited = process.WaitForExit(5000);

                            if (exited || process.HasExited)
                                closedCount++;
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
                    return ModuleResult.Successful($"Closed all {closedCount} process(es) matching {normalizedName}.");

                if (closeRequestedCount > 0)
                {
                    return ModuleResult.Successful(
                        $"Close request sent to {closeRequestedCount} process(es) matching {normalizedName}. " +
                        $"{failedCount} process(es) may still be running.");
                }

                return ModuleResult.Failed(
                    $"Failed to close processes matching {normalizedName}. They may not have normal windows to close.");
            }
            catch (Exception ex)
            {
                return ModuleResult.Failed($"Failed to close matching processes for {processName}: {ex.Message}");
            }
        }

        public static ModuleResult EndProcessByName(string processName)
        {
            try
            {
                string normalizedName = NormalizeProcessName(processName);

                if (string.IsNullOrWhiteSpace(normalizedName))
                    return ModuleResult.Failed("Process name was empty.");

                if (IsProtectedProcess(normalizedName))
                    return ModuleResult.Failed($"Process {normalizedName} is protected and cannot be ended.");

                Process[] processes = Process.GetProcessesByName(normalizedName);

                if (processes.Length == 0)
                    return ModuleResult.Successful($"No running processes found matching {normalizedName}.");

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

                            bool exited = process.WaitForExit(5000);

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
                    return ModuleResult.Successful($"Ended all {endedCount} process(es) matching {normalizedName}.");

                if (endedCount > 0)
                    return ModuleResult.Successful($"Ended {endedCount} process(es) matching {normalizedName}. {failedCount} process(es) could not be ended.");

                return ModuleResult.Failed($"Failed to end any processes matching {normalizedName}.");
            }
            catch (Exception ex)
            {
                return ModuleResult.Failed($"Failed to end matching processes for {processName}: {ex.Message}");
            }
        }

        public static bool IsProcessRunning(string processName)
        {
            string normalizedName = NormalizeProcessName(processName);
            return Process.GetProcessesByName(normalizedName).Any();
        }

        private static bool IsProtectedProcess(string processName)
        {
            return ProtectedProcessesList.Contains(NormalizeProcessName(processName));
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