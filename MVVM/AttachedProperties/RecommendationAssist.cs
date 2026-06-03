using System.Windows;

namespace GameBoost.MVVM.AttachedProperties
{
    public static class RecommendationAssist
    {
        public static readonly DependencyProperty IsRecommendedProperty =
            DependencyProperty.RegisterAttached(
                "IsRecommended",
                typeof(bool),
                typeof(RecommendationAssist),
                new PropertyMetadata(false));

        public static bool GetIsRecommended(DependencyObject element)
        {
            return (bool)element.GetValue(IsRecommendedProperty);
        }

        public static void SetIsRecommended(DependencyObject element, bool value)
        {
            element.SetValue(IsRecommendedProperty, value);
        }

        public static readonly DependencyProperty RecommendationToolTipProperty =
            DependencyProperty.RegisterAttached(
                "RecommendationToolTip",
                typeof(string),
                typeof(RecommendationAssist),
                new PropertyMetadata(string.Empty));

        public static string GetRecommendationToolTip(DependencyObject element)
        {
            return (string)element.GetValue(RecommendationToolTipProperty);
        }

        public static void SetRecommendationToolTip(DependencyObject element, string value)
        {
            element.SetValue(RecommendationToolTipProperty, value);
        }
    }
}