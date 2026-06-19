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
    }
}