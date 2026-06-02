using Microsoft.Win32;
using System.Diagnostics;

namespace GameBoost.Infrastructure.Registry.DirectXUserGlobal
{
    public class DirectXUserGlobalHelper
    {
        public static RegistryResult GetDirectXUserGlobalFlag(string flagName)
        {
            try
            {
                RegistryResult result = RegistryHelper.GetValue(new RegistryEditInfo
                {
                    Hive = RegistryHive.CurrentUser,
                    Path = RegistryConstants.DirectXUserGpuPreferences,
                    Key = RegistryConstants.DirectXGlobalSettings,
                });

                if (result.Value is string str && str.Contains($"{flagName}="))
                {
                    var match = System.Text.RegularExpressions.Regex.Match(str, $"{flagName}=(\\d)");

                    if (match.Success)
                    return RegistryResult.Successful("Successful retrieval", match.Groups[1].Value);
                }

                return RegistryResult.Failed("Flag not found");
            }
            catch (Exception ex)
            {
                return RegistryResult.Failed(ex.Message);
            }
        }
        public static RegistryResult SetDirectXUserGlobalFlag(string flagName, int value)
        {
            try
            {
                var editData = new RegistryEditInfo
                {
                    Hive = RegistryHive.CurrentUser,
                    Path = RegistryConstants.DirectXUserGpuPreferences,
                    Key = RegistryConstants.DirectXGlobalSettings,
                };

                var result = RegistryHelper.GetValue(editData);
                string settings = result.Value as string ?? string.Empty;

                // Parse existing settings into dictionary
                var settingsDict = settings
                    .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Split('='))
                    .Where(pair => pair.Length == 2)
                    .ToDictionary(pair => pair[0], pair => pair[1]);

                // Update or add the flag
                settingsDict[flagName] = value.ToString();

                // Reconstruct the settings string
                string updatedSettings = string.Join(";",
                    settingsDict.Select(kvp => $"{kvp.Key}={kvp.Value}")) + ";";

                // Write back to registry
                return RegistryHelper.SetValue(editData, updatedSettings);
            }
            catch (Exception ex)
            {
                return RegistryResult.Failed(ex.Message);
            }
        }
    }
}
