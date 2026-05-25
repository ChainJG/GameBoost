using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace GameBoost.MVVM.Behaviors
{
    public static class PulsePopBehavior
    {
        private class PulseState
        {
            public Stopwatch Stopwatch { get; } = Stopwatch.StartNew();
            public ScaleTransform Transform { get; } = new();
            public bool Completed;
        }

        private static readonly Dictionary<UIElement, PulseState> States = new();

        #region IsEnabled

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsEnabled",
                typeof(bool),
                typeof(PulsePopBehavior),
                new PropertyMetadata(false, OnIsEnabledChanged));

        public static bool GetIsEnabled(DependencyObject obj) =>
            (bool)obj.GetValue(IsEnabledProperty);

        public static void SetIsEnabled(DependencyObject obj, bool value) =>
            obj.SetValue(IsEnabledProperty, value);

        #endregion

        #region Scale

        public static readonly DependencyProperty StartScaleProperty =
            DependencyProperty.RegisterAttached(
                "StartScale",
                typeof(double),
                typeof(PulsePopBehavior),
                new PropertyMetadata(0.7));

        public static double GetStartScale(DependencyObject obj) =>
            (double)obj.GetValue(StartScaleProperty);

        public static void SetStartScale(DependencyObject obj, double value) =>
            obj.SetValue(StartScaleProperty, value);

        #endregion

        #region Overshoot

        public static readonly DependencyProperty OvershootProperty =
            DependencyProperty.RegisterAttached(
                "Overshoot",
                typeof(double),
                typeof(PulsePopBehavior),
                new PropertyMetadata(1.15));

        public static double GetOvershoot(DependencyObject obj) =>
            (double)obj.GetValue(OvershootProperty);

        public static void SetOvershoot(DependencyObject obj, double value) =>
            obj.SetValue(OvershootProperty, value);

        #endregion

        #region Duration

        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.RegisterAttached(
                "Duration",
                typeof(double),
                typeof(PulsePopBehavior),
                new PropertyMetadata(0.35)); // seconds

        public static double GetDuration(DependencyObject obj) =>
            (double)obj.GetValue(DurationProperty);

        public static void SetDuration(DependencyObject obj, double value) =>
            obj.SetValue(DurationProperty, value);

        #endregion


        private static void AttachLoadedHandler(FrameworkElement element)
        {
            if (element.IsLoaded)
            {
                StartAnimation(element);
            }
            else
            {
                element.Loaded += Element_Loaded;
            }
        }

        private static void Element_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement element)
            {
                element.Loaded -= Element_Loaded; // important cleanup
                StartAnimation(element);
            }
        }

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not FrameworkElement element)
                return;

            if ((bool)e.NewValue)
            {
                AttachLoadedHandler(element);
            }
        }

        private static void StartAnimation(UIElement element)
        {
            if (States.ContainsKey(element))
                return;

            var state = new PulseState();

            element.RenderTransform = state.Transform;
            element.RenderTransformOrigin = new Point(0.5, 0.5);

            double start = GetStartScale(element);

            state.Transform.ScaleX = start;
            state.Transform.ScaleY = start;

            States[element] = state;

            CompositionTarget.Rendering += OnRender;
        }

        private static void Stop(UIElement element)
        {
            if (!States.Remove(element))
                return;

            if (States.Count == 0)
                CompositionTarget.Rendering -= OnRender;
        }

        private static void OnRender(object? sender, EventArgs e)
        {
            var toRemove = new List<UIElement>();

            foreach (var pair in States)
            {
                var element = pair.Key;
                var state = pair.Value;

                double duration = GetDuration(element);
                double overshoot = GetOvershoot(element);
                double startScale = GetStartScale(element);

                double t = state.Stopwatch.Elapsed.TotalSeconds / duration;

                if (t >= 1.0)
                {
                    state.Transform.ScaleX = 1.0;
                    state.Transform.ScaleY = 1.0;

                    state.Completed = true;
                    toRemove.Add(element);
                    continue;
                }

                // Ease-out-back style curve
                double value = EaseOutBack(t, startScale, overshoot);

                state.Transform.ScaleX = value;
                state.Transform.ScaleY = value;
            }

            foreach (var el in toRemove)
                States.Remove(el);

            if (States.Count == 0)
                CompositionTarget.Rendering -= OnRender;
        }

        // Smooth “pop” easing function
        private static double EaseOutBack(double t, double start, double overshoot)
        {
            double c1 = overshoot;
            double c3 = c1 + 1;

            double x = 1 - Math.Pow(1 - t, 3);

            return start + (1 - start) *
                (1 + c3 * Math.Pow(x - 1, 3) + c1 * Math.Pow(x - 1, 2));
        }
    }
}