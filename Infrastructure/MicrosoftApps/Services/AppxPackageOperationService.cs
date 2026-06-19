using GameBoost.Infrastructure.MicrosoftApps.Catalog;
using GameBoost.Infrastructure.MicrosoftApps.Models;
using GameBoost.Infrastructure.Shell;
using GameBoost.Shared.Results;
using System.Text.Json;

namespace GameBoost.Infrastructure.MicrosoftApps.Services
{
    public static class AppxPackageOperationService
    {
        private static readonly SemaphoreSlim RefreshLock = new(1, 1);

        private static List<InstalledAppxPackageInfo>? _installedPackages;

        public static IReadOnlyList<InstalledAppxPackageInfo> InstalledPackages => _installedPackages ?? [];

        public static async Task<List<InstalledAppxPackageInfo>> GetInstalledPackagesAsync(IProgress<ProgressResult>? progress = default, bool forceRefresh = false, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            if (!forceRefresh && _installedPackages is not null)
                return _installedPackages;

            await RefreshLock.WaitAsync(token);

            try
            {
                if (!forceRefresh && _installedPackages is not null)
                    return _installedPackages;

                progress?.Report(
                    new ProgressResult("Fetching Microsoft packages...", 70));

                _installedPackages = await AppxPackageQueryService.QueryInstalledPackagesAsync(token);

                return _installedPackages;
            }
            catch
            {
                return [];
            }
            finally
            {
                RefreshLock.Release();
            }
        }

        public static async Task<bool> IsInstalledForCurrentUserAsync(string packageName, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var package = GetInstalledPackageAsync(packageName);

            return package is not null;
        }

        public static async Task<ModuleResult> InstallPackageAsync(MicrosoftAppDefinition app, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var packageName = EscapeSingleQuotedPowerShell(app.PackageName);

            var command =
                "$packages = Get-AppxPackage -AllUsers -Name '" + packageName + "'; " +
                "if (-not $packages) { throw 'Package was not found on this system.' }; " +
                "$package = $packages | Where-Object { $_.InstallLocation } | Select-Object -First 1; " +
                "if (-not $package) { throw 'Package install location was not found.' }; " +
                "$manifest = Join-Path $package.InstallLocation 'AppXManifest.xml'; " +
                "if (-not (Test-Path $manifest)) { throw 'AppXManifest.xml was not found.' }; " +
                "Add-AppxPackage -DisableDevelopmentMode -Register $manifest";

            var result = await ShellService.RunAsync(
                ShellType.PowerShell,
                command,
                token);

            if (!result.Success)
                return ModuleResult.Failed(result.Error);

            return ModuleResult.Successful($"{app.DisplayName} was installed/re-registered");
        }

        public static async Task<ModuleResult> UninstallPackageAsync(MicrosoftAppDefinition app, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (MicrosoftAppSafetyPolicy.IsProtectedPackage(app.PackageName))
                return ModuleResult.Failed($"{app.DisplayName} is protected and should not be removed");

            var installedPackage = GetInstalledPackageAsync(app.PackageName);


            if (installedPackage is null)
                return ModuleResult.Failed($"{app.DisplayName} is not installed for the current user");

            var packageFullName = EscapeSingleQuotedPowerShell(installedPackage.PackageFullName);

            var command = "Remove-AppxPackage -Package '" + packageFullName + "'";

            var result = await ShellService.RunAsync(
                ShellType.PowerShell,
                command,
                token);

            if (!result.Success)
                return ModuleResult.Failed(result.Error);

            RemovePackageFromCache(installedPackage.PackageFullName);

            return ModuleResult.Successful($"{app.DisplayName} was uninstalled");
        }

        private static InstalledAppxPackageInfo? GetInstalledPackageAsync(string packageName)
        {
            return InstalledPackages.FirstOrDefault(package =>
                PackageMatches(package, packageName));
        }

        private static bool PackageMatches(InstalledAppxPackageInfo package, string packageName)
        {
            if (string.Equals(
                    package.Name,
                    packageName,
                    StringComparison.OrdinalIgnoreCase) 

                || package.PackageFullName.StartsWith(
                    packageName + "_",
                    StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }


        private static void RemovePackageFromCache(string packageFullName)
        {
            if (_installedPackages is null)
                return;

            _installedPackages =
            [
                .. _installedPackages.Where(package =>
                    !string.Equals(
                        package.PackageFullName,
                        packageFullName,
                        StringComparison.OrdinalIgnoreCase))
            ];
        }

        private static string EscapeSingleQuotedPowerShell(string value)
        {
            return value.Replace("'", "''");
        }
    }
}