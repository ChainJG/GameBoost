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
        public SystemViewModel(string pageTitle)
        {
            PageTitle = pageTitle;

            var windowsSecurity = new SelectionFeatureViewModel
            {
                Title = "Windows Security",
                Description = "Real-time protection, and firewall settings is crucial for maintaining system security, preventing malware attacks, and ensuring data integrity",
                Icon = PackIconKind.Security,
            };
            windowsSecurity.AddActions(
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
            ]);

            var networkTroubleshoot = new SelectionFeatureViewModel
            {
                Title = "Network Troubleshoot",
                Description = "Running network troubleshooting scripts is essential for resolving connectivity issues, clearing outdated configurations, and improving network speed",
                Icon = PackIconKind.HelpNetwork,
                SelectionType = SelectionType.Single,
            };
            networkTroubleshoot.AddActions(
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
            ]);

            FeatureCards =
            [
                windowsSecurity,
                networkTroubleshoot,
            ];
        }
    }
}
