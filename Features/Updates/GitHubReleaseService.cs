using GameBoost.Core;
using GameBoost.Infrastructure.Http;
using GameBoost.Shared.Results;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace GameBoost.Features.Updates
{
    public class GitHubReleaseService
    {
        private const string OWNER = "ChainJG";
        private const string REPO = "GameBoost";
        private const string API_URL = $"https://api.github.com/repos/{OWNER}/{REPO}/releases/latest";

        private static string CurrentVersion => Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "1.0.0";

        public static async Task<UpdateReleaseInfo> CheckForUpdatesAsync(IProgress<ProgressResult>? progress = default)
        {
            try
            {
                // Report current update check status
                progress?.Report(
                    new ProgressResult(
                        "Checking for updates...",
                        25));


                // Download latest release JSON from GitHub using the shared HTTP client.
                var json = await HttpClientProvider.Client.GetStringAsync(API_URL);

                // Parse GitHub release response into model
                var releaseInfo = ParseGitHubRelease(json);

                // Check if a newer version exists
                if (releaseInfo.IsUpdateAvailable)
                {
                    // Report update found
                    progress?.Report(
                        new ProgressResult(
                            $"Update available: v{releaseInfo.Version}",
                            100));

                    // Handle user update flow
                    await GameBoostServices.ShowUpdateDialog(releaseInfo, progress);
                }

                return releaseInfo;
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine(
                    $"Update check error: {ex.Message}");
#endif

                // Report update failure
                progress?.Report(
                    new ProgressResult(
                        "Error checking for updates",
                        100));

                return new UpdateReleaseInfo
                {
                    Version = CurrentVersion
                };
            }
        }

        private static UpdateReleaseInfo ParseGitHubRelease(string json)
        {
            try
            {
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                var version = root.GetProperty("tag_name").GetString()?.TrimStart('v') ?? "unknown";
                var releaseNotes = root.GetProperty("body").GetString() ?? "";

                string downloadUrl =
                    root.GetProperty("assets")[0]
                    .GetProperty("browser_download_url").GetString() ?? "";

                // Compare versions
                bool isUpdateAvailable = CompareVersions(version, CurrentVersion) > 0;

                return new UpdateReleaseInfo
                {
                    Version = version,
                    DownloadUrl = downloadUrl,
                    Notes = releaseNotes,
                    IsUpdateAvailable = isUpdateAvailable
                };
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error parsing GitHub release: {ex.Message}");
#endif
                return new UpdateReleaseInfo { Version = CurrentVersion };
            }
        }

        private static int CompareVersions(string version, string currentVersion) => Version.Parse(version).CompareTo(Version.Parse(currentVersion));
    }
}
