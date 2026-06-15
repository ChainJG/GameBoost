using GameBoost.Application.Selection.Definitions;
using GameBoost.Infrastructure.MicrosoftApps.Catalog;
using GameBoost.Infrastructure.MicrosoftApps.Models;
using MaterialDesignThemes.Wpf;

namespace GameBoost.Features.Modules.SystemModules.MicrosoftApps
{
    public static class MicrosoftAppFeatureFactory
    {
        public static FeatureDefinition CreateInstallFeature()
        {
            return new FeatureDefinition
            {
                Title = "Microsoft App Manager",
                Description = "Install or uninstall built in Microsoft Store apps when their package still exists on the system",
                Icon = PackIconKind.MicrosoftWindows,
                Actions = [.. MicrosoftAppCatalog.CommonApps.Select(app => CreateInstallAction(app))]
            };
        }
        private static ActionCardDefinition CreateInstallAction(MicrosoftAppDefinition app)
        {
            return new ActionCardDefinition
            {
                Title = $"{app.DisplayName}",
                Icon = app.Icon,
                Kind = ActionCardKind.Multipurpose,
                ActionModule = new MicrosoftAppToggleModule(app),
            };
        }
    }
}