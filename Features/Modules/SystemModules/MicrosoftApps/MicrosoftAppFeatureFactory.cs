using GameBoost.Application.Selection.Definitions;
using GameBoost.Infrastructure.MicrosoftApps.Models;
using GameBoost.Infrastructure.MicrosoftApps.Services;
using MaterialDesignThemes.Wpf;

namespace GameBoost.Features.Modules.SystemModules.MicrosoftApps
{
    public static class MicrosoftAppFeatureFactory
    {
        public static FeatureDefinition CreateInstallFeature()
        {
            return new FeatureDefinition
            {
                Title = "Microsoft App Install",
                Description = "Install or update built in Microsoft Store apps when their package still exists on the system",
                Icon = PackIconKind.MicrosoftWindows,
                Actions = [.. MicrosoftAppCatalog.CommonApps.Select(app => CreateInstallAction(app))]
            };
        }
        private static ActionCardDefinition CreateInstallAction(MicrosoftAppDefinition app)
        {
            return new ActionCardDefinition
            {
                Title = $"Install {app.DisplayName}",
                Icon = app.Icon,
                Kind = ActionCardKind.Multipurpose,
                ActionModule = new MicrosoftAppInstallModule(app),
            };
        }


    }
}