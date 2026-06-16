using System.Globalization;
using System.Windows.Data;

namespace GameBoost.MVVM.Converters
{
    public sealed class ProgressWidthConverter : IMultiValueConverter
    {
        public object Convert(
            object[] values,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (values.Length < 2)
                return 0d;

            if (!TryGetDouble(values[0], out var actualWidth))
                return 0d;

            if (!TryGetDouble(values[1], out var percentage))
                return 0d;

            if (actualWidth <= 0)
                return 0d;

            percentage = Math.Clamp(percentage, 0, 100);

            return actualWidth * (percentage / 100d);
        }

        public object[] ConvertBack(
            object value,
            Type[] targetTypes,
            object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
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