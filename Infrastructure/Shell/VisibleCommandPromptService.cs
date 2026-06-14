using System.Diagnostics;

namespace GameBoost.Infrastructure.Shell
{
    public static class VisibleCommandPromptService
    {
        public static Process StartCommand(ShellType shell, string command, string title, bool runAsAdministrator = true)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentException("Command cannot be empty.", nameof(command));


            string fileName = shell switch
            {
                ShellType.Cmd => "cmd.exe",
                ShellType.PowerShell => "powershell",
                _ => throw new NotSupportedException()
            };

            var fullCommand = BuildCommand(command, title);

            var processStartInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = fullCommand,
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Normal
            };

            if (runAsAdministrator)
                processStartInfo.Verb = "runas";

            return Process.Start(processStartInfo)
                   ?? throw new InvalidOperationException("Failed to start Command Prompt");
        }

        private static string BuildCommand(string command, string title, bool keepOpen = true)
        {
            var prefix = string.IsNullOrWhiteSpace(title)
                ? string.Empty
                : $"title {EscapeCommandTitle(title)} && ";

            var suffix = keepOpen
                ? " && echo. && echo Command finished. You can close this window. && pause"
                : string.Empty;

            var cmdSwitch = keepOpen
                ? "/k"
                : "/c";

            return $"{cmdSwitch} \"{prefix}{command}{suffix}\"";
        }

        private static string EscapeCommandTitle(
            string title)
        {
            return title
                .Replace("&", string.Empty)
                .Replace("|", string.Empty)
                .Replace("<", string.Empty)
                .Replace(">", string.Empty)
                .Replace("\"", string.Empty);
        }
    }
}