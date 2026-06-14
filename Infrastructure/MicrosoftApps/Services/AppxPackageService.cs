using GameBoost.Infrastructure.MicrosoftApps.Models;
using GameBoost.Infrastructure.Shell;
using GameBoost.Shared.Results;
using System.Text.Json;

namespace GameBoost.Infrastructure.MicrosoftApps.Services
{
    public static class AppxPackageService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private static readonly string[] ProtectedPackagePrefixes =
        [
            "Microsoft.WindowsStore",
            "Microsoft.StorePurchaseApp",
            "Microsoft.DesktopAppInstaller",
            "Microsoft.Windows.ShellExperienceHost",
            "Microsoft.Windows.StartMenuExperienceHost",
            "Microsoft.Windows.Search",
            "Microsoft.AAD.BrokerPlugin",
            "Microsoft.AccountsControl",
            "Microsoft.LockApp",
            "Microsoft.Windows.CloudExperienceHost",
            "Microsoft.Windows.ContentDeliveryManager",
            "Microsoft.Windows.OOBENetworkConnectionFlow",
            "MicrosoftWindows.Client.CBS"
        ];

        private static readonly SemaphoreSlim RefreshLock = new(1, 1);

        private static List<InstalledAppxPackageInfo>? _installedPackages;

        public static IReadOnlyList<InstalledAppxPackageInfo> InstalledPackages =>
            _installedPackages ?? [];

        public static async Task<List<InstalledAppxPackageInfo>> GetInstalledPackagesAsync(
            IProgress<ProgressResult>? progress = default,
            bool forceRefresh = false,
            CancellationToken token = default)
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
                    new ProgressResult("Querying installed packages...", 70));

                _installedPackages = await QueryInstalledPackagesAsync(token);

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

        public static async Task<bool> IsInstalledForCurrentUserAsync(
            string packageName,
            CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var package = await GetInstalledPackageAsync(
                packageName,
                forceRefresh: false,
                token);

            return package is not null;
        }

        public static async Task<ModuleResult> ReRegisterPackageAsync(
            MicrosoftAppDefinition app,
            CancellationToken token = default)
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
                return ModuleResult.Failed(GetPowerShellError(result));

            InvalidateCache();

            return ModuleResult.Successful($"{app.DisplayName} was installed/re-registered");
        }

        public static async Task<ModuleResult> UninstallPackageAsync(MicrosoftAppDefinition app, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (IsProtectedPackage(app.PackageName))
                return ModuleResult.Failed($"{app.DisplayName} is protected and should not be removed");

            var installedPackage = await GetInstalledPackageAsync(
                app.PackageName,
                forceRefresh: false,
                token);

            if (installedPackage is null)
            {
                installedPackage = await GetInstalledPackageAsync(
                    app.PackageName,
                    forceRefresh: true,
                    token);
            }

            if (installedPackage is null)
                return ModuleResult.Failed($"{app.DisplayName} is not installed for the current user");

            var packageFullName = EscapeSingleQuotedPowerShell(installedPackage.PackageFullName);

            var command = "Remove-AppxPackage -Package '" + packageFullName + "'";

            var result = await ShellService.RunAsync(
                ShellType.PowerShell,
                command,
                token);

            if (!result.Success)
                return ModuleResult.Failed(GetPowerShellError(result));

            RemovePackageFromCache(installedPackage.PackageFullName);

            return ModuleResult.Successful($"{app.DisplayName} was uninstalled");
        }

        public static bool IsProtectedPackage(string packageName)
        {
            return ProtectedPackagePrefixes.Any(prefix =>
                packageName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }

        public static void InvalidateCache()
        {
            _installedPackages = null;
        }

        private static async Task<InstalledAppxPackageInfo?> GetInstalledPackageAsync(string packageName, bool forceRefresh, CancellationToken token)
        {
            var packages = await GetInstalledPackagesAsync(
                forceRefresh: forceRefresh,
                token: token);

            return packages.FirstOrDefault(package =>
                PackageMatches(package, packageName));
        }

        private static bool PackageMatches(
            InstalledAppxPackageInfo package,
            string packageName)
        {
            if (string.Equals(
                    package.Name,
                    packageName,
                    StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (package.PackageFullName.StartsWith(
                    packageName + "_",
                    StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        private static async Task<List<InstalledAppxPackageInfo>> QueryInstalledPackagesAsync(
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var command =
                "Get-AppxPackage -PackageTypeFilter Main | " +
                "Select-Object Name, PackageFullName, DisplayName, InstallLocation, Publisher, IsFramework, IsResourcePackage | " +
                "ConvertTo-Json -Depth 3";

            var result = await ShellService.RunAsync(
                ShellType.PowerShell,
                command,
                token);

            if (!result.Success || string.IsNullOrWhiteSpace(result.Output))
                return [];

            return
            [
                .. ParseInstalledPackages(result.Output)
                    .Where(IsUserFacingPackage)
                    .OrderBy(package => package.FriendlyName)
            ];
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

        private static List<InstalledAppxPackageInfo> ParseInstalledPackages(string json)
        {
            try
            {
                var trimmed = json.Trim();

                if (trimmed.StartsWith('['))
                {
                    return JsonSerializer.Deserialize<List<InstalledAppxPackageInfo>>(
                               trimmed,
                               JsonOptions)
                           ?? [];
                }

                var single = JsonSerializer.Deserialize<InstalledAppxPackageInfo>(
                    trimmed,
                    JsonOptions);

                return single is null ? [] : [single];
            }
            catch
            {
                return [];
            }
        }

        private static bool IsUserFacingPackage(InstalledAppxPackageInfo package)
        {
            if (string.IsNullOrWhiteSpace(package.Name) ||
                string.IsNullOrWhiteSpace(package.PackageFullName))
            {
                return false;
            }

            if (package.IsFramework || package.IsResourcePackage)
                return false;

            if (IsProtectedPackage(package.Name))
                return false;

            return true;
        }

        private static string EscapeSingleQuotedPowerShell(string value)
        {
            return value.Replace("'", "''");
        }

        private static string GetPowerShellError(ProcessResult result)
        {
            if (!string.IsNullOrWhiteSpace(result.Error))
                return result.Error.Trim();

            if (!string.IsNullOrWhiteSpace(result.Output))
                return result.Output.Trim();

            return $"PowerShell command failed with exit code {result.ExitCode}";
        }
    }
}