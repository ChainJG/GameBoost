using GameBoost.Application.Selection.Definitions;
using GameBoost.Features.Modules.GameModules.BlackOps7;

namespace GameBoost.Application.Selection.Registries
{
    public static class GamesFeatureRegistry
    {
        public static IReadOnlyList<FeatureDefinition> GetFeatures()
        {
            var features = new List<FeatureDefinition>
            {
                BlackOps7PersetModule(),
            };

            return features;
        }

        public static FeatureDefinition BlackOps7PersetModule() => BlackOps7FeatureFactory.CreateFeature();
    }
}
