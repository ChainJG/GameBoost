using GameBoost.Application;
using GameBoost.Core;
using GameBoost.Infrastructure.Http;
using GameBoost.Shared.Helpers;
using GameBoost.Shared.Results;
using System.Diagnostics;
using System.IO;
using System.Net.Http;

namespace GameBoost.Features.Updates
{
    public class UpdateReleaseInfo
    {
        public string Version { get; set; }
        public string DownloadUrl { get; set; }
        public string Notes { get; set; }
        public bool IsUpdateAvailable { get; set; }

        public async Task DownloadAndInstallAsync(IProgress<ProgressResult>? progress = default, CancellationToken cancellationToken = default)
        {
            string tempFile = Path.Combine(
                Path.GetTempPath(),
                $"{GameBoostContext.AppName}_Update_{Version}.exe");

            using (var response = await HttpClientProvider.Client.GetAsync(
                DownloadUrl,
                HttpCompletionOption.ResponseHeadersRead,
                cancellationToken))
            {
                response.EnsureSuccessStatusCode();

                long? totalBytes = response.Content.Headers.ContentLength;

                await using var contentStream =
                    await response.Content.ReadAsStreamAsync(cancellationToken);

                await using var fileStream = new FileStream(
                    tempFile,
                    FileMode.Create,
                    FileAccess.Write,
                    FileShare.None,
                    8192,
                    useAsync: true);

                var buffer = new byte[8192];

                long totalRead = 0;
                int bytesRead;

                while ((bytesRead = await contentStream.ReadAsync(buffer, cancellationToken)) > 0)
                {
                    await fileStream.WriteAsync(
                        buffer.AsMemory(0, bytesRead),
                        cancellationToken);

                    totalRead += bytesRead;

                    if (totalBytes.HasValue && totalBytes.Value > 0)
                    {
                        //int percent = (int)((totalRead * 100L) / totalBytes.Value);
                        var percent = MathHelper.ToPercentageInt(totalRead, totalBytes.Value);

                        progress?.Report(
                            new ProgressResult(
                                $"Downloading update... {percent}%",
                                percent));
                    }
                }

                await fileStream.FlushAsync(cancellationToken);
            }

            progress?.Report(
                new ProgressResult(
                    "Launching installer...",
                    100));

            Process.Start(new ProcessStartInfo
            {
                FileName = tempFile,
                UseShellExecute = true,
                WorkingDirectory = Path.GetDirectoryName(tempFile)
            });

            System.Windows.Application.Current.Shutdown();
        }
    }
}