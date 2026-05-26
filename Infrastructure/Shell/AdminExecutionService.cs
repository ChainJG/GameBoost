using GameBoost.Application;
using GameBoost.Shared.Results;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows;

namespace GameBoost.Infrastructure.Shell
{
    public static class AdminExecutionService
    {
        public static async Task<ProcessResult> RunAsAdminAsync(string fileName, string arguments, bool createNoWindow = true)
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = arguments,

                    Verb = "runas",
                    CreateNoWindow = createNoWindow,
                    UseShellExecute = true
                };

                using var process = Process.Start(psi);

                if (process is null)
                    ProcessResult.Failed(-1);

                await process.WaitForExitAsync();

                return ProcessResult.Successful(process.ExitCode, process.StandardOutput.ReadToEnd());
            }
            catch (Exception ex)
            {
                return ProcessResult.Failed(-1, ex.Message);
            }
        }
    }
}
