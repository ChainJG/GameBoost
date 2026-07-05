using GameBoost.Application;
using GameBoost.Core;
using GameBoost.Core.Debugger;
using GameBoost.Features.AppState;
using GameBoost.Infrastructure.Registry;
using GameBoost.Infrastructure.Shell;
using GameBoost.Shared.Helpers;
using GameBoost.Shared.Results;
using Microsoft.Win32;
using System.Diagnostics;
using System.Management;

namespace GameBoost.Features.RestorePoints
{
    public class RestorePointHelper
    {
        private static readonly string Description = $"{GameBoostContext.AppName} Restore Point";
        private static readonly TimeSpan RestorePointMaxAge = TimeSpan.FromDays(30);

        public static bool HasExistingGameBoostRestorePoint(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            // If not admin, check if there is a restore point
            if (!GameBoostServices.IsAdministrator())
                return HasRecentSuccessfulRestorePoint();

            // Compares the description of the restore points
            var hasRestorePoint = GetRestorePointInfoList().Any(p => p.Description == Description);

            // Update the restore point state
            SaveRestorePointState(hasRestorePoint ? ResultType.Successful : ResultType.Failed);

            return hasRestorePoint;
        }
        private static bool HasRecentSuccessfulRestorePoint()
        {
            var state = AppStateService.Load();
            var restorePoint = state.RestorePoint;

#if DEBUG
            GameBoostDebug.Info(
                $"Loaded restore point state. Status: {restorePoint.LastStatus}, " +
                $"LastCreated: {(restorePoint.LastCreated.HasValue ? restorePoint.LastCreated.Value.ToString("yyyy-MM-dd HH:mm:ss") : "None")}, " +
                $"MaxAge: {StringHelper.FormatTimeSpan(RestorePointMaxAge)}");
#endif

            if (restorePoint.LastStatus != ResultType.Successful)
            {
#if DEBUG
                GameBoostDebug.Error(
                    $"Restore point check failed. Last status was {restorePoint.LastStatus}, not {ResultType.Successful}");
#endif

                return false;
            }

            if (restorePoint.LastCreated is not { } lastCreated)
            {
#if DEBUG
                GameBoostDebug.Error(
                    "Restore point check failed. LastCreated was empty even though status was successful");
#endif

                return false;
            }

            var age = DateTime.Now - lastCreated;

            if (age < TimeSpan.Zero)
            {
#if DEBUG
                GameBoostDebug.Error(
                    "Restore point check failed. LastCreated is in the future, which means the saved state/time is invalid");
#endif

                return false;
            }

            if (age > RestorePointMaxAge)
            {
#if DEBUG
                GameBoostDebug.Error(
                    $"Restore point check failed. Restore point is too old. " +
                    $"Age: {StringHelper.FormatTimeSpan(age)}, MaxAllowedAge: {StringHelper.FormatTimeSpan(RestorePointMaxAge)}");
#endif

                return false;
            }

#if DEBUG
            GameBoostDebug.Success(
                $"Restore point check passed. Recent successful restore point found. Age: {StringHelper.FormatTimeSpan(age)}");
#endif

            return true;
        }

        public static void SaveRestorePointState(ResultType status)
        {
            var state = AppStateService.Load();

            state.RestorePoint.LastCreated = DateTime.Now;
            state.RestorePoint.LastStatus = status;

#if DEBUG
            GameBoostDebug.Info(status == ResultType.Successful
                ? "Renewed Restore Point saved status" 
                : $"Restore Point status has been set to {status}");
#endif

            AppStateService.Save(state);
        }

        public static ModuleResult CreateRestorePoint(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var scope = new ManagementScope(@"\\.\root\default");
                scope.Connect();

                using var restoreClass = new ManagementClass(
                    scope,
                    new ManagementPath("SystemRestore"),
                    null);

                using var inParams = restoreClass.GetMethodParameters("CreateRestorePoint");

                inParams["Description"] = Description;
                inParams["RestorePointType"] = 0;
                inParams["EventType"] = 100;

                using var outParams = restoreClass.InvokeMethod(
                    "CreateRestorePoint",
                    inParams,
                    null);

                var returnValue = Convert.ToUInt32(outParams?["ReturnValue"] ?? 1);

                var status = returnValue == 0
                    ? ResultType.Successful
                    : ResultType.Failed;

                SaveRestorePointState(status);

                return returnValue switch
                {
                    0 => ModuleResult.Successful("Successfully created restore point"),
                    _ => ModuleResult.Failed(
                        $"Restore point failed. Windows returned code {returnValue}.")
                };
            }
            catch (UnauthorizedAccessException ex)
            {
#if DEBUG
                Debug.WriteLine($"Restore point permission error: {ex.Message}");
#endif

                return ModuleResult.Failed(
                    "Administrator permission is required to create a restore point.",
                    ResultType.Administrator);
            }
            catch (ManagementException ex)
            {
#if DEBUG
                Debug.WriteLine($"WMI Error in CreateRestorePoint: {ex.Message}");
                Debug.WriteLine($"WMI Error Code: {ex.ErrorCode}");
#endif

                return ModuleResult.Failed(
                    $"Failed to create restore point. WMI error: {ex.Message}",
                    ResultType.Failed);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error in CreateRestorePoint: {ex.Message}");
#endif

                return ModuleResult.Failed(
                    "Failed to create restore point.",
                    ResultType.Failed);
            }
        }

        public static List<RestorePointInfo> GetRestorePointInfoList()
        {
            var restorePoints = new List<RestorePointInfo>();

            try
            {
                var searcher = new ManagementObjectSearcher(
                    @"root\default",
                    "SELECT * FROM SystemRestore");

                foreach (ManagementObject obj in searcher.Get())
                {
                    restorePoints.Add(new RestorePointInfo
                    {
                        Description = obj["Description"]?.ToString(),
                        SequenceNumber = Convert.ToInt32(obj["SequenceNumber"]),
                        RestorePointType = Convert.ToInt32(obj["RestorePointType"])
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in GetRestorePointInfoList: {ex.Message}");
            }

            return restorePoints;
        }

        public static async Task<ModuleResult> EnableSystemProtection()
        {
            // Enables windows system protection so restore points can be created
            var result = await AdminExecutionService.RunAsAdminAsync(
                ShellType.PowerShell,
                "Enable-ComputerRestore -Drive 'C:\\'"
                );

            return result.Success && result.ExitCode == 0 
                ? ModuleResult.Successful("System protection enabled successfully")
                : ModuleResult.Failed($"Failed to enable system protection. (Exit code: {result.ExitCode})");
        }
        public static bool IsSystemProtectionEnabled()
        {
            // Check if system protection is enabled
            var result = RegistryHelper.GetValue(new RegistryEditInfo
            {
                Hive = RegistryHive.LocalMachine,
                Path = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore",
                Key = "RPSessionInterval"
            });

            return result?.Value is not null and (object)1;
        }
    }
}
