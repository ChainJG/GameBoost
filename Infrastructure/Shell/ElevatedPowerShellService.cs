namespace GameBoost.Infrastructure.Shell
{
    public static class ElevatedPowerShellService
    {
        public static async Task<ProcessResult> RunPowerShellAsAdmin(string command)
        {
            string args =
                $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"";

            return await AdminExecutionService.RunAsAdminAsync(
                "powershell",
                args);
        }
    }
}
