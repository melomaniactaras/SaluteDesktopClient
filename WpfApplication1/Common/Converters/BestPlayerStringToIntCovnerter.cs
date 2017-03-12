using System;
using System.Globalization;
using System.Windows.Data;
using static WpfApplication1.Common.CommonMethods;

namespace WpfApplication1.Common.Converters
{
    internal class BestPlayerStringToIntCovnerter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var source = value as int?;
            if (source == 0 || source == null) return string.Empty;
            switch (source)
            {
                case 1:
                    return GetLocalized("Best1");
                case 2:
                    return GetLocalized("Best2");
                case 3:
                    return GetLocalized("Best3");
                default:
                    return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (string) value;
            if (val == GetLocalized("Best1"))
            {
                return 1;
            }
            if (val == GetLocalized("Best2"))
            {
                return 2;
            }
            if (val == GetLocalized("Best3"))
            {
                return 3;
            }
            return null;
        }
    }
}
