using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using WpfApplication1.Models;

namespace WpfApplication1.Common.Converters
{
    internal class AchievementTypeToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var source = value as AchievementsType? ?? AchievementsType.Empty;
            var param = parameter as string;
            if (source == AchievementsType.Empty) return Visibility.Collapsed;
            if (string.IsNullOrEmpty(param))
            {
                switch (source)
                {
                    case AchievementsType.ComboFw:
                    case AchievementsType.ComboWin:
                        return Visibility.Visible;
                    case AchievementsType.EpicFw:
                    case AchievementsType.EpicWin:
                        return Visibility.Collapsed;
                    case AchievementsType.Empty:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (param == "reversed")
            {
                switch (source)
                {
                    case AchievementsType.ComboFw:
                    case AchievementsType.ComboWin:
                        return Visibility.Collapsed;
                    case AchievementsType.EpicFw:
                    case AchievementsType.EpicWin:
                        return Visibility.Visible;
                    case AchievementsType.Empty:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
