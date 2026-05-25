using GameBoost.Scripts.Services.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameBoost.Scripts.Services.Shell
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
