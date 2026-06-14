using System.ComponentModel;
using System.Diagnostics;

namespace GameBoost.Infrastructure.Shell
{
    public static class AdminExecutionService
    {
        private const int UacCancelledErrorCode = 1223;

        public static async Task<ProcessResult> RunAsAdminAsync(ShellType shell, string command)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(command))
                    throw new ArgumentException("Command cannot be empty.", nameof(command));

                string fileName = shell switch
                {
                    ShellType.Cmd => "cmd.exe",
                    ShellType.PowerShell => "powershell",
                    _ => throw new NotSupportedException()
                };

                var arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"";

                var psi = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,

                    Verb = "runas",
                    UseShellExecute = true,

                    CreateNoWindow = false
                };

                using var process = Process.Start(psi);

                if (process is null)
                    return ProcessResult.Failed(-1, "Failed to start elevated process.");

                await process.WaitForExitAsync();

                return process.ExitCode == 0
                    ? ProcessResult.Successful(process.ExitCode)
                    : ProcessResult.Failed(
                        process.ExitCode,
                        $"Elevated process failed with exit code {process.ExitCode}.");
            }
            catch (Win32Exception ex) when (ex.NativeErrorCode == UacCancelledErrorCode)
            {
                return ProcessResult.Failed(-1, "Administrator permission was declined.");
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error in RunAsAdminAsync: {ex.Message}");
#endif
                return ProcessResult.Failed(-1, ex.Message);
            }
        }
    }
}
