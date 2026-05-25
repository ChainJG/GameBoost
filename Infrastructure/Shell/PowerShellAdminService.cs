namespace GameBoost.Infrastructure.Shell
{
    public static class PowerShellAdminService
    {
        public static ProcessResult RunPowerShellAsAdmin(string command)
        {
            string args =
                $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"";

            return AdminExecutionService.RunAsAdmin(
                "powershell",
                args);
        }
    }
}
