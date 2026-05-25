using Microsoft.Win32;

namespace GameBoost.Scripts.Registry.Model
{
    public class RegistryEditInfo
    {
        public RegistryHive Hive { get; set; }
        public string Path { get; init; } = string.Empty;
        public string Key { get; init; } = string.Empty;
        public Object EnabledValue { get; init; } = default;
        public Object DisabledValue { get; init; } = default;
    }
}
