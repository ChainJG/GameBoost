using GameBoost.Infrastructure.MicrosoftApps.Models;
using GameBoost.Infrastructure.Shell;
using System.Text.Json;

namespace GameBoost.Infrastructure.MicrosoftApps.Services
{
    public sealed class AppxPackageQueryService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public static async Task<List<InstalledAppxPackageInfo>> QueryInstalledPackagesAsync(CancellationToken token = default)
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

            if (MicrosoftAppSafetyPolicy.IsProtectedPackage(package.Name))
                return false;

            return true;
        }

    }
}
