using GameBoost.Core.Debugger;
using GameBoost.Infrastructure.Installers.Models;
using GameBoost.Infrastructure.Installers.Services.Winget;
using GameBoost.Shared.Helpers.ProcessHelpers;
using GameBoost.Shared.Results;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace GameBoost.Infrastructure.Installers.Services
{
    public sealed class WingetInstallProvider
    {
        public static async Task<ModuleResult> InstallAsync(AppInstallDefinition app, IProgress<ProgressResult>? progress, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(app.WingetId))
            {
                return ModuleResult.Failed($"Missing WinGet package id for {app.DisplayName}");
            }

            using var process = new Process();

            process.StartInfo = CreateStartInfo(app);
            process.EnableRaisingEvents = true;

            var outputBuilder = new StringBuilder();
            var errorBuilder = new StringBuilder();
            var progressParser = new WingetOutputParser(progress);

            try
            {
                progressParser.Report(progress, $"Starting install: {app.DisplayName}");

                if (!process.Start())
                {
                    return ModuleResult.Failed($"Failed to start winget process for {app.DisplayName}");
                }

                var outputTask = ReadStreamLiveAsync(app, process.StandardOutput, outputBuilder, progressParser, progress, token);
                var errorTask = ReadStreamLiveAsync(app, process.StandardError, errorBuilder, progressParser, progress, token);

                await process.WaitForExitAsync(token).ConfigureAwait(false);

                await Task.WhenAll(outputTask, errorTask).ConfigureAwait(false);

                var output = outputBuilder.ToString();
                var error = errorBuilder.ToString();

                var success = process.ExitCode == 0;

                return success ? ModuleResult.Successful($"Installed {app.DisplayName}") : ModuleResult.Failed($"Failed to install {app.DisplayName}");
            }
            catch (OperationCanceledException)
            {
                ProcessHelper.TryEndProcess(process);

                return ModuleResult.Cancel($"Cancelled {app.DisplayName} install");
            }
            catch (Exception ex)
            {
                GameBoostDebug.Error($"Failed installation: {ex.Message}");

                return ModuleResult.Failed($"Installation Failed {app.DisplayName}");
            }
        }

        private static async Task ReadStreamLiveAsync(AppInstallDefinition app, StreamReader reader, StringBuilder output, WingetOutputParser progressParser, IProgress<ProgressResult>? progress, CancellationToken token)
        {
            var buffer = new char[512];

            while (true)
            {
                token.ThrowIfCancellationRequested();

                var read = await reader.ReadAsync(
                    buffer.AsMemory(0, buffer.Length),
                    token).ConfigureAwait(false);

                if (read <= 0)
                    break;

                var chunk = new string(buffer, 0, read);

                output.Append(chunk);

                progressParser.ProcessChunk(chunk, app);
            }
        }

        private static ProcessStartInfo CreateStartInfo(AppInstallDefinition app)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "winget",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            startInfo.ArgumentList.Add("install");
            startInfo.ArgumentList.Add("--id");
            startInfo.ArgumentList.Add(app.WingetId!);
            startInfo.ArgumentList.Add("-e");
            startInfo.ArgumentList.Add("--source");
            startInfo.ArgumentList.Add(app.WingetSource ?? "winget");
            startInfo.ArgumentList.Add("--accept-source-agreements");
            startInfo.ArgumentList.Add("--accept-package-agreements");
            startInfo.ArgumentList.Add("--disable-interactivity");

            return startInfo;
        }
    }
}