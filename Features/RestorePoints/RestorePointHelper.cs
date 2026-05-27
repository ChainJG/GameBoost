using GameBoost.Application;
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
        private static string Description => $"{GameBoostContext.AppName} Restore Point";
        public static bool HasExistingGameBoostRestorePoint()
        {
            // If not admin, check if there is a restore point
            if (GameBoostContext.SystemInfo != null && !GameBoostContext.SystemInfo.IsAdministrator)
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

                using var mc =
                    new ManagementClass(scope,
                    new ManagementPath("SystemRestore"),
                    null);

                var inParams =
                    mc.GetMethodParameters("CreateRestorePoint");

                inParams["Description"] = Description;
                inParams["RestorePointType"] = 0;
                inParams["EventType"] = 100;

                var result =
                    mc.InvokeMethod("CreateRestorePoint",
                    inParams,
                    null);


                uint returnValue = (uint)(result?["ReturnValue"] ?? 1);

                SaveRestorePointState(returnValue == 0 ? ResultType.Successful : ResultType.Failed);

                return returnValue switch
                {
                    0 => new ModuleResult
                    {
                        Success = true,
                        Status = ResultType.Successful,
                        Message = "Successfully created restore point"
                    },

                    _ => new ModuleResult
                    {
                        Success = false,
                        Status = ResultType.Failed,
                        Message = $"Restore point failed (Code: {returnValue})"
                    }
                };
            }

            catch (ManagementException ex)
            {
#if DEBUG
                Debug.WriteLine($"WMI Error: {ex.Message}");
#endif

                return new ModuleResult
                {
                    Success = false,
                    Status = ResultType.AdministratorProtection,
                    Message = "Administrator Permission Required"
                };
            }

            catch (Exception ex)
            {

#if DEBUG
                Debug.WriteLine($"Error in CreateRestorePoint: {ex.Message}");
#endif

                return new ModuleResult
                {
                    Success = false,
                    Status = ResultType.Failed,
                    Message = "Failed to create restore point"
                };

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
        public static ModuleResult EnableSystemProtection()
        {
            // Enables windows system protection so restore points can be created
            var result = ElevatedPowerShellService.RunPowerShellAsAdmin(
                "Enable-ComputerRestore -Drive 'C:\\'");

            return result.Success && result.ExitCode == 0 
                ? ModuleResult.Successful("System protection enabled successfully")
                : ModuleResult.Failed($"Failed to enable system protection. (Exit code: {result.ExitCode})");
        }
    }
}
