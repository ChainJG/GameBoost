namespace GameBoost.Infrastructure.Registry
{
    public static class RegistryConstants
    {
        // DirectX GPU Preferences
        public const string DirectXUserGpuPreferences = @"Software\Microsoft\DirectX\UserGpuPreferences";
        public const string DirectXGlobalSettings = "DirectXUserGlobalSettings";

        // Visual Effects
        public const string VisualEffectsPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects";
        public const string WindowMetricsPath = @"Control Panel\Desktop\WindowMetrics";
        public const string Personalize = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        public const string DesktopPath = @"Control Panel\Desktop";
        public const string DWMPath = @"Software\Microsoft\Windows\DWM";
        public const string ExplorerAdvancedPath = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced";

        public const string GraphicsDriversPath = @"SYSTEM\CurrentControlSet\Control\GraphicsDrivers";
        public const string DataCollectionPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\DataCollection";

        // Services
        public const string BiometricsPath = @"SOFTWARE\Policies\Microsoft\Biometrics";
        public const string SearchPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Search";
    }
}
