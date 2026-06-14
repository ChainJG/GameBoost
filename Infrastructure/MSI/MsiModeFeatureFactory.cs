using GameBoost.Application.Selection.Definitions;
using GameBoost.Features.Modules.SystemModules.MsiMode;
using GameBoost.Features.Modules.SystemModules.MSIMode;
using MaterialDesignThemes.Wpf;

namespace GameBoost.Infrastructure.MSI
{
    public static class MsiModeFeatureFactory
    {
        public static FeatureDefinition CreateFeature()
        {
            var devices = PNPDeviceIDs.GetAllMsiModeTargets();

            return new FeatureDefinition
            {
                Title = "MSI Mode",
                Description = "Enable Message Signaled Interrupts for supported GPU, USB controller, and network adapter devices to improve device interrupt handling and responsiveness.",
                Icon = PackIconKind.ExpansionCard,

                Actions = [.. devices.Select(CreateAction)]
            };
        }

        private static ActionCardDefinition CreateAction(
            MsiModeDeviceInfo device)
        {
            return new ActionCardDefinition
            {
                Title = GetActionTitle(device),
                Icon = GetIcon(device.Category),
                Kind = ActionCardKind.Multipurpose,
                ActionModule = new MsiModeModule(device),
            };
        }

        private static string GetActionTitle(
            MsiModeDeviceInfo device)
        {
            return device.Category switch
            {
                MsiModeDeviceCategory.VideoController =>
                    $"GPU MSI Mode - {device.Name}",

                MsiModeDeviceCategory.UsbController =>
                    $"USB MSI Mode - {device.Name}",

                MsiModeDeviceCategory.NetworkAdapter =>
                    $"Network MSI Mode - {device.Name}",

                _ =>
                    $"MSI Mode - {device.Name}"
            };
        }

        private static PackIconKind GetIcon(
            MsiModeDeviceCategory category)
        {
            return category switch
            {
                MsiModeDeviceCategory.VideoController =>
                    PackIconKind.GraphicsProcessingUnit,

                MsiModeDeviceCategory.UsbController =>
                    PackIconKind.UsbPort,

                MsiModeDeviceCategory.NetworkAdapter =>
                    PackIconKind.Ethernet,

                _ =>
                    PackIconKind.ExpansionCard
            };
        }

        private static string GetToolTip(
            MsiModeDeviceInfo device)
        {
            return
                $"Enables MSI Mode for {device.DisplayName}. " +
                "This can improve interrupt handling and device responsiveness, but it may not benefit every driver. A restart is required.";
        }
    }
}