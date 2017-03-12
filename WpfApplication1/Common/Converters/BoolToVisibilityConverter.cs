using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfApplication1.Common.Converters
{
    internal class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var source = value as bool?;
            var param = parameter as string;
            if (param == "reversed")
            {
                return (source != null && (bool)source) ? Visibility.Collapsed : Visibility.Visible;
            }
            return (source != null && (bool)source) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
