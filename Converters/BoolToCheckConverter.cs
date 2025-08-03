using System.Globalization;

namespace Tokero.Converters
{
    public class BoolToCheckConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isSelected)
            {
                return isSelected ? "✓" : string.Empty;
            }
            return string.Empty;
        }
        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
