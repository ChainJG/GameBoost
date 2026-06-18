using GameBoost.Application.Selection.Definitions;
using GameBoost.Features.Modules.WindowsModules.Services.Catalog;
using MaterialDesignThemes.Wpf;

namespace GameBoost.Features.Modules.WindowsModules.Services.Builder
{
    public static class WindowsServicesFeatureFactory
    {
        public static FeatureDefinition CreateFeature()
        {
            return new FeatureDefinition
            {
                Title = "Windows Services",
                Description = "Manage selected Windows background services and startup modes for cleaner gaming-focused systems.",
                Icon = PackIconKind.Cogs,
                Actions = [.. WindowsServiceCatalog.Services
                    .Select(service => new ActionCardDefinition
                    {
                        Title = service.DisplayName,
                        Icon = service.Icon,
                        Kind = ActionCardKind.ComboBox,
                        ObjectInputModule = new WindowsServiceStartupModule(service),
                        InfoToolTip = $"{service.Description} Recommended: {service.RecommendedStartupMode}. {service.RecommendationReason}"
                    })]
            };
        }
    }
}