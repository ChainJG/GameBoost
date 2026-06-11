using System;
namespace GameBoost.Infrastructure.Shell
{
    public static class PowerShellService
    {
        public static Task<ProcessResult> RunAsync(string command, CancellationToken token = default)
        {
            return ShellService.RunAsync(
                ShellType.PowerShell,
                command,
                token);
        }
    }
}
