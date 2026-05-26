using GameBoost.MVVM.ViewModels.Shared;
using GameBoost.MVVM.ViewModels.Shared.Selection;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameBoost.MVVM.ViewModels
{
    public class SystemViewModel : SelectionViewModel
    {
        public SystemViewModel()
        {
            PageTitle = "System Optimistion";

            FeatureCards =
            [
                 new SelectionFeatureViewModel
                 {
                    Title = "Windows Security",
                    Description = "Real-time protection, and firewall settings is crucial for maintaining system security, preventing malware attacks, and ensuring data integrity",
                    Icon = PackIconKind.Security,
                    Actions =
                    [
                        new()
                        {
                            Title = "Real Time Protection",
                            Icon = PackIconKind.SmokeDetector,
                        },
                        new()
                        {
                            Title = "Firewall",
                            Icon = PackIconKind.Firebase,
                        },
                        new()
                        {
                            Title = "Core Memory Integrity",
                            Icon = PackIconKind.Memory,
                        }
                    ]
                 },
                 new SelectionFeatureViewModel
                 {
                    Title = "Network Troubleshoot",
                    Description = "Running network troubleshooting scripts is essential for resolving connectivity issues, clearing outdated configurations, and improving network speed",
                    Icon = PackIconKind.HelpNetwork,
                    Actions =
                    [
                        new()
                        {
                            Title = "Flush DNS",
                            Icon = PackIconKind.Dns,
                        },
                        new()
                        {
                            Title = "Release and Renew IP",
                            Icon = PackIconKind.IpNetwork,
                        },
                        new()
                        {
                            Title = "Clear ARP Cache",
                            Icon = PackIconKind.SilverwareClean,
                        },
                        new()
                        {
                            Title = "Reset Winsock",
                            Icon = PackIconKind.Restart,
                        },
                        new()
                        {
                            Title = "Restart Network Services",
                            Icon = PackIconKind.NetworkStrength4Warning,
                        },
                        new()
                        {
                            Title = "Restart Network Adapter",
                            Icon = PackIconKind.RouterNetwork,
                        },
                    ]
                 },
            ];
        }
    }
}
