 using GameBoost.Scripts.Services.Models;
using System.Diagnostics;

namespace GameBoost.Scripts.Services.Shell
{
    public static class ShellService
    {
        public static async Task<ProcessResult> RunAsync(
            ShellType shell,
            string command)
        {
            string shellName = shell switch
            {
                ShellType.Cmd => "cmd.exe",
                ShellType.PowerShell => "powershell",
                _ => throw new NotSupportedException()
            };

            string arguments = shell switch
            {
                ShellType.Cmd =>
                    $"/c \"{command}\"",
                ShellType.PowerShell =>
                    $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"",
                _ => ""
            };

            var psi = new ProcessStartInfo
            {
                FileName = shellName,
                Arguments = arguments,

                RedirectStandardOutput = true,
                RedirectStandardError = true,

                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process
            {
                StartInfo = psi
            };

            process.Start();

            string output =
                await process.StandardOutput.ReadToEndAsync();

            string error =
                await process.StandardError.ReadToEndAsync();

            await process.WaitForExitAsync();

            return new ProcessResult
            {
                Success = process.ExitCode == 0,
                ExitCode = process.ExitCode,
                Output = output,
                Error = error
            };
        }
    }
}