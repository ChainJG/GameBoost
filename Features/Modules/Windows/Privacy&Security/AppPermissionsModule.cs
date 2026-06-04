using GameBoost.Features.Modules.Base;
using GameBoost.Infrastructure.Registry;
using Microsoft.Win32;

namespace GameBoost.Features.Modules.Windows.Privacy_Security
{
    public class AppPermissionsModule : SystemTweakModuleBase
    {
        public override string Name => "App Permissions";

        public override object? RecommendedValue => ToggleType.Disabled;
        public override string RecommendationReason => "Location, cmaera, and account info access, is recommended to be disabled because most gaming-focused systems do not need apps constantly accessing the user’s location";

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
                    EnabledValue = "Allow",
                    DisabledValue = "Deny"
                };

                RegistryEdits[PermissionTypes.IndexOf(type)] = permissionRegistry;
            }
        }
    }
}
