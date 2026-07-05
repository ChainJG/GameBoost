using GameBoost.Infrastructure.GameConfigs.BlackOps7.Settings;

namespace GameBoost.Infrastructure.GameConfigs.BlackOps7.Catalog
{
    public static class BlackOps7PresetCatalog
    {
        public static IReadOnlyList<BlackOps7PresetDefinition> GetPresets()
        {
            return
            [
                CompetitiveFps(),
                Balanced(),
                Quality()
            ];
        }

        private static List<BlackOps7SettingChange> StaticSettings()
        {
            return
            [
                // Display
                new() { SettingName = "AmdAntilag2", Value = "true" },
                new() { SettingName = "ResolutionMultiplier", Value = "100" },
                new() { SettingName = "VSyncInMenu", Value = "disabled" },
                new() { SettingName = "VSync", Value = "disabled" },
                new() { SettingName = "NvidiaReflex", Value = "Enabled + boost" },
                new() { SettingName = "FocusedMode", Value = "false" },
                new() { SettingName = "DisplayGamma", Value = "BT709_sRGB" },
                new() { SettingName = "DisplayMode", Value = "Fullscreen" },
                new() { SettingName = "PreferredDisplayMode", Value = "Fullscreen" },
                new() { SettingName = "DxrMode", Value = "Off" }, // DirectX Raytracing

                // Gameplay
                new() { SettingName = "CapFps", Value = "true" },
                new() { SettingName = "MaxFpsInGame", Value = "300" },
                new() { SettingName = "MaxFpsInMenu", Value = "60" },
                new() { SettingName = "MaxFpsOutOfFocus", Value = "30" },

                // Interface 
                new() { SettingName = "SkipIntro", Value = "true" },
                new() { SettingName = "SkipSeasonIntroVideo", Value = "true" },
                new() { SettingName = "SkipSeasonVideo", Value = "true" },
                new() { SettingName = "UseOSCursors", Value = "true" },
                new() { SettingName = "ViewedSplashScreen", Value = "true" },
                new() { SettingName = "ShowFPSCounter", Value = "false" },

                // Display
                new() { SettingName = "VSync", Value = "disabled" },
                new() { SettingName = "VSyncInMenu", Value = "disabled" },


                // Graphics
                new() { SettingName = "GPUUploadHeaps", Value = "true" },
                new() { SettingName = "VRS", Value = "true" },
                new() { SettingName = "SubdivisionLevel", Value = "3" },

                // Motion Blur
                new() { SettingName = "EnableVelocityBasedBlur", Value = "false" },
                new() { SettingName = "DepthOfField", Value = "false" },
                new() { SettingName = "DepthOfFieldQuality", Value = "Low" },
            ];
        }

        public static BlackOps7PresetDefinition CompetitiveFps()
        {
            var changes = StaticSettings();

            changes.AddRange(
            [
                // Gameplay
                new() { SettingName = "ShowBlood", Value = "false" },
                new() { SettingName = "BloodLimitInterval", Value = "2000" },
                new() { SettingName = "ShowBrass", Value = "false" },
                new() { SettingName = "CorpseLimit", Value = "0" },
                new() { SettingName = "BulletImpacts", Value = "false" },

                // Graphics
                new() { SettingName = "BulletImpacts", Value ="false"},
                new() { SettingName = "CorpsesCullingThreshold", Value ="0.500000"},
                new() { SettingName = "ReflectionProbeRelighting", Value ="1"},
                new() { SettingName = "ScreenSpaceShadowQuality", Value ="Off"},
                new() { SettingName = "SSRQuality", Value ="Off"},
                new() { SettingName = "StaticSunshadowClipmapResolution", Value ="1024"},
                new() { SettingName = "Tessellation", Value ="0_Off"},
                new() { SettingName = "TextureFilter", Value ="aniso 2x"},
                new() { SettingName = "TextureQuality", Value = "2" },
                new() { SettingName = "UiQuality", Value = "Auto" },
                new() { SettingName = "WorldStreamingQuality", Value = "Low" },
                new() { SettingName = "DeferredPhysics", Value = "Low Quality" },
                new() { SettingName = "AmbientLightingQuality", Value = "Off" },
                new() { SettingName = "ModelQuality", Value = "Low Quality" },
                new() { SettingName = "ParticleQuality", Value = "very low" },
                new() { SettingName = "ShadowQuality", Value = "Very_Low" },
                new() { SettingName = "ReflectionProbeHalfResolution", Value = "true" },
                new() { SettingName = "ShaderQuality", Value = "Low" },
                new() { SettingName = "VirtualTexturingMemoryMode", Value = "Extra Large" },
                new() { SettingName = "VolumetricQuality", Value = "QUALITY_LOW" },
                new() { SettingName = "WaterCausticsMode", Value = "Off" },
                new() { SettingName = "WaterWaveWetness", Value = "false" },
                new() { SettingName = "WeatherGridVolumesQuality", Value = "Off" },
            ]);

            return new BlackOps7PresetDefinition
            {
                Id = "competitive_fps",
                DisplayName = "Competitive FPS",
                Description = "Prioritises visibility, reduced latency, and stable performance",
                Changes = changes
            };
        }

        public static BlackOps7PresetDefinition Balanced()
        {
            var changes = StaticSettings();

            changes.AddRange(
            [
                new() { SettingName = "TextureFilter", Value = "aniso 4x" },
                new() { SettingName = "TextureQuality", Value = "1" },
                new() { SettingName = "ModelQuality", Value = "Medium Quality" },
            ]);

            return new BlackOps7PresetDefinition
            {
                Id = "balanced",
                DisplayName = "Balanced",
                Description = "Balanced visuals and performance for most gaming systems",
                Changes = changes
            };
        }

        public static BlackOps7PresetDefinition Quality()
        {
            var changes = StaticSettings();

            changes.AddRange(
            [
                new() { SettingName = "TextureFilter", Value = "aniso 8x" },
                new() { SettingName = "TextureQuality", Value = "1" },
                new() { SettingName = "ModelQuality", Value = "Medium Quality" },
            ]);

            return new BlackOps7PresetDefinition
            {
                Id = "quality",
                DisplayName = "Quality",
                Description = "Prioritises image quality while keeping sensible competitive settings",
                Changes = changes
            };
        }
    }
}