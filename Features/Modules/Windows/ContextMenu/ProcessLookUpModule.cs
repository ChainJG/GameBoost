using GameBoost.Features.Modules.Base;
using GameBoost.Shared.Results;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;

namespace GameBoost.Features.Modules.Windows.ContextMenu
{
    public sealed class ProcessLookUpModule : SystemTweakModuleBase
    {
        public override string Name => "Process LookUp";

        private const string MenuKeyPath = @"Software\Classes\*\shell\GameBoostProcessLookup";
        private const string CommandKeyPath = @"Software\Classes\*\shell\GameBoostProcessLookup\command";

        private const string MenuText = "Process look up";
        private const string ArgumentName = "--process-lookup";

        private readonly string exePath = Environment.ProcessPath!;

        protected override ToggleType GetToggleStatus()
        {
            if (string.IsNullOrWhiteSpace(exePath))
                return ToggleType.Disabled;

            string expectedCommandValue = BuildCommandValue();

            using RegistryKey? menuKey = Registry.CurrentUser.OpenSubKey(MenuKeyPath);
            if (menuKey == null)
                return ToggleType.Disabled;

            using RegistryKey? commandKey = Registry.CurrentUser.OpenSubKey(CommandKeyPath);
            if (commandKey == null)
                return ToggleType.Disabled;

            string? actualMenuText = menuKey.GetValue("") as string;
            string? actualIcon = menuKey.GetValue("Icon") as string;
            string? actualCommandValue = commandKey.GetValue("") as string;

            return string.Equals(actualMenuText, MenuText, StringComparison.Ordinal) &&
                   string.Equals(actualIcon, exePath, StringComparison.OrdinalIgnoreCase) &&
                   string.Equals(actualCommandValue, expectedCommandValue, StringComparison.OrdinalIgnoreCase) 
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

        private string BuildCommandValue() => 
             $"\"{exePath}\" {ArgumentName} \"%1\"";


        public ModuleResult EnableProcessLookupContextMenu()
        {
            if (string.IsNullOrWhiteSpace(exePath))
                throw new ArgumentException("The GameBoost executable path is empty.", nameof(exePath));

            if (!File.Exists(exePath))
                throw new FileNotFoundException("The GameBoost executable could not be found.", exePath);

            string commandValue = BuildCommandValue();

            using RegistryKey? menuKey = Registry.CurrentUser.CreateSubKey(MenuKeyPath);
            if (menuKey == null)
                return ModuleResult.Failed("Failed to create registry key");

            menuKey.SetValue("", MenuText, RegistryValueKind.String);
            menuKey.SetValue("Icon", exePath, RegistryValueKind.String);

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
    }
}
