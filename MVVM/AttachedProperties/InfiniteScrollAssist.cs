using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GameBoost.MVVM.AttachedProperties
{
    public static class InfiniteScrollAssist
    {
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsEnabled",
                typeof(bool),
                typeof(InfiniteScrollAssist),
                new PropertyMetadata(false, OnIsEnabledChanged));

        public static bool GetIsEnabled(DependencyObject element)
        {
            return (bool)element.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject element, bool value)
        {
            element.SetValue(IsEnabledProperty, value);
        }

        public static readonly DependencyProperty LoadMoreCommandProperty =
            DependencyProperty.RegisterAttached(
                "LoadMoreCommand",
                typeof(ICommand),
                typeof(InfiniteScrollAssist),
                new PropertyMetadata(null));

        public static ICommand? GetLoadMoreCommand(DependencyObject element)
        {
            return (ICommand?)element.GetValue(LoadMoreCommandProperty);
        }

        public static void SetLoadMoreCommand(DependencyObject element, ICommand? value)
        {
            element.SetValue(LoadMoreCommandProperty, value);
        }

        public static readonly DependencyProperty ScrollThresholdProperty =
            DependencyProperty.RegisterAttached(
                "ScrollThreshold",
                typeof(double),
                typeof(InfiniteScrollAssist),
                new PropertyMetadata(120d));

        public static double GetScrollThreshold(DependencyObject element)
        {
            return (double)element.GetValue(ScrollThresholdProperty);
        }

        public static void SetScrollThreshold(DependencyObject element, double value)
        {
            element.SetValue(ScrollThresholdProperty, value);
        }

        private static void OnIsEnabledChanged(
            DependencyObject element,
            DependencyPropertyChangedEventArgs args)
        {
            if (element is not ScrollViewer scrollViewer)
                return;

            if ((bool)args.NewValue)
                scrollViewer.ScrollChanged += OnScrollChanged;
            else
                scrollViewer.ScrollChanged -= OnScrollChanged;
        }

        private static void OnScrollChanged(
            object sender,
            ScrollChangedEventArgs args)
        {
            if (sender is not ScrollViewer scrollViewer)
                return;

            if (!GetIsEnabled(scrollViewer))
                return;

            // Prevent auto-loading multiple batches from ExtentHeight changes.
            // Only trigger when the user actually scrolls down.
            if (args.VerticalChange <= 0)
                return;

            var remainingScroll = scrollViewer.ExtentHeight -
                                  scrollViewer.VerticalOffset -
                                  scrollViewer.ViewportHeight;

            if (remainingScroll > GetScrollThreshold(scrollViewer))
                return;

            var command = GetLoadMoreCommand(scrollViewer);

            if (command?.CanExecute(null) == true)
                command.Execute(null);
        }
    }
}