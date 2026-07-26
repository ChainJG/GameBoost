using MaterialDesignThemes.Wpf;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace GameBoost.MVVM.Behaviors
{
    public static class HoldToClickBehavior
    {
        // Internal tracking property for the stopwatch
        private static readonly DependencyProperty StopwatchProperty =
            DependencyProperty.RegisterAttached("Stopwatch", typeof(Stopwatch), typeof(HoldToClickBehavior), new PropertyMetadata(null));

        private static Button _activeButton;

        // --- NEW CONFIGURATION PROPERTY ---
        // Allows you to change the duration easily from XAML. Defaults to 3 seconds if not specified.
        public static readonly DependencyProperty HoldDurationSecondsProperty =
            DependencyProperty.RegisterAttached(
                "HoldDurationSeconds",
                typeof(double),
                typeof(HoldToClickBehavior),
                new PropertyMetadata(1.0));

        public static double GetHoldDurationSeconds(DependencyObject obj) => (double)obj.GetValue(HoldDurationSecondsProperty);
        public static void SetHoldDurationSeconds(DependencyObject obj, double value) => obj.SetValue(HoldDurationSecondsProperty, value);


        // Master switch property
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsEnabled",
                typeof(bool),
                typeof(HoldToClickBehavior),
                new PropertyMetadata(false, OnIsEnabledChanged));

        public static bool GetIsEnabled(DependencyObject obj) => (bool)obj.GetValue(IsEnabledProperty);
        public static void SetIsEnabled(DependencyObject obj, bool value) => obj.SetValue(IsEnabledProperty, value);

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Button button)
            {
                if ((bool)e.NewValue)
                {
                    button.SetValue(StopwatchProperty, new Stopwatch());
                    button.PreviewMouseDown += Button_PreviewMouseDown;
                    button.PreviewMouseUp += Button_ResetHold;
                    button.MouseLeave += Button_ResetHold;
                }
                else
                {
                    button.PreviewMouseDown -= Button_PreviewMouseDown;
                    button.PreviewMouseUp -= Button_ResetHold;
                    button.MouseLeave -= Button_ResetHold;
                    button.ClearValue(StopwatchProperty);
                }
            }
        }

        private static void Button_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Button button && button.Template != null)
            {
                var progressRing = button.Template.FindName("ProgressRing", button) as ProgressBar;
                var stopwatch = button.GetValue(StopwatchProperty) as Stopwatch;
                var icon = button.Template.FindName("Icon", button) as PackIcon;
                var txtCountdown = button.Template.FindName("CountdownText", button) as TextBlock;

                if (progressRing != null && stopwatch != null && icon != null && txtCountdown != null)
                {
                    Mouse.Capture(button);
                    _activeButton = button;

                    // Get the custom duration configured for this specific button
                    double targetDuration = GetHoldDurationSeconds(button);

                    stopwatch.Restart();

                    // Display the dynamic starting ceiling integer (e.g., "3s" or "5s")
                    txtCountdown.Text = $"{Math.Ceiling(targetDuration)}s";
                    txtCountdown.Visibility = Visibility.Visible;

                    CompositionTarget.Rendering += UpdateCountdownText;

                    DoubleAnimation holdAnimation = new()
                    {
                        From = 0,
                        To = 100,
                        Duration = TimeSpan.FromSeconds(targetDuration),
                        FillBehavior = FillBehavior.HoldEnd
                    };

                    DoubleAnimation fadeOutIconAnimation = new()
                    {
                        To = 0,
                        Duration = TimeSpan.FromSeconds(0.2),
                        FillBehavior = FillBehavior.HoldEnd
                    };

                    progressRing.BeginAnimation(ProgressBar.ValueProperty, holdAnimation);
                    icon.BeginAnimation(UIElement.OpacityProperty, fadeOutIconAnimation);
                }
            }
        }

        private static void UpdateCountdownText(object sender, EventArgs e)
        {
            if (_activeButton == null || _activeButton.Template == null) return;

            var stopwatch = _activeButton.GetValue(StopwatchProperty) as Stopwatch;
            var txtCountdown = _activeButton.Template.FindName("CountdownText", _activeButton) as TextBlock;

            if (stopwatch != null && txtCountdown != null && stopwatch.IsRunning)
            {
                double targetDuration = GetHoldDurationSeconds(_activeButton);
                double elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
                double remainingSeconds = Math.Max(0, targetDuration - elapsedSeconds);

                // If the dynamic limit is hit, trigger execution
                if (elapsedSeconds >= targetDuration)
                {
                    stopwatch.Stop();

                    if (_activeButton.Command != null && _activeButton.Command.CanExecute(_activeButton.CommandParameter))
                        _activeButton.Command.Execute(_activeButton.CommandParameter);

                    CleanupVisuals(_activeButton);
                    return;
                }

                if (remainingSeconds > 0)
                {
                    txtCountdown.Text = $"{remainingSeconds:F1}s";
                }
            }
        }

        private static void Button_ResetHold(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                var stopwatch = button.GetValue(StopwatchProperty) as Stopwatch;
                double targetDuration = GetHoldDurationSeconds(button);

                if (stopwatch != null && stopwatch.IsRunning && stopwatch.Elapsed.TotalSeconds < targetDuration)
                {
                    CleanupVisuals(button);

                    if (e is MouseButtonEventArgs mouseArgs)
                        mouseArgs.Handled = true;
                }
                else
                {
                    CleanupVisuals(button);
                }
            }
        }

        private static void CleanupVisuals(Button button)
        {
            CompositionTarget.Rendering -= UpdateCountdownText;

            if (_activeButton == button)
                _activeButton = null;

            if (Mouse.Captured == button)
                Mouse.Capture(null);

            if (button.Template != null)
            {
                var progressRing = button.Template.FindName("ProgressRing", button) as ProgressBar;
                var icon = button.Template.FindName("Icon", button) as PackIcon;
                var txtCountdown = button.Template.FindName("CountdownText", button) as TextBlock;
                var stopwatch = button.GetValue(StopwatchProperty) as Stopwatch;

                stopwatch?.Stop();

                if (progressRing != null)
                {
                    progressRing.BeginAnimation(ProgressBar.ValueProperty, null);
                    progressRing.Value = 0;
                }

                if (icon != null)
                {
                    icon.BeginAnimation(UIElement.OpacityProperty, null);
                    icon.Opacity = 1;
                }

                if (txtCountdown != null)
                {
                    txtCountdown.Visibility = Visibility.Collapsed;
                    txtCountdown.Text = "";
                }
            }
        }
    }
}