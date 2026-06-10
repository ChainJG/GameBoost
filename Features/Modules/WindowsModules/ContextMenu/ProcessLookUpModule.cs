using GameBoost.Core;
using GameBoost.Features.Modules.Base;
using GameBoost.Shared.Results;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;

namespace GameBoost.Features.Modules.WindowsModules.ContextMenu
{
    public sealed class ProcessLookUpModule : SystemTweakModuleBase
    {
        public override string Name => "Process LookUp";

        private const string MenuKeyPath = @"Software\Classes\*\shell\GameBoostProcessLookup";
        private const string CommandKeyPath = @"Software\Classes\*\shell\GameBoostProcessLookup\command";

        private const string MenuText = "Process look up";
        private const string ArgumentName = "--process-lookup";

        protected override ToggleType GetToggleStatus()
        {
            string? exePath = GameBoostServices.ExePath;

            if (string.IsNullOrWhiteSpace(exePath))
                return ToggleType.Disabled;

            using RegistryKey? menuKey = Registry.CurrentUser.OpenSubKey(MenuKeyPath);

            if (menuKey == null)
                return ToggleType.Disabled;

            using RegistryKey? commandKey = Registry.CurrentUser.OpenSubKey(CommandKeyPath);

            if (commandKey == null)
                return ToggleType.Disabled;

            string? actualMenuText = menuKey.GetValue("") as string;
            string? actualCommandValue = commandKey.GetValue("") as string;

            return string.Equals(actualMenuText, MenuText, StringComparison.Ordinal) && string.Equals(actualCommandValue, BuildCommandValue(exePath), StringComparison.Ordinal)
                   ? ToggleType.Enabled : ToggleType.Disabled;
        }

        public override async Task<ModuleResult> ExecuteAsync(CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var currentStatus = GetToggleStatus();
                var targetStatus = GetTargetStatus(currentStatus);

                if (targetStatus == ToggleType.Enabled)
                    return EnableProcessLookupContextMenu();
                else
                    return DisableProcessLookupContextMenu();
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error in ExecuteAsync: {ex.Message}");
#endif
                return ModuleResult.Failed(ex.Message);
            }
        }

        public ModuleResult EnableProcessLookupContextMenu()
        {
            string exePath = GameBoostServices.ExePath;

            if (string.IsNullOrWhiteSpace(exePath))
                return ModuleResult.Failed("The executable path is empty");

            if (!File.Exists(exePath))
                return ModuleResult.Failed("The executable could not be found");

            string commandValue = BuildCommandValue(exePath);
            string iconValue = BuildIconValue(exePath);

            using RegistryKey? menuKey = Registry.CurrentUser.CreateSubKey(MenuKeyPath);

            if (menuKey == null)
                return ModuleResult.Failed("Failed to create registry key");

            menuKey.SetValue("", MenuText, RegistryValueKind.String);
            menuKey.SetValue("Icon", iconValue, RegistryValueKind.String);

            using RegistryKey? commandKey = Registry.CurrentUser.CreateSubKey(CommandKeyPath);
            if (commandKey == null)
                return ModuleResult.Failed("Failed to create registry key");

            commandKey.SetValue("", commandValue, RegistryValueKind.String);

            return ModuleResult.Successful();
        }


        public static ModuleResult DisableProcessLookupContextMenu()
        {
            try
            {
                Registry.CurrentUser.DeleteSubKeyTree(MenuKeyPath, false);
                return ModuleResult.Successful();
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Error in DisableProcessLookupContextMenu: {ex.Message}");
#endif
                return ModuleResult.Failed(ex.Message);
            }
        }
        private static string BuildCommandValue(string exePath) =>  $"\"{exePath}\" {ArgumentName} \"%1\"";
        private static string BuildIconValue(string exePath)
        {
            string exeDirectory = Path.GetDirectoryName(exePath) ?? AppContext.BaseDirectory;

            string iconPath = Path.Combine(
                exeDirectory,
                "Assets",
                "Icons",
                "GameBoost.ico");

            if (File.Exists(iconPath))
                return iconPath;

            return $"{exePath},0";
        }
    }
}
