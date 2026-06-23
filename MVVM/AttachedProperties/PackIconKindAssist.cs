using MaterialDesignThemes.Wpf;
using System.Windows;

namespace GameBoost.MVVM.AttachedProperties
{
    public static class PackIconKindAssist
    {
        public static readonly DependencyProperty KindProperty =
            DependencyProperty.RegisterAttached(
                "Kind",
                typeof(PackIconKind),
                typeof(PackIconKindAssist),
                new PropertyMetadata(
                    PackIconKind.QuestionMark,
                    OnKindChanged));

        public static readonly DependencyProperty SizeProperty =
            DependencyProperty.RegisterAttached(
                "Size",
                typeof(double),
                typeof(PackIconKindAssist),
                new PropertyMetadata(
                    double.NaN,
                    OnSizeChanged));

        public static PackIconKind GetKind(DependencyObject element)
        {
            return (PackIconKind)element.GetValue(KindProperty);
        }

        public static void SetKind(
            DependencyObject element,
            PackIconKind value)
        {
            element.SetValue(KindProperty, value);
        }

        public static double GetSize(DependencyObject element)
        {
            return (double)element.GetValue(SizeProperty);
        }

        public static void SetSize(
            DependencyObject element,
            double value)
        {
            element.SetValue(SizeProperty, value);
        }

        private static void OnKindChanged(
            DependencyObject element,
            DependencyPropertyChangedEventArgs args)
        {
            if (element is not PackIcon packIcon)
                return;

            if (args.NewValue is not PackIconKind iconKind)
                return;

            packIcon.Kind = iconKind;
        }

        private static void OnSizeChanged(
            DependencyObject element,
            DependencyPropertyChangedEventArgs args)
        {
            if (element is not PackIcon packIcon)
                return;

            if (args.NewValue is not double iconSize)
                return;

            if (double.IsNaN(iconSize))
            {
                packIcon.ClearValue(FrameworkElement.WidthProperty);
                packIcon.ClearValue(FrameworkElement.HeightProperty);
                return;
            }

            iconSize = Math.Max(0, iconSize);

            packIcon.Width = iconSize;
            packIcon.Height = iconSize;
        }
    }
}