using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hollinger2025.Helpers
{
    public class InverseBooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b && b)
                return Visibility.Collapsed; // If b==true, hide
            return Visibility.Visible;      // If b==false, show
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Typically not used in one-way visibility bindings
            throw new NotImplementedException();
        }
    }
}
