using System;
using System.Globalization;
using System.Windows.Data;
using WpfApplication1.Models;
using static WpfApplication1.Common.CommonMethods;

namespace WpfApplication1.Common.Converters
{
    internal class AchievementTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var source = value as AchievementEntry;
            if (source == null) return string.Empty;
            switch (source.TypeOfAchievements)
            {
                case AchievementsType.ComboFw:
                    return GetLocalized("ComboWfText");
                case AchievementsType.ComboWin:
                    return GetLocalized("ComboWinText");
                case AchievementsType.EpicFw:
                    return GetLocalized("EpicFwText");
                case AchievementsType.EpicWin:
                    return GetLocalized("EpicWinText");
                case AchievementsType.Empty:
                    break;
                default:
                    return string.Empty;
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
