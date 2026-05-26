using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace GameBoost
{
    public class CornerRadiusAnimation : AnimationTimeline
    {
        public override Type TargetPropertyType => typeof(CornerRadius);

        public CornerRadius From
        {
            get => (CornerRadius)GetValue(FromProperty);
            set => SetValue(FromProperty, value);
        }

        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register(nameof(From), typeof(CornerRadius), typeof(CornerRadiusAnimation));

        public CornerRadius To
        {
            get => (CornerRadius)GetValue(ToProperty);
            set => SetValue(ToProperty, value);
        }

        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register(nameof(To), typeof(CornerRadius), typeof(CornerRadiusAnimation));

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue,
            AnimationClock clock)
        {
            if (clock.CurrentProgress == null)
                return From;

            double progress = clock.CurrentProgress.Value;

            return new CornerRadius(
                From.TopLeft + (To.TopLeft - From.TopLeft) * progress,
                From.TopRight + (To.TopRight - From.TopRight) * progress,
                From.BottomRight + (To.BottomRight - From.BottomRight) * progress,
                From.BottomLeft + (To.BottomLeft - From.BottomLeft) * progress
            );
        }

        protected override Freezable CreateInstanceCore()
        {
            return new CornerRadiusAnimation();
        }
    }
}