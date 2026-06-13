using GameBoost.Infrastructure.Registry;

namespace GameBoost.Features.Modules.WindowsModules.VisualEffects.Options
{
    public sealed class PreferenceOptions
    {
        public required PreferenceOption Option { get; init; }

        public required string DisplayName { get; init; }

        public required string Description { get; init; }

        public required IReadOnlyList<RegistryPresetValue> RegistryValues { get; init; }
    }

    public enum PreferenceOption
    {
        Appearance,
        Performance
    }
}
