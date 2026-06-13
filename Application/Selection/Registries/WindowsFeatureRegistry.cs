using GameBoost.Application.Selection.Definitions;
using GameBoost.Features.Modules.WindowsModules.ContextMenu;
using GameBoost.Features.Modules.WindowsModules.DirectXUserGlobal;
using GameBoost.Features.Modules.WindowsModules.Gaming;
using GameBoost.Features.Modules.WindowsModules.PowerPerformance;
using GameBoost.Features.Modules.WindowsModules.PrivacySecurity;
using GameBoost.Features.Modules.WindowsModules.Security;
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
                PrivacyAndSecurity(),
                ContextMenu(),
                PowerPlan(),
                GpuPreferencesGames(),
            ];
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

        private static FeatureDefinition PowerPlan()
        {
            return new FeatureDefinition
            {
                Title = "Power & Performance",
                Description = "Adjust the active Windows power plan for performance or energy efficiency.",
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
                        TickFrequency = 1,
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
                        TickFrequency = 1,
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
                    }
                ]
            };
        }
    }
}