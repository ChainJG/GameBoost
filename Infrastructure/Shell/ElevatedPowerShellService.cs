namespace GameBoost.Infrastructure.Shell
{
    public static class ElevatedPowerShellService
    {
        public static ProcessResult RunPowerShellAsAdmin(string command)
        {
            string args =
                $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"";

            return AdminExecutionService.RunAsAdminAsync(
                "powershell",
                args).Result;
        }
    }
}
