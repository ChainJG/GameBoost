using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace GameBoost.MVVM.AttachedProperties
{
    public static class InteractiveCardAssist
    {
        #region IsEnabled

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsEnabled",
                typeof(bool),
                typeof(InteractiveCardAssist),
                new PropertyMetadata(false, OnIsEnabledChanged));

        public static bool GetIsEnabled(DependencyObject element)
        {
            return (bool)element.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(DependencyObject element, bool value)
        {
            element.SetValue(IsEnabledProperty, value);
        }

        private static void OnIsEnabledChanged(
            DependencyObject element,
            DependencyPropertyChangedEventArgs args)
        {
            if (element is not Border border)
                return;

            border.MouseEnter -= OnMouseEnter;
            border.MouseLeave -= OnMouseLeave;
            border.PreviewMouseLeftButtonDown -= OnPreviewMouseLeftButtonDown;
            border.PreviewMouseLeftButtonUp -= OnPreviewMouseLeftButtonUp;
            border.IsEnabledChanged -= OnBorderIsEnabledChanged;

            if ((bool)args.NewValue)
            {
                border.MouseEnter += OnMouseEnter;
                border.MouseLeave += OnMouseLeave;
                border.PreviewMouseLeftButtonDown += OnPreviewMouseLeftButtonDown;
                border.PreviewMouseLeftButtonUp += OnPreviewMouseLeftButtonUp;
                border.IsEnabledChanged += OnBorderIsEnabledChanged;
            }

            ApplyState(border);
        }

        #endregion

        #region IsSelected

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.RegisterAttached(
                "IsSelected",
                typeof(bool),
                typeof(InteractiveCardAssist),
                new PropertyMetadata(false, OnVisualStatePropertyChanged));

        public static bool GetIsSelected(DependencyObject element)
        {
            return (bool)element.GetValue(IsSelectedProperty);
        }

        public static void SetIsSelected(DependencyObject element, bool value)
        {
            element.SetValue(IsSelectedProperty, value);
        }

        #endregion

        #region IsPressed

        private static readonly DependencyProperty IsPressedProperty =
            DependencyProperty.RegisterAttached(
                "IsPressed",
                typeof(bool),
                typeof(InteractiveCardAssist),
                new PropertyMetadata(false, OnVisualStatePropertyChanged));

        private static bool GetIsPressed(DependencyObject element)
        {
            return (bool)element.GetValue(IsPressedProperty);
        }

        private static void SetIsPressed(DependencyObject element, bool value)
        {
            element.SetValue(IsPressedProperty, value);
        }

        #endregion

        #region Background Brushes

        public static readonly DependencyProperty NormalBackgroundProperty =
            DependencyProperty.RegisterAttached(
                "NormalBackground",
                typeof(Brush),
                typeof(InteractiveCardAssist),
                new PropertyMetadata(Brushes.Transparent, OnVisualStatePropertyChanged));

        public static Brush GetNormalBackground(DependencyObject element)
        {
            return (Brush)element.GetValue(NormalBackgroundProperty);
        }

        public static void SetNormalBackground(DependencyObject element, Brush value)
        {
            element.SetValue(NormalBackgroundProperty, value);
        }


        public static readonly DependencyProperty HoverBackgroundProperty =
            DependencyProperty.RegisterAttached(
                "HoverBackground",
                typeof(Brush),
                typeof(InteractiveCardAssist),
                new PropertyMetadata(Brushes.Transparent, OnVisualStatePropertyChanged));

        public static Brush GetHoverBackground(DependencyObject element)
        {
            return (Brush)element.GetValue(HoverBackgroundProperty);
        }

        public static void SetHoverBackground(DependencyObject element, Brush value)
        {
            element.SetValue(HoverBackgroundProperty, value);
        }


        public static readonly DependencyProperty PressedBackgroundProperty =
            DependencyProperty.RegisterAttached(
                "PressedBackground",
                typeof(Brush),
                typeof(InteractiveCardAssist),
                new PropertyMetadata(Brushes.Transparent, OnVisualStatePropertyChanged));

        public static Brush GetPressedBackground(DependencyObject element)
        {
            return (Brush)element.GetValue(PressedBackgroundProperty);
        }

        public static void SetPressedBackground(DependencyObject element, Brush value)
        {
            element.SetValue(PressedBackgroundProperty, value);
        }


        public static readonly DependencyProperty SelectedBackgroundProperty =
            DependencyProperty.RegisterAttached(
                "SelectedBackground",
                typeof(Brush),
                typeof(InteractiveCardAssist),
                new PropertyMetadata(Brushes.Transparent, OnVisualStatePropertyChanged));

        public static Brush GetSelectedBackground(DependencyObject element)
        {
            return (Brush)element.GetValue(SelectedBackgroundProperty);
        }

        public static void SetSelectedBackground(DependencyObject element, Brush value)
        {
            element.SetValue(SelectedBackgroundProperty, value);
        }


        public static readonly DependencyProperty DisabledBackgroundProperty =
            DependencyProperty.RegisterAttached(
                "DisabledBackground",
                typeof(Brush),
                typeof(InteractiveCardAssist),
                new PropertyMetadata(Brushes.Transparent, OnVisualStatePropertyChanged));

        public static Brush GetDisabledBackground(DependencyObject element)
        {
            return (Brush)element.GetValue(DisabledBackgroundProperty);
        }

        public static void SetDisabledBackground(DependencyObject element, Brush value)
        {
            element.SetValue(DisabledBackgroundProperty, value);
        }

        #endregion

        #region Border Brushes

        public static readonly DependencyProperty NormalBorderBrushProperty =
            DependencyProperty.RegisterAttached(
                "NormalBorderBrush",
                typeof(Brush),
                typeof(InteractiveCardAssist),
                new PropertyMetadata(Brushes.Transparent, OnVisualStatePropertyChanged));

        public static Brush GetNormalBorderBrush(DependencyObject element)
        {
            return (Brush)element.GetValue(NormalBorderBrushProperty);
        }

        public static void SetNormalBorderBrush(DependencyObject element, Brush value)
        {
            element.SetValue(NormalBorderBrushProperty, value);
        }


        public static readonly DependencyProperty HoverBorderBrushProperty =
            DependencyProperty.RegisterAttached(
                "HoverBorderBrush",
                typeof(Brush),
                typeof(InteractiveCardAssist),
                new PropertyMetadata(Brushes.Transparent, OnVisualStatePropertyChanged));

        public static Brush GetHoverBorderBrush(DependencyObject element)
        {
            return (Brush)element.GetValue(HoverBorderBrushProperty);
        }

        public static void SetHoverBorderBrush(DependencyObject element, Brush value)
        {
            element.SetValue(HoverBorderBrushProperty, value);
        }


        public static readonly DependencyProperty PressedBorderBrushProperty =
            DependencyProperty.RegisterAttached(
                "PressedBorderBrush",
                typeof(Brush),
                typeof(InteractiveCardAssist),
                new PropertyMetadata(Brushes.Transparent, OnVisualStatePropertyChanged));

        public static Brush GetPressedBorderBrush(DependencyObject element)
        {
            return (Brush)element.GetValue(PressedBorderBrushProperty);
        }

        public static void SetPressedBorderBrush(DependencyObject element, Brush value)
        {
            element.SetValue(PressedBorderBrushProperty, value);
        }


        public static readonly DependencyProperty SelectedBorderBrushProperty =
            DependencyProperty.RegisterAttached(
                "SelectedBorderBrush",
                typeof(Brush),
                typeof(InteractiveCardAssist),
                new PropertyMetadata(Brushes.Transparent, OnVisualStatePropertyChanged));

        public static Brush GetSelectedBorderBrush(DependencyObject element)
        {
            return (Brush)element.GetValue(SelectedBorderBrushProperty);
        }

        public static void SetSelectedBorderBrush(DependencyObject element, Brush value)
        {
            element.SetValue(SelectedBorderBrushProperty, value);
        }


        public static readonly DependencyProperty DisabledBorderBrushProperty =
            DependencyProperty.RegisterAttached(
                "DisabledBorderBrush",
                typeof(Brush),
                typeof(InteractiveCardAssist),
                new PropertyMetadata(Brushes.Transparent, OnVisualStatePropertyChanged));

        public static Brush GetDisabledBorderBrush(DependencyObject element)
        {
            return (Brush)element.GetValue(DisabledBorderBrushProperty);
        }

        public static void SetDisabledBorderBrush(DependencyObject element, Brush value)
        {
            element.SetValue(DisabledBorderBrushProperty, value);
        }

        #endregion

        #region Opacity

        public static readonly DependencyProperty NormalOpacityProperty =
            DependencyProperty.RegisterAttached(
                "NormalOpacity",
                typeof(double),
                typeof(InteractiveCardAssist),
                new PropertyMetadata(1d, OnVisualStatePropertyChanged));

        public static double GetNormalOpacity(DependencyObject element)
        {
            return (double)element.GetValue(NormalOpacityProperty);
        }

        public static void SetNormalOpacity(DependencyObject element, double value)
        {
            element.SetValue(NormalOpacityProperty, value);
        }


        public static readonly DependencyProperty DisabledOpacityProperty =
            DependencyProperty.RegisterAttached(
                "DisabledOpacity",
                typeof(double),
                typeof(InteractiveCardAssist),
                new PropertyMetadata(0.45d, OnVisualStatePropertyChanged));

        public static double GetDisabledOpacity(DependencyObject element)
        {
            return (double)element.GetValue(DisabledOpacityProperty);
        }

        public static void SetDisabledOpacity(DependencyObject element, double value)
        {
            element.SetValue(DisabledOpacityProperty, value);
        }

        #endregion

        #region Event Handlers

        private static void OnMouseEnter(object sender, MouseEventArgs args)
        {
            if (sender is Border border)
                ApplyState(border);
        }

        private static void OnMouseLeave(object sender, MouseEventArgs args)
        {
            if (sender is not Border border)
                return;

            SetIsPressed(border, false);
            ApplyState(border);
        }

        private static void OnPreviewMouseLeftButtonDown(
            object sender,
            MouseButtonEventArgs args)
        {
            if (sender is not Border border)
                return;

            SetIsPressed(border, true);
            ApplyState(border);
        }

        private static void OnPreviewMouseLeftButtonUp(
            object sender,
            MouseButtonEventArgs args)
        {
            if (sender is not Border border)
                return;

            SetIsPressed(border, false);
            ApplyState(border);
        }

        private static void OnBorderIsEnabledChanged(
            object sender,
            DependencyPropertyChangedEventArgs args)
        {
            if (sender is Border border)
                ApplyState(border);
        }

        private static void OnVisualStatePropertyChanged(
            DependencyObject element,
            DependencyPropertyChangedEventArgs args)
        {
            if (element is Border border)
                ApplyState(border);
        }

        #endregion

        #region State Application

        private static void ApplyState(Border border)
        {
            if (!GetIsEnabled(border))
                return;

            if (!border.IsEnabled)
            {
                border.Background = GetDisabledBackground(border);
                border.BorderBrush = GetDisabledBorderBrush(border);
                border.Opacity = GetDisabledOpacity(border);
                return;
            }

            border.Opacity = GetNormalOpacity(border);

            if (GetIsPressed(border))
            {
                border.Background = GetPressedBackground(border);
                border.BorderBrush = GetPressedBorderBrush(border);
                return;
            }

            if (GetIsSelected(border))
            {
                border.Background = GetSelectedBackground(border);
                border.BorderBrush = GetSelectedBorderBrush(border);
                return;
            }

            if (border.IsMouseOver)
            {
                border.Background = GetHoverBackground(border);
                border.BorderBrush = GetHoverBorderBrush(border);
                return;
            }

            border.Background = GetNormalBackground(border);
            border.BorderBrush = GetNormalBorderBrush(border);
        }

        #endregion
    }
}