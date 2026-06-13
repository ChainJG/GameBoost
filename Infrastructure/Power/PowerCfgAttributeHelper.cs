using GameBoost.Infrastructure.Shell;
using GameBoost.Shared.Results;

namespace GameBoost.Infrastructure.Power
{
    public static class PowerCfgAttributeHelper
    {
        public static Task<ModuleResult> ShowSettingAsync(
            string subGroup,
            string setting,
            CancellationToken token)
        {
            return RunAttributeCommandAsync(
                subGroup,
                setting,
                "-ATTRIB_HIDE",
                token);
        }

        public static Task<ModuleResult> HideSettingAsync(
            string subGroup,
            string setting,
            CancellationToken token)
        {
            return RunAttributeCommandAsync(
                subGroup,
                setting,
                "+ATTRIB_HIDE",
                token);
        }

        private static async Task<ModuleResult> RunAttributeCommandAsync(
            string subGroup,
            string setting,
            string attributeCommand,
            CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var command =
                $"powercfg -attributes {subGroup} {setting} {attributeCommand}";

            var result = await ShellService.RunAsync(
                ShellType.Cmd,
                command,
                token);

            if (!result.Success)
            {
                var message = !string.IsNullOrWhiteSpace(result.Error)
                    ? result.Error
                    : result.Output;

                return ModuleResult.Failed(message);
            }

            return ModuleResult.Successful(result.Output);
        }
    }
}