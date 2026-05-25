using System;
using System.Collections.Generic;
using System.Text;

namespace GameBoost.Infrastructure.Shell
{
    public static class PowerShellService
    {
        public static Task<ProcessResult> RunAsync(string command)
        {
            return ShellService.RunAsync(
                ShellType.PowerShell,
                command);
        }
    }
}
