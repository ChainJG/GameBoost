using GameBoost.Core.Services;
using GameBoost.Scripts.Services.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace GameBoost.Update
{
    public class UpdateReleaseInfo
    {
        public string Version { get; set; }
        public string DownloadUrl { get; set; }
        public string Notes { get; set; }
        public bool IsUpdateAvailable { get; set; }

        public async Task DownloadAndInstallAsync(
            IProgress<ProgressInfo> progress)
        {
            string tempFile = Path.Combine(
                Path.GetTempPath(),
                $"{AppServices.AppName}_Update.exe");

            using var client = new HttpClient();

            using var response = await client.GetAsync(
                DownloadUrl,
                HttpCompletionOption.ResponseHeadersRead);

            response.EnsureSuccessStatusCode();

            long? totalBytes = response.Content.Headers.ContentLength;

            await using var contentStream =
                await response.Content.ReadAsStreamAsync();

            await using var fileStream = new FileStream(
                tempFile,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                8192,
                true);

            var buffer = new byte[8192];

            long totalRead = 0;
            int bytesRead;

            while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead);

                totalRead += bytesRead;

                if (totalBytes.HasValue)
                {
                    int percent = (int)((totalRead * 100L) / totalBytes.Value);

                    progress.Report(
                        new ProgressInfo(
                            $"Downloading v{Version} update... {percent}%",
                            percent));
                }
            }

            progress.Report(
                new ProgressInfo(
                    "Launching installer...",
                    100));

            Process.Start(new ProcessStartInfo
            {
                FileName = tempFile,
                UseShellExecute = true
            });

            Environment.Exit(0);
        }
    }
}