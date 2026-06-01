using Microsoft.Win32;

namespace GameBoost.Infrastructure.Registry
{
    public class RegistryEditInfo
    {
        public RegistryHive Hive { get; set; }
        public string Path { get; init; } = string.Empty;
        public string Key { get; init; } = string.Empty;
        public bool Debug { get; init; } = false;
        public object? EnabledValue { get; init; }
        public object? DisabledValue { get; init; }
        public RegistryValueAction EnabledAction { get; init; } = RegistryValueAction.Set;
        public RegistryValueAction DisabledAction { get; init; } = RegistryValueAction.Set;

    }
}
