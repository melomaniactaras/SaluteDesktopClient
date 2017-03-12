using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WpfApplication1.Models;
using static WpfApplication1.Common.CommonMethods;

namespace WpfApplication1.Common
{
    public static class Extensions
    {
        public static int Increment(this int? value)
        {
            return value + 1 ?? 1;
        }

        public static bool IsRedTeam(this PlayerEntry player)
        {
            return player.Role == GetLocalized("Red") || player.Role == GetLocalized("Sheriff");
        }

        public static bool IsBlackTeam(this PlayerEntry player)
        {
            return player.Role == GetLocalized("Black") || player.Role == GetLocalized("Don");
        }

        public static bool IsRedTeam(this FiimPlayerEntry player)
        {
            return player.Role == GetLocalized("Red") || player.Role == GetLocalized("Sheriff");
        }

        public static bool IsBlackTeam(this FiimPlayerEntry player)
        {
            return player.Role == GetLocalized("Black") || player.Role == GetLocalized("Don");
        }

        public static string ToStatisticLimitedString(this string nick)
        {
            return nick.Length >= 15 ? nick.Substring(0, 12) + "..." : nick;
        }

        public static void AddRange<T>(this ObservableCollection<T> coll, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                coll.Add(item);
            }
        }

        public static ObservableCollection<T> ToObservable<T>(this IEnumerable<T> current)
        {
            var result = new ObservableCollection<T>();
            foreach (var item in current)
            {
                result.Add(item);
            }
            return result;
        }

        public static double DoubleDivide(int? a, int? b)
        {
            if (a != null && b != null) return (double)a.Value /b.Value;
            return double.NaN;
        }

        public static double RoundTwo(this double value)
        {
            return Math.Round(value, 2);
        }

        public static string GetUnifiedRole(this string role)
        {
            if (role == "Мафія" || role == "Мафия" || role == "Black")
                return GetLocalized("Black");
            if (role == "Мирний" || role == "Мирный" || role == "Red")
                return GetLocalized("Red");
            if (role == "Дон" || role == "Don")
                return GetLocalized("Don");
            if (role == "Шериф" || role == "Sheriff")
                return GetLocalized("Sheriff");
            return string.Empty;
        }

        public static string SetUnifiedRole(this string role)
        {
            if (role == GetLocalized("Black"))
                return "Black";
            if (role == GetLocalized("Red"))
                return "Red";
            if (role == GetLocalized("Don"))
                return "Don";
            if (role == GetLocalized("Sheriff"))
                return "Sheriff";
            return string.Empty;
        }
    }
}
