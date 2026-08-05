using GameBoost.Infrastructure.Installers.Models;
using GameBoost.Shared.Helpers;
using GameBoost.Shared.Results;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace GameBoost.Infrastructure.Installers.Services.Winget
{
    public sealed class WingetOutputParser(IProgress<ProgressResult>? progress)
    {
        private readonly IProgress<ProgressResult>? _progress = progress;

        #region Regex

        private static readonly Regex AnsiRegex = new(
            @"\x1B\[[0-?]*[ -/]*[@-~]",
            RegexOptions.Compiled);
        #endregion

        private readonly Lock _sync = new();
        private readonly Stopwatch _throttle = Stopwatch.StartNew();
        private readonly StringBuilder _recentOutput = new();

        private string _lastMessage = string.Empty;

        public void ProcessChunk(string chunk, AppInstallDefinition app)
        {
            if (string.IsNullOrWhiteSpace(chunk))
                return;

            if (IsOnlySpinnerNoise(chunk))
                return;

            lock (_sync)
            {
                _recentOutput.Append(chunk);

                if (_recentOutput.Length > 8000)
                    _recentOutput.Remove(0, _recentOutput.Length - 8000);

                var update = TryCreateProgressUpdate(_recentOutput.ToString(), app);

                if (update is null)
                    return;

                ReportCore(
                    progress,
                    update.Status);
            }
        }

        #region Report Methods
        public void Report(IProgress<ProgressResult>? progress, string message)
        {
            lock (_sync)
            {
                ReportCore(
                    progress,
                    message);
            }
        }

        private void ReportCore(IProgress<ProgressResult>? progress, string message)
        {
            if (progress is null)
                return;

            var messageChanged = !string.Equals(
                message,
                _lastMessage,
                StringComparison.OrdinalIgnoreCase);

            if (!messageChanged &&
                _throttle.ElapsedMilliseconds < 250)
            {
                return;
            }

            _lastMessage = message;
            _throttle.Restart();

            progress.Report(new ProgressResult(message, 0));
        }
        #endregion

        #region Try Get Methods
        private static ProgressResult? TryCreateProgressUpdate(string rawText, AppInstallDefinition app)
        {
            var text = CleanWingetOutput(rawText);

            if (string.IsNullOrWhiteSpace(text))
                return null;

            if (text.Contains("Successfully installed", StringComparison.OrdinalIgnoreCase))
            {
                return new ProgressResult(
                    $"{app.DisplayName} installed successfully");
            }

            if (text.Contains("0x800704c7", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("operation was canceled by the user", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("operation was cancelled by the user", StringComparison.OrdinalIgnoreCase))
            {
                return new ProgressResult(
                    "Administrator permission was cancelled");
            }

            if (text.Contains("The installer will request to run as administrator", StringComparison.OrdinalIgnoreCase) ||
                text.Contains("Expect a prompt", StringComparison.OrdinalIgnoreCase))
            {
                return new ProgressResult(
                    "Waiting for administrator approval...");
            }

            if (text.Contains("Starting package install", StringComparison.OrdinalIgnoreCase))
            {
                return new ProgressResult(
                    $"Starting {app.DisplayName} install...");
            }

            if (text.Contains("Successfully verified installer hash", StringComparison.OrdinalIgnoreCase))
            {
                return new ProgressResult(
                    $"{app.DisplayName} verified");
            }

            if (text.Contains("Downloading ", StringComparison.OrdinalIgnoreCase))
            {
                return new ProgressResult(
                    $"Downloading {app.DisplayName}...");
            }

            if (text.Contains("Found ", StringComparison.OrdinalIgnoreCase))
            {
                return new ProgressResult(
                    $"{app.DisplayName} found");
            }

            return null;
        }
        #endregion

        private static string CleanWingetOutput(string text)
        {
            text = AnsiRegex.Replace(text, string.Empty);

            return text
                .Replace('\r', '\n')
                .Replace("\u001b", string.Empty)

                // WinGet progress bar characters.
                .Replace("█", string.Empty)
                .Replace("▒", string.Empty)
                .Replace("▓", string.Empty)
                .Replace("░", string.Empty)

                // Mojibake fallback, in case some systems still output broken characters.
                .Replace("â–ˆ", string.Empty)
                .Replace("â–’", string.Empty)

                // Clean repeated spacing.
                .Trim();
        }
        private static bool IsOnlySpinnerNoise(string text)
        {
            var cleaned = text
                .Replace("-", string.Empty)
                .Replace("\\", string.Empty)
                .Replace("|", string.Empty)
                .Replace("/", string.Empty)
                .Replace("\r", string.Empty)
                .Replace("\n", string.Empty)
                .Trim();

            return string.IsNullOrWhiteSpace(cleaned);
        }

    }
}
