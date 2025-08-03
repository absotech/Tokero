using System.Globalization;

namespace Tokero.Converters
{
    public class RoiToColorConverter : IValueConverter
    {
        private readonly Color lightThemePositive = Colors.Green;
        private readonly Color darkThemePositive = Color.FromRgb(30, 215, 96);
        private readonly Color lightThemeNegative = Colors.Red;
        private readonly Color darkThemeNegative = Color.FromRgb(255, 107, 107);

        private readonly Color neutralColor = Colors.Gray;

        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            bool isDarkMode = Application.Current?.RequestedTheme == AppTheme.Dark;
            if (value is not (double) and not (decimal))
                return neutralColor;

            double numericRoi = System.Convert.ToDouble(value);

            if (numericRoi > 0)
            {
                return isDarkMode ? darkThemePositive : lightThemePositive;
            }
            if (numericRoi < 0)
            {
                return isDarkMode ? darkThemeNegative : lightThemeNegative;
            }

            return neutralColor;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
