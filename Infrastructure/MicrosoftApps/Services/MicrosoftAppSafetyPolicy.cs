namespace GameBoost.Infrastructure.MicrosoftApps.Services
{
    public sealed class MicrosoftAppSafetyPolicy
    {
        public static readonly string[] ProtectedPackagePrefixes =
        [
            "Microsoft.WindowsStore",
            "Microsoft.StorePurchaseApp",
            "Microsoft.DesktopAppInstaller",
            "Microsoft.Windows.ShellExperienceHost",
            "Microsoft.Windows.StartMenuExperienceHost",
            "Microsoft.Windows.Search",
            "Microsoft.AAD.BrokerPlugin",
            "Microsoft.AccountsControl",
            "Microsoft.LockApp",
            "Microsoft.Windows.CloudExperienceHost",
            "Microsoft.Windows.ContentDeliveryManager",
            "Microsoft.Windows.OOBENetworkConnectionFlow",
            "MicrosoftWindows.Client.CBS"
        ];

        public static bool IsProtectedPackage(string packageName)
        {
            return ProtectedPackagePrefixes.Any(prefix =>
                packageName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
        }
    }
}
