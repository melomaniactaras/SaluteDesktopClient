using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfApplication1.Common.Converters
{
    internal class ToUppercaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var source = value as string;
            return !string.IsNullOrEmpty(source) ? source.ToUpper() : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
