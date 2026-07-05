using GameBoost.Application.Selection.Definitions;
using MaterialDesignThemes.Wpf;

namespace GameBoost.Features.Modules.GameModules.BlackOps7
{
    public static class BlackOps7FeatureFactory
    {
        public static FeatureDefinition CreateFeature()
        {
            return new FeatureDefinition
            {
                Title = "Black Ops 7",
                Description = "Apply custom Black Ops 7 configuration presets by editing the local game settings files safely",
                Icon = PackIconKind.ControllerClassic,
                SelectionType = SelectionType.Single,

                Actions =
                [
                    new ActionCardDefinition
                    {
                        Title = "Presets",
                        Icon = PackIconKind.TuneVariant,
                        Kind = ActionCardKind.ComboBox,
                        SortOrder = 100,
                        ObjectInputModule = new BlackOps7PresetModule(),
                    },

                    new ActionCardDefinition
                    {
                        Title = "Audio Music",
                        Icon = PackIconKind.Music,
                        Kind = ActionCardKind.Slider,

                        DoubleInputModule = new BlackOps7GroupedFloatSliderSettingModule(
                            displayName: "Audio Music",
                            settingNames:
                            [
                                "MusicVolume",
                                "LicensedMusicVolume",
                                "WarTracksVolume",
                                "CinematicVolume",
                                "TelescopeVolume"
                            ]),

                        Minimum = 0,
                        Maximum = 100,
                        TickFrequency = 5,
                        ValueSuffix = "%",
                    }
                ]
            };
        }
    }
}