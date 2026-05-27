using GameBoost.Infrastructure.Registry;
using GameBoost.Shared.Results;
using Microsoft.Win32;
using System.ServiceProcess;

namespace GameBoost.Infrastructure.Services
{
    public static class ServiceHelper
    {
        public static bool IsRunning(ServiceEditInfo service)
        {
            using var sc = new ServiceController(service.Name);
            return sc.Status == ServiceControllerStatus.Running;
        }

        public static ModuleResult ChangeState(
            ServiceEditInfo service,
            ServiceAction action)
        {
            try
            {
                using var sc = new ServiceController(service.Name);

                switch (action)
                {
                    case ServiceAction.Start:
                        if (sc.Status != ServiceControllerStatus.Running)
                        {
                            sc.Start();
                            sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                        }
                        break;

                    case ServiceAction.Stop:
                        return StopIfRunning(service);

                    case ServiceAction.Enable:
                        SetStartupType(service, ServiceStartMode.Automatic);
                        break;

                    case ServiceAction.Disable:
                        StopIfRunning(service);
                        return SetStartupType(service, ServiceStartMode.Disabled);
                }

                return ModuleResult.Successful();
            }
            catch (UnauthorizedAccessException)
            {
                return ModuleResult.Failed("Administrator Permission Required", ResultType.AdministratorProtection);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine($"Error with {service.Name}: {ex.Message}");
#endif

                return ModuleResult.Failed(ex.Message);
            }
        }

        public static ModuleResult StopIfRunning(ServiceEditInfo service)
        {
            try
            {
                using var sc = new ServiceController(service.Name);

                if (sc.Status != ServiceControllerStatus.Running)
                    return ModuleResult.Successful();

                sc.Stop();
                sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));

                return ModuleResult.Successful();
            }
            catch (UnauthorizedAccessException)
            {
                return ModuleResult.Failed("Administrator Permission Required", ResultType.AdministratorProtection);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine($"Error with {service.Name}: {ex.Message}");
#endif

                return ModuleResult.Failed(ex.Message);
            }
        }

        private static ModuleResult SetStartupType(
            ServiceEditInfo service,
            ServiceStartMode mode)
        {
            try
            {
                var path = $@"SYSTEM\CurrentControlSet\Services\{service.Name}";

                int value = mode switch
                {
                    ServiceStartMode.Automatic => 2,
                    ServiceStartMode.Manual => 3,
                    ServiceStartMode.Disabled => 4,
                    _ => 3
                };

                var reg = new RegistryEditInfo
                {
                    Hive = RegistryHive.LocalMachine,
                    Path = path,
                    Key = "Start",
                };

                var result = RegistryHelper.SetValue(reg, value);

                if (!result.Success)
                {
#if DEBUG
                    Console.WriteLine($"Error with {reg.Path}: {result.Message}");
#endif

                    return ModuleResult.Failed(result.Message);
                }
            }
            catch (UnauthorizedAccessException)
            {
                return ModuleResult.Failed("Administrator Permission Required", ResultType.AdministratorProtection);
            }
            catch (Exception ex)
            {
#if DEBUG
                Console.WriteLine($"Error with {service.Name}: {ex.Message}");
#endif
                return ModuleResult.Failed(ex.Message);
            }

            return ModuleResult.Successful();
        }
    }
}
