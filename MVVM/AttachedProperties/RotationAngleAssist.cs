using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GameBoost.MVVM.AttachedProperties
{
    public static class RotationAngleAssist
    {
        public static readonly DependencyProperty RotationAngleProperty =
            DependencyProperty.RegisterAttached(
                "RotationAngle",
                typeof(double),
                typeof(RotationAngleAssist),
                new PropertyMetadata(0d, OnRotationAngleChanged));

        public static double GetRotationAngle(DependencyObject element)
        {
            return (double)element.GetValue(RotationAngleProperty);
        }

        public static void SetRotationAngle(DependencyObject element, double value)
        {
            element.SetValue(RotationAngleProperty, value);
        }

        private static void OnRotationAngleChanged(
            DependencyObject dependencyObject,
            DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is not TextBlock textBlock)
                return;

            var angle = (double)e.NewValue;

            textBlock.LayoutTransform = new RotateTransform(angle);
        }
    }
}