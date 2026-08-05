using GameBoost.Application.Modules;
using GameBoost.Application.Selection.Definitions;
using GameBoost.MVVM.ViewModels.Shared.Selection.Cards;
using GameBoost.MVVM.ViewModels.Shared.Selection.Cards.Actions;

namespace GameBoost.Application.Selection.Builders
{
    public static class SelectionFeatureBuilder
    {
        public static SelectionFeatureViewModel Build(FeatureDefinition definition)
        {
            var feature = new SelectionFeatureViewModel
            {
                Title = definition.Title,
                Description = definition.Description,
                Icon = definition.Icon,
                SelectionType = definition.SelectionType
            };

            feature.AddActions(
                definition.Actions
                    .OrderBy(action => action.SortOrder ?? int.MaxValue)
                    .ThenBy(action => action.Title, StringComparer.OrdinalIgnoreCase)
                    .Select(BuildAction));

            return feature;
        }

        public static IReadOnlyList<SelectionFeatureViewModel> BuildMany(
            IEnumerable<FeatureDefinition> definitions) =>
            [
                .. definitions
                    .OrderBy(definition => definition.Title, StringComparer.OrdinalIgnoreCase)
                    .Select(Build)
            ];


        private static SelectionActionCardViewModelBase BuildAction(ActionCardDefinition definition)
        {
            return definition.Kind switch
            {
                ActionCardKind.Multipurpose => BuildMultipurposeAction(definition),
                ActionCardKind.ComboBox => BuildComboBoxAction(definition),
                ActionCardKind.Slider => BuildSliderAction(definition),
                _ => throw new InvalidOperationException($"Unknown action card kind: {definition.Kind}")
            };
        }

        private static MultipurposeActionCardViewModel BuildMultipurposeAction(ActionCardDefinition definition)
        {
            if (definition.ActionModule is null)
                throw new InvalidOperationException($"{definition.Title} requires an IActionModule");

            var action = OptimizationAction.ForModule(
                definition.Title,
                definition.Icon,
                definition.ActionModule);

            return new MultipurposeActionCardViewModel(action)
            {
                InfoToolTip = definition.InfoToolTip,
            };
        }
        private static ComboBoxActionCardViewModel BuildComboBoxAction(ActionCardDefinition definition)
        {
            if (definition.ObjectInputModule is null)
                throw new InvalidOperationException(
                    $"{definition.Title} requires an IInputActionModule<object>");

            var action = OptimizationAction.ForObjectInput(
                definition.Title,
                definition.Icon,
                definition.ObjectInputModule);

            return new ComboBoxActionCardViewModel(action)
            {
                InfoToolTip = definition.InfoToolTip
            };
        }
        private static SliderActionCardViewModel BuildSliderAction(ActionCardDefinition definition)
        {
            if (definition.DoubleInputModule is null)
                throw new InvalidOperationException(
                    $"{definition.Title} requires an IInputActionModule<double>.");

            var action = OptimizationAction.ForDoubleInput(
                definition.Title,
                definition.Icon,
                definition.DoubleInputModule);

            return new SliderActionCardViewModel(action)
            {
                InfoToolTip = definition.InfoToolTip,
                Minimum = definition.Minimum,
                Maximum = definition.Maximum,
                TickFrequency = definition.TickFrequency,
                ValueSuffix = definition.ValueSuffix
            };
        }

    }
}
