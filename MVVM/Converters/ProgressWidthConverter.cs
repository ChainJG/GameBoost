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

            if (values[0] is not double actualWidth)
                return 0d;

            if (!TryGetProgress(values[1], out var progress))
                return 0d;

            progress = Math.Clamp(progress, 0d, 100d);

            return actualWidth * (progress / 100d);
        }

        public object[] ConvertBack(
            object value,
            Type[] targetTypes,
            object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private static bool TryGetProgress(object value, out double progress)
        {
            progress = 0d;

            if (value is double doubleValue)
            {
                progress = doubleValue;
                return true;
            }

            if (value is int intValue)
            {
                progress = intValue;
                return true;
            }

            if (value is string stringValue &&
                double.TryParse(stringValue, out var parsed))
            {
                progress = parsed;
                return true;
            }

            return false;
        }
    }
}