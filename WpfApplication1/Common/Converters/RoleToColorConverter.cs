using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using static WpfApplication1.Common.CommonMethods;

namespace WpfApplication1.Common.Converters
{
    internal class RoleToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var source = value as string;
            var foreground = parameter as string == "Foreground";
            if (source == GetLocalized("Red"))
            {
                return foreground ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Color.FromRgb(255, 125, 125));
            }
            if (source == GetLocalized("Black"))
            {
                return foreground ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Color.FromRgb(70, 70, 70));
            }
            if (source == GetLocalized("Sheriff"))
            {
                return foreground ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Red);
            }
            if (source == GetLocalized("Don"))
            {
                return foreground ? new SolidColorBrush(Colors.White) : new SolidColorBrush(Colors.Black);
            }
            return foreground ? new SolidColorBrush(Colors.Black) : new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
