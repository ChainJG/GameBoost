using System.Diagnostics;

namespace GameBoost.Infrastructure.Shell
{
    public static class ShellService
    {
        public static async Task<ProcessResult> RunAsync(ShellType shell, string command, CancellationToken token = default)
        {
            try
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

                var outputTask = process.StandardOutput.ReadToEndAsync(token);
                var errorTask = process.StandardError.ReadToEndAsync(token);

                await process.WaitForExitAsync(token);

                var output = await outputTask;
                var error = await errorTask;

                return new ProcessResult
                {
                    Success = process.ExitCode == 0,
                    ExitCode = process.ExitCode,
                    Output = output,
                    Error = GetFailureMessage(error, output, process.ExitCode)
                };
            }
            catch (Exception ex)
            {
                return ProcessResult.Failed(-1, ex.Message);
            }
        }

        public static string GetFailureMessage(
            string error,
            string output,
            int exitCode)
        {
            if (!string.IsNullOrWhiteSpace(error))
                return error.Trim();

            if (!string.IsNullOrWhiteSpace(output))
                return output.Trim();

            return $"Exited with code {exitCode}";
        }
    }
}