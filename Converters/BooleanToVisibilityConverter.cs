using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Calculator.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public bool IsInverted { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolVal)
            {
                if (IsInverted) boolVal = !boolVal;
                return boolVal ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                bool result = visibility == Visibility.Visible;
                return IsInverted ? !result : result;
            }
            return false;
        }
    }
}
