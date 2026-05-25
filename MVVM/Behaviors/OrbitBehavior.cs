using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace GameBoost.MVVM.Behaviors
{
    public static class OrbitBehavior
    {
        private class OrbitState
        {
            public Stopwatch Stopwatch { get; } = Stopwatch.StartNew();
            public TranslateTransform Transform { get; } = new();
        }

        private static readonly Dictionary<UIElement, OrbitState> States = new();

        #region IsOrbiting

        public static readonly DependencyProperty IsOrbitingProperty =
            DependencyProperty.RegisterAttached(
                "IsOrbiting",
                typeof(bool),
                typeof(OrbitBehavior),
                new PropertyMetadata(false, OnIsOrbitingChanged));

        public static bool GetIsOrbiting(DependencyObject obj) =>
            (bool)obj.GetValue(IsOrbitingProperty);

        public static void SetIsOrbiting(DependencyObject obj, bool value) =>
            obj.SetValue(IsOrbitingProperty, value);

        #endregion

        #region Radius

        public static readonly DependencyProperty RadiusProperty =
            DependencyProperty.RegisterAttached(
                "Radius",
                typeof(double),
                typeof(OrbitBehavior),
                new PropertyMetadata(50.0));

        public static double GetRadius(DependencyObject obj) =>
            (double)obj.GetValue(RadiusProperty);

        public static void SetRadius(DependencyObject obj, double value) =>
            obj.SetValue(RadiusProperty, value);

        #endregion

        #region Speed

        public static readonly DependencyProperty SpeedProperty =
            DependencyProperty.RegisterAttached(
                "Speed",
                typeof(double),
                typeof(OrbitBehavior),
                new PropertyMetadata(1.0));

        public static double GetSpeed(DependencyObject obj) =>
            (double)obj.GetValue(SpeedProperty);

        public static void SetSpeed(DependencyObject obj, double value) =>
            obj.SetValue(SpeedProperty, value);

        #endregion

        private static void OnIsOrbitingChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is not UIElement element)
                return;

            bool enabled = (bool)e.NewValue;

            if (enabled)
            {
                StartOrbit(element);
            }
            else
            {
                StopOrbit(element);
            }
        }

        private static void StartOrbit(UIElement element)
        {
            if (States.ContainsKey(element))
                return;

            var state = new OrbitState();

            element.RenderTransform = state.Transform;
            element.RenderTransformOrigin = new Point(0.5, 0.5);

            States[element] = state;

            CompositionTarget.Rendering += OnRendering;
        }

        private static void StopOrbit(UIElement element)
        {
            if (!States.Remove(element))
                return;

            if (States.Count == 0)
            {
                CompositionTarget.Rendering -= OnRendering;
            }
        }

        private static void OnRendering(object? sender, EventArgs e)
        {
            foreach (var pair in States)
            {
                var element = pair.Key;
                var state = pair.Value;

                double radius = GetRadius(element);
                double speed = GetSpeed(element);

                double time = state.Stopwatch.Elapsed.TotalSeconds;

                double angle = time * speed;

                double x = Math.Cos(angle) * radius;
                double y = Math.Sin(angle) * radius;

                state.Transform.X = x;
                state.Transform.Y = y;
            }
        }
    }
}