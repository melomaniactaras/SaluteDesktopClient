using System;
using System.Globalization;
using System.Windows.Data;

namespace WpfApplication1.Common.Converters
{
    internal class CultureConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var source = value as CultureInfo;
            if (value == null)
                return string.Empty;
            switch (source?.Name)
            {
                case "en-US":
                    return "English";
                case "ru-RU":
                    return "Русский";
                case "uk-UA":
                    return "Українська";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value as string)
            {
                case "English":
                    return CultureInfo.GetCultureInfo("en-US");
                case "Русский":
                    return CultureInfo.GetCultureInfo("ru-RU");
                case "Українська":
                    return CultureInfo.GetCultureInfo("uk-UA");
            }
            return CultureInfo.CurrentCulture;
        }
    }
}
