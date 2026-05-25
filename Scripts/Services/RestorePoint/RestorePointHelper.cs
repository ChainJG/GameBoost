using GameBoost.Core.Services;
using GameBoost.Scripts.Models;
using GameBoost.Scripts.Registry;
using GameBoost.Scripts.Registry.Model;
using GameBoost.Scripts.Services.AppState;
using GameBoost.Scripts.Services.Models;
using GameBoost.Scripts.Services.Shell;
using Microsoft.Win32;
using System.Diagnostics;
using System.Management;

namespace GameBoost.Scripts.Services.RestorePoint
{
    public class RestorePointHelper
    {
        private static string Description => $"{AppServices.AppName} Restore Point";
        public static bool HasGameBoostRestorePoint()
        {
            // If not admin, check if there is a restore point
            if (AppServices.SystemInfo != null && !AppServices.SystemInfo.IsAdministrator)
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
            var value = RegistryHelper.GetValue(new RegistryEditInfo
            {
                Hive = RegistryHive.LocalMachine,
                Path = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\SystemRestore",
                Key = "RPSessionInterval"
            });

            return value is not null and (object)1;
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
                    Message = "Administrator permission required"
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
            // Enable system protection
            var result = PowerShellAdminService.RunPowerShellAsAdmin("Enable-ComputerRestore -Drive 'C:\\'");

            return new ModuleResult
            {
                Success = result.Success,
                Status = result.ExitCode == 0 ? ResultType.Successful : ResultType.Failed,
            };
        }
    }
}
