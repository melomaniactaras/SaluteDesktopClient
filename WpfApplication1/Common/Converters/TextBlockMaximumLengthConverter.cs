using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfApplication1.Common.Converters
{
    internal class TextBlockMaximumLengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var source = value as string;
            int param;
            var isParamParsed = int.TryParse(parameter as string, out param);
            if (!isParamParsed) return string.Empty;
            if (!(source?.Length >= param)) return source;
            var output = source.Substring(0, param - 3) + "...";
            return output;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
