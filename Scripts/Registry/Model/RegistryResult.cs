using Microsoft.Win32;

namespace GameBoost.Scripts.Registry.Model
{
    public class RegistryResult
    {
        public bool Success { get; init; }
        public string? Message { get; init; }
        public RegistryKey? key { get; init; }
    }
}
