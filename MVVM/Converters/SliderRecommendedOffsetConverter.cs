using System.Globalization;
using System.Windows.Data;

namespace GameBoost.MVVM.Converters
{
    public sealed class SliderRecommendedOffsetConverter : IMultiValueConverter
    {
        public object Convert(
            object[] values,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (values.Length < 5)
                return 0d;

            if (!TryToDouble(values[0], out var trackWidth) ||
                !TryToDouble(values[1], out var minimum) ||
                !TryToDouble(values[2], out var maximum) ||
                !TryToDouble(values[3], out var recommendedValue) ||
                !TryToDouble(values[4], out var markerWidth))
            {
                return 0d;
            }

            if (trackWidth <= 0 || maximum <= minimum || double.IsNaN(recommendedValue))
                return 0d;

            var percentage = (recommendedValue - minimum) / (maximum - minimum);
            percentage = Math.Clamp(percentage, 0d, 1d);

            return (trackWidth - markerWidth) * percentage;
        }

        public object[] ConvertBack(
            object value,
            Type[] targetTypes,
            object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private static bool TryToDouble(object? value, out double result)
        {
            result = 0;

            if (value is null)
                return false;

            if (value is double doubleValue)
            {
                result = doubleValue;
                return true;
            }

            return double.TryParse(
                value.ToString(),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out result);
        }
    }
}