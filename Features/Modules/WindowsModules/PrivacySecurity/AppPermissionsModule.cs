using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using Microsoft.Win32;

namespace GameBoost.Features.Modules.WindowsModules.PrivacySecurity
{
    public class AppPermissionsModule : SystemTweakModuleBase
    {
        public override string Name => "App Permissions";

        public override RecommendationPriority RecommendationPriority => RecommendationPriority.Medium;
        public override object? RecommendedValue => ToggleType.Disabled;
        public override string RecommendationReason => "App permissions are recommended to be disabled by default on gaming-focused systems because most games do not need access to location, camera, calls, account activity, or personal app data";

        public override RegistryEditInfo[] RegistryEdits { get; } = [];

        private static readonly string[] PermissionTypes = 
        [
            "location",
            "activity",
            "chat",
            "email",
            "musicLibrary",
            "phoneCall",
            "phoneCallHistory",
            "webcam",
        ];

        public AppPermissionsModule()
        {
            RegistryEdits = new RegistryEditInfo[PermissionTypes.Length];

            foreach(var type in PermissionTypes)
            {
                var permissionRegistry = new RegistryEditInfo()
                {
                    Hive = RegistryHive.CurrentUser,
                    Path = $@"Software\Microsoft\Windows\CurrentVersion\CapabilityAccessManager\ConsentStore\{type}",
                    Key = "Value",
                    Kind = RegistryValueKind.String,
                    EnabledValue = "Allow",
                    DisabledValue = "Deny"
                };

                RegistryEdits[PermissionTypes.IndexOf(type)] = permissionRegistry;
            }
        }
    }
}
