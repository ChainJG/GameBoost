using System.Windows;

namespace GameBoost.MVVM.AttachedProperties
{
    /// <summary>
    /// Lets a ViewModel drive <see cref="VisualStateManager"/> states through a
    /// binding instead of code-behind event wiring. Bind <c>State</c> to a string
    /// (or enum — WPF converts it) matching a VisualState name declared on the
    /// element the property is attached to.
    /// </summary>
    public static class VisualStateAssist
    {
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.RegisterAttached(
                "State",
                typeof(string),
                typeof(VisualStateAssist),
                new PropertyMetadata(null, OnStateChanged));

        public static string? GetState(FrameworkElement element) =>
            (string?)element.GetValue(StateProperty);

        public static void SetState(FrameworkElement element, string? value) =>
            element.SetValue(StateProperty, value);

        private static void OnStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement element)
                return;

            if (e.NewValue is not string stateName || string.IsNullOrEmpty(stateName))
                return;

            if (element.IsLoaded)
            {
                VisualStateManager.GoToElementState(element, stateName, useTransitions: true);
                return;
            }

            // Apply the initial state without transitions once the element is ready.
            element.Loaded += ApplyInitialStateOnLoaded;

            static void ApplyInitialStateOnLoaded(object sender, RoutedEventArgs args)
            {
                var loadedElement = (FrameworkElement)sender;
                loadedElement.Loaded -= ApplyInitialStateOnLoaded;

                var currentState = GetState(loadedElement);

                if (!string.IsNullOrEmpty(currentState))
                    VisualStateManager.GoToElementState(loadedElement, currentState, useTransitions: false);
            }
        }
    }
}
