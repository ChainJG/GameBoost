using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace GameBoost.MVVM.Behaviors
{
    public static class ProgressBarAnimationBehavior
    {
        public static readonly DependencyProperty SmoothValueProperty =
            DependencyProperty.RegisterAttached(
                "SmoothValue",
                typeof(double),
                typeof(ProgressBarAnimationBehavior),
                new PropertyMetadata(0.0, OnSmoothValueChanged));

        public static void SetSmoothValue(DependencyObject obj, double value)
            => obj.SetValue(SmoothValueProperty, value);

        public static double GetSmoothValue(DependencyObject obj)
            => (double)obj.GetValue(SmoothValueProperty);

        private static void OnSmoothValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ProgressBar pb) return;

            double newValue = (double)e.NewValue;

            var animation = new DoubleAnimation
            {
                To = newValue,
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };

            pb.BeginAnimation(ProgressBar.ValueProperty, animation);
        }
    }
}
