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
                    .OrderBy(action => action.Title, StringComparer.OrdinalIgnoreCase)
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

            return new MultipurposeActionCardViewModel
            {
                Title = definition.Title,
                Icon = definition.Icon,
                InfoToolTip = definition.InfoToolTip,
                Module = definition.ActionModule
            };
        }
        private static ComboBoxActionCardViewModel BuildComboBoxAction(ActionCardDefinition definition)
        {
            if (definition.ObjectInputModule is null)
                throw new InvalidOperationException(
                    $"{definition.Title} requires an IInputActionModule<object>.");

            return new ComboBoxActionCardViewModel
            {
                Title = definition.Title,
                Icon = definition.Icon,
                InfoToolTip = definition.InfoToolTip,
                Module = definition.ObjectInputModule
            };
        }
        private static SliderActionCardViewModel BuildSliderAction(ActionCardDefinition definition)
        {
            if (definition.DoubleInputModule is null)
                throw new InvalidOperationException(
                    $"{definition.Title} requires an IInputActionModule<double>.");

            return new SliderActionCardViewModel
            {
                Title = definition.Title,
                Icon = definition.Icon,
                InfoToolTip = definition.InfoToolTip,
                Module = definition.DoubleInputModule,
                Minimum = definition.Minimum,
                Maximum = definition.Maximum,
                TickFrequency = definition.TickFrequency,
                ValueSuffix = definition.ValueSuffix
            };
        }

    }
}
