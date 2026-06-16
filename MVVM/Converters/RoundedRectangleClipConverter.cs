using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace GameBoost.MVVM.Converters
{
    public sealed class RoundedRectangleClipConverter : IMultiValueConverter
    {
        public object Convert(
            object[] values,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (values.Length < 3)
                return Geometry.Empty;

            if (!TryGetDouble(values[0], out var width) ||
                !TryGetDouble(values[1], out var height))
            {
                return Geometry.Empty;
            }

            if (width <= 0 || height <= 0)
                return Geometry.Empty;

            var radius = GetRadius(values[2]);

            radius = Math.Clamp(
                radius,
                0,
                Math.Min(width, height) / 2);

            return new RectangleGeometry(
                new Rect(0, 0, width, height),
                radius,
                radius);
        }

        public object[] ConvertBack(
            object value,
            Type[] targetTypes,
            object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private static double GetRadius(object? value)
        {
            if (value is CornerRadius cornerRadius)
                return cornerRadius.TopLeft;

            if (TryGetDouble(value, out var radius))
                return radius;

            return 0;
        }

        private static bool TryGetDouble(object? value, out double result)
        {
            if (value is double doubleValue)
            {
                result = doubleValue;
                return true;
            }

            return double.TryParse(
                value?.ToString(),
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out result);
        }
    }
}