using GameBoost.Application;
using GameBoost.Features.AppState;
using GameBoost.Infrastructure.Registry;
using GameBoost.Infrastructure.Shell;
using GameBoost.Shared.Results;
using Microsoft.Win32;
using System.Diagnostics;
using System.Management;

namespace GameBoost.Features.RestorePoints
{
    public class RestorePointHelper
    {
        private static string Description => $"{GameBoostContext.AppName} Restore Point";
        public static bool HasExistingGameBoostRestorePoint()
        {
            // If not admin, check if there is a restore point
            if (GameBoostContext.SystemInfo is not null && !GameBoostContext.SystemInfo.IsAdministrator)
            {
                var state = AppStateService.Load();
                return state.RestorePoint.LastStatus == ResultType.Successful;
            }

            // Compares the description of the restore points
            var hasRestorePoint = GetRestorePointInfoList().Any(p => p.Description == Description);

            // Update the restore point state
            SaveRestorePointState(hasRestorePoint ? ResultType.Successful : ResultType.Failed);

            return hasRestorePoint;
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

        public static void SaveRestorePointState(ResultType status)
        {
            var state = AppStateService.Load();

            state.RestorePoint.LastCreated = DateTime.Now;
            state.RestorePoint.LastStatus = status;

            AppStateService.Save(state);
        }

        public static ModuleResult CreateRestorePoint()
        {
            try
            {
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
                    ResultType.AdministratorProtection);
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
            var result = await ElevatedPowerShellService.RunPowerShellAsAdmin(
                "Enable-ComputerRestore -Drive 'C:\\'"
                );

            return result.Success && result.ExitCode == 0 
                ? ModuleResult.Successful("System protection enabled successfully")
                : ModuleResult.Failed($"Failed to enable system protection. (Exit code: {result.ExitCode})");
        }
    }
}
