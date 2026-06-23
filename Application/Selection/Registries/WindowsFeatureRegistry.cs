using GameBoost.Application.Selection.Definitions;
using GameBoost.Features.Modules.WindowsModules.ContextMenu;
using GameBoost.Features.Modules.WindowsModules.DirectXUserGlobal;
using GameBoost.Features.Modules.WindowsModules.Gaming;
using GameBoost.Features.Modules.WindowsModules.Gaming.GameFocus;
using GameBoost.Features.Modules.WindowsModules.KeyboardMouse.Keyboard;
using GameBoost.Features.Modules.WindowsModules.KeyboardMouse.Mouse;
using GameBoost.Features.Modules.WindowsModules.PowerOptions;
using GameBoost.Features.Modules.WindowsModules.PrivacySecurity;
using GameBoost.Features.Modules.WindowsModules.Security;
using GameBoost.Features.Modules.WindowsModules.Services.Builder;
using GameBoost.Features.Modules.WindowsModules.Taskbar;
using GameBoost.Features.Modules.WindowsModules.VisualEffects;
using GameBoost.Infrastructure.Registry.DirectXUserGlobal;
using MaterialDesignThemes.Wpf;

namespace GameBoost.Application.Selection.Registries
{
    public static class WindowsFeatureRegistry
    {
        public static IReadOnlyList<FeatureDefinition> GetFeatures()
        {
            return
            [
                WindowsSecurity(),
                Gaming(),
                VisualEffects(),
                Taskbar(),
                KeyboardAndMouse(),
                PrivacyAndSecurity(),
                ContextMenu(),
                PowerOptions(),
                GpuPreferencesGames(),
                WindowsServices(),
            ];
        }

        private static FeatureDefinition WindowsServices() => WindowsServicesFeatureFactory.CreateFeature();

        private static FeatureDefinition KeyboardAndMouse()
        {
            return new FeatureDefinition
            {
                Title = "Keyboard & Mouse",
                Description = "Adjust keyboard repeat behaviour, mouse speed, scrolling, double-click timing, and pointer precision settings",
                Icon = PackIconKind.Devices,
                Actions =
                [
                    new ActionCardDefinition
                    {
                        Title = "Enhance Pointer Precision",
                        Icon = PackIconKind.Mouse,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new EnhancePointerPrecisionModule(),
                    },

                    new ActionCardDefinition
                    {
                        Title = "Mouse Pointer Trails",
                        Icon = PackIconKind.Mouse,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new MousePointerTrailsModule(),
                    },

                    new ActionCardDefinition
                    {
                        Title = "Snap To Default Button",
                        Icon = PackIconKind.Mouse,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new SnapToDefaultButtonModule(),
                    },

                    new ActionCardDefinition
                    {
                        Title = "Mouse Pointer Speed",
                        Icon = PackIconKind.Mouse,
                        Kind = ActionCardKind.Slider,
                        DoubleInputModule = new MousePointerSpeedModule(),
                        Minimum = 1,
                        Maximum = 20,
                        TickFrequency = 1,
                    },

                    new ActionCardDefinition
                    {
                        Title = "Mouse Wheel Scroll Lines",
                        Icon = PackIconKind.Mouse,
                        Kind = ActionCardKind.Slider,
                        DoubleInputModule = new MouseWheelScrollLinesModule(),
                        Minimum = 1,
                        Maximum = 20,
                        TickFrequency = 1,
                    },

                    new ActionCardDefinition
                    {
                        Title = "Mouse Double-Click Speed",
                        Icon = PackIconKind.Mouse,
                        Kind = ActionCardKind.Slider,
                        DoubleInputModule = new MouseDoubleClickSpeedModule(),
                        Minimum = 200,
                        Maximum = 900,
                        TickFrequency = 50,
                        ValueSuffix = "ms",
                    },

                    new ActionCardDefinition
                    {
                        Title = "Keyboard Repeat Delay",
                        Icon = PackIconKind.Keyboard,
                        Kind = ActionCardKind.Slider,
                        DoubleInputModule = new KeyboardRepeatDelayModule(),
                        Minimum = 0,
                        Maximum = 3,
                        TickFrequency = 1,
                    },

                    new ActionCardDefinition
                    {
                        Title = "Keyboard Repeat Rate",
                        Icon = PackIconKind.Keyboard,
                        Kind = ActionCardKind.Slider,
                        DoubleInputModule = new KeyboardRepeatRateModule(),
                        Minimum = 0,
                        Maximum = 31,
                        TickFrequency = 1,
                    },


                ]
            };
        }

        private static FeatureDefinition GpuPreferencesGames()
        {
            var discoveredGames = DirectXUserGpuPreferences.GetDirectXUserGpuPreferencesGames();

            List<ActionCardDefinition> actions = [];

            foreach (var regGame in discoveredGames)
            {
                ActionCardDefinition actionCardDefinition = new ActionCardDefinition
                {
                    Title = regGame.Message,
                    Icon = PackIconKind.Controller,
                    Kind = ActionCardKind.Multipurpose,
                    ActionModule = new GpuPreferencesGameModule(regGame)
                };

                actions.Add(actionCardDefinition);
            }

            var gamePreferences = new FeatureDefinition
            {
                Title = "Application Preferences",
                Description = "Configures per-game Windows graphics preferences, GPU performance modes, and gaming optimizations for reduced latency and improved frame consistency.",
                Icon = PackIconKind.AppBadge,
                Actions = actions
            };

            return gamePreferences;
        }

        private static FeatureDefinition Gaming()
        {
            return new FeatureDefinition
            {
                Title = "Gaming",
                Description = "Optimise Windows 11 for gaming performance with Game Mode, VRR, DirectStorage, and controller optimisations.",
                Icon = PackIconKind.Gamepad,

                Actions =
                [
                    new ActionCardDefinition
                    {
                        Title = "Game Mode",
                        Icon = PackIconKind.Controller,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new GameModeModule()
                    },

                    new ActionCardDefinition
                    {
                        Title = "Hardware Acceleration GPU Scheduling",
                        Icon = PackIconKind.WindowRestore,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new HardwareAcceleratedGpuScheduling()
                    },

                    new ActionCardDefinition
                    {
                        Title = "Windowed Game Optimization",
                        Icon = PackIconKind.WindowRestore,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new WindowedGameOptimizationModule()
                    },

                    new ActionCardDefinition
                    {
                        Title = "Variable Refresh Rate (VRR)",
                        Icon = PackIconKind.MonitorShimmer,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new VariableRefreshRateModule()
                    },

                    new ActionCardDefinition
                    {
                        Title = "Gaming Focus",
                        Icon = PackIconKind.Target,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new GamingFocusProcessModule(),
                    },

                    new ActionCardDefinition
                    {
                        Title = "Foreground Priority Boost",
                        Icon = PackIconKind.PriorityHigh,
                        Kind = ActionCardKind.ComboBox,
                        ObjectInputModule = new Win32PrioritySeparationModule(),
                    }
                ]
            };
        }

        private static FeatureDefinition VisualEffects()
        {
            return new FeatureDefinition
            {
                Title = "Visual Effects",
                Description = "Manage Windows appearance and performance-focused visual effect settings.",
                Icon = PackIconKind.Theme,

                Actions =
                [
                    new ActionCardDefinition
                    {
                        Title = "Preference Options",
                        Icon = PackIconKind.VectorPolyline,
                        Kind = ActionCardKind.ComboBox,
                        ObjectInputModule = new PreferenceOptionsModule()
                    },

                    new ActionCardDefinition
                    {
                        Title = "System Theme Mode",
                        Icon = PackIconKind.Computer,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new SystemThemeModeModule()
                    },

                    new ActionCardDefinition
                    {
                        Title = "Transparency Effects",
                        Icon = PackIconKind.VectorUnion,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new TransparencyEffectModule()
                    }
                ]
            };
        }

        private static FeatureDefinition PowerOptions()
        {
            return new FeatureDefinition
            {
                Title = "Power Options",
                Description = "Adjust the active Windows power plan for performance or energy efficiency",
                Icon = PackIconKind.PowerStandby,

                Actions =
                [
                    new ActionCardDefinition
                    {
                        Title = "Current Power Plan",
                        Icon = PackIconKind.PowerPlugBattery,
                        Kind = ActionCardKind.ComboBox,
                        ObjectInputModule = new SetPowerPlanModule()
                    },
                    new ActionCardDefinition
                    {
                        Title = "PCIe Link State",
                        Icon = PackIconKind.Link,
                        Kind = ActionCardKind.ComboBox,
                        ObjectInputModule = new PciExpressLinkStatePowerManagementModule()
                    },
                    new ActionCardDefinition 
                    {
                        Title = "Processor Boost Mode",
                        Icon = PackIconKind.Chip,
                        Kind = ActionCardKind.ComboBox,
                        ObjectInputModule = new ProcessorBoostModeModule()
                    },
                    new ActionCardDefinition
                    {
                        Title = "Hibernate",
                        Icon = PackIconKind.Sleep,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new HibernateModule()
                    },
                    new ActionCardDefinition
                    {
                        Title = "Processor Minimum State",
                        Icon = PackIconKind.Chip,
                        Kind = ActionCardKind.Slider,
                        Minimum = 0,
                        Maximum = 100,
                        TickFrequency = 5,
                        ValueSuffix = "%",
                        DoubleInputModule = new ProcessorMinimumStatePowerManagementModule(),
                    },
                    new ActionCardDefinition
                    {
                        Title = "Processor Maximum State",
                        Icon = PackIconKind.Chip,
                        Kind = ActionCardKind.Slider,
                        Minimum = 0,
                        Maximum = 100,
                        TickFrequency = 5,
                        ValueSuffix = "%",
                        DoubleInputModule = new ProcessorMaximumStatePowerManagementModule(),
                    }
                ]
            };
        }

        private static FeatureDefinition ContextMenu()
        {
            return new FeatureDefinition
            {
                Title = "Context Menu",
                Description = "Add or remove GameBoost actions from the Windows context menu.",
                Icon = PackIconKind.MenuOpen,

                Actions =
                [
                    new ActionCardDefinition
                    {
                        Title = "End Task",
                        Icon = PackIconKind.ContainEnd,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new EndTaskTaskbarModule()
                    },

                    new ActionCardDefinition
                    {
                        Title = "Process Lookup",
                        Icon = PackIconKind.CommentProcessing,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new ProcessLookupModule()
                    }
                ]
            };
        }

        private static FeatureDefinition PrivacyAndSecurity()
        {
            return new FeatureDefinition
            {
                Title = "Privacy & Security",
                Description = "Reduce telemetry, background tracking, and unnecessary app permissions.",
                Icon = PackIconKind.ShieldEdit,

                Actions =
                [
                    new ActionCardDefinition
                    {
                        Title = "Telemetry",
                        Icon = PackIconKind.ClipboardPlus,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new TelemetryModule()
                    },

                    new ActionCardDefinition
                    {
                        Title = "Biometrics",
                        Icon = PackIconKind.HandWash,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new BiometricsModule()
                    },

                    new ActionCardDefinition
                    {
                        Title = "App Permissions",
                        Icon = PackIconKind.Widgets,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new AppPermissionsModule()
                    },

                    new ActionCardDefinition
                    {
                        Title = "User Tracking",
                        Icon = PackIconKind.MapMarkerRadius,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new UserTrackingModule()
                    },

                    new ActionCardDefinition
                    {
                        Title = "Feedback Requests",
                        Icon = PackIconKind.CardAccountDetails,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new FeedbackRequestsModule()
                    },

                    new ActionCardDefinition
                    {
                        Title = "Error Reporting",
                        Icon = PackIconKind.AlertRemove,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new ErrorReportingModule()
                    }
                ]
            };
        }

        private static FeatureDefinition Taskbar()
        {
            return new FeatureDefinition
            {
                Title = "Taskbar",
                Description = "Customize which Windows features appear on the taskbar.",
                Icon = PackIconKind.TableRow,

                Actions =
                [
                    new ActionCardDefinition
                    {
                        Title = "Search Bar",
                        Icon = PackIconKind.Search,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new SearchboxTaskbarModule()
                    },

                    new ActionCardDefinition
                    {
                        Title = "Task View",
                        Icon = PackIconKind.ImageFilterNone,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new TaskViewTaskbarModule()
                    },

                    new ActionCardDefinition
                    {
                        Title = "Widgets",
                        Icon = PackIconKind.Widgets,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new WidgetsTaskbarModule()
                    }
                ]
            };
        }

        private static FeatureDefinition WindowsSecurity()
        {
            return new FeatureDefinition
            {
                Title = "Windows Security",
                Description = "Manage selected Windows security features.",
                Icon = PackIconKind.Security,

                Actions =
                [
                    new ActionCardDefinition
                    {
                        Title = "Real Time Protection",
                        Icon = PackIconKind.SmokeDetector,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new RealTimeProtectionModule()
                    },

                    new ActionCardDefinition
                    {
                        Title = "Firewall",
                        Icon = PackIconKind.Firebase,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new FirewallModule()
                    },

                    new ActionCardDefinition
                    {
                        Title = "Core Isolation",
                        Icon = PackIconKind.Security,
                        Kind = ActionCardKind.Multipurpose,
                        ActionModule = new MemoryIntegrityModule(),
                    }
                ]
            };
        }
    }
}