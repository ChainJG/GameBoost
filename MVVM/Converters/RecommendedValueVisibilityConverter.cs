using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace GameBoost.MVVM.Converters
{
    public sealed class RecommendedValueVisibilityConverter : IValueConverter
    {
        public object Convert(
            object? value,
            Type targetType,
            object? parameter,
            CultureInfo culture)
        {
            if (value is null || value == DependencyProperty.UnsetValue)
                return Visibility.Collapsed;

            if (value is double doubleValue)
                return double.IsNaN(doubleValue)
                    ? Visibility.Collapsed
                    : Visibility.Visible;

            if (double.TryParse(
                    value.ToString(),
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out var parsedValue))
            {
                return double.IsNaN(parsedValue)
                    ? Visibility.Collapsed
                    : Visibility.Visible;
            }

            return Visibility.Collapsed;
        }

        public object ConvertBack(
            object? value,
            Type targetType,
            object? parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}