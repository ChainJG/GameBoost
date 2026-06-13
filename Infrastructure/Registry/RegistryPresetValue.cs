using Microsoft.Win32;

namespace GameBoost.Infrastructure.Registry
{
    public sealed class RegistryPresetValue
    {
        public required RegistryEditInfo Edit { get; init; }

        public required object Value { get; init; }
    }
}
