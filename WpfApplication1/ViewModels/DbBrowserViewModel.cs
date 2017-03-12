using System.Collections.ObjectModel;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls.Dialogs;
using WpfApplication1.DAL;

namespace WpfApplication1.ViewModels
{
    internal class DbBrowserViewModel : ViewModelBase
    {
        private readonly SQLiteConnection _con;
        public string TableName;
        readonly MetroDialogSettings _settings = new MetroDialogSettings
        {
            ColorScheme = MetroDialogColorScheme.Accented,
            AnimateShow = true,
            AnimateHide = true
        };
        public DbBrowserViewModel()
        {
            SetDefaultBrushes();
            Brushes[0] = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF086F9E");
            var s = $@"{Directory.GetCurrentDirectory()}\DB\databaseFile.db3";
            _con = new SQLiteConnection($"data source={s}");
            TableName = "SeasonRating";
            ShowDb();
        }

        private void SetDefaultBrushes()
        {
            Brushes = new ObservableCollection<Brush>
            {
                (SolidColorBrush) new BrushConverter().ConvertFrom("#CC119EDA"),
                (SolidColorBrush) new BrushConverter().ConvertFrom("#CC119EDA"),
                (SolidColorBrush) new BrushConverter().ConvertFrom("#CC119EDA"),
                (SolidColorBrush) new BrushConverter().ConvertFrom("#CC119EDA")
            };
        }

        #region Properties

        public RelayCommand SeasonRatingOpen => new RelayCommand(SeasonRatingOpenMethod);

        public RelayCommand SeriesRatingOpen => new RelayCommand(SeriesRatingOpenMethod);

        public RelayCommand AllTimeRatingOpen => new RelayCommand(AllTimeRatingOpenMethod);

        public RelayCommand WeekRatingOpen => new RelayCommand(WeekRatingOpenMethod);

        public RelayCommand<string> FilterDb => new RelayCommand<string>(FilterDbMethod);

        public RelayCommand<string> ShowPlayerInfo => new RelayCommand<string>(ShowPlayerInfoMethod);

        public RelayCommand ExpanderClosing => new RelayCommand(ExpanderClosingMethod);

        public RelayCommand<string> MemberStatusChecked => new RelayCommand<string>(MemberStatusCheckedMehtod);

        public RelayCommand<string> MemberStatusUnchecked => new RelayCommand<string>(MemberStatusUncheckedMehtod);

        private DataView _itemsRating;
        public DataView ItemsRating
        {
            get { return _itemsRating; }
            set { Set(nameof(ItemsRating), ref _itemsRating, value); }
        }

        private ObservableCollection<Brush> _brushes;
        public ObservableCollection<Brush> Brushes
        {
            get { return _brushes; }
            set { Set(nameof(Brushes), ref _brushes, value); }
        }

        private string _filterText;
        public string FilterText
        {
            get { return _filterText; }
            set { Set(nameof(FilterText), ref _filterText, value); }
        }

        #endregion

        public void SeasonRatingOpenMethod()
        {
            TableName = "SeasonRating";
            ShowDb();
            SetDefaultBrushes();
            Brushes[0] = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF086F9E");
        }

        public void SeriesRatingOpenMethod()
        {
            TableName = "SeriesRating";
            ShowDb();
            SetDefaultBrushes();
            Brushes[1] = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF086F9E");
        }

        public void AllTimeRatingOpenMethod()
        {
            TableName = "AllTimeRating";
            ShowDb();
            SetDefaultBrushes();
            Brushes[2] = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF086F9E");
        }

        public void WeekRatingOpenMethod()
        {
            TableName = "WeekRating";
            ShowDb();
            SetDefaultBrushes();
            Brushes[3] = (SolidColorBrush)new BrushConverter().ConvertFrom("#FF086F9E");
        }

        private void ShowDb()
        {
            try
            {
                var dataSet = new DataSet();
                var com = "SELECT Nick, Games, Score, Rating, BestPlayer, BestChoice, Win, TechRed, TechBlack, Lose, RedUgadayka, BlackUgadayka, WinRow, WinDon, WinSheriff, WinRed, WinBlack, Fouls, WithoutFouls, KilledAtFirstDay, ThreeZv, Ban, IsMember FROM " + TableName + " ORDER BY Rating DESC";
                var dataAdapter = new SQLiteDataAdapter(com, _con);
                dataAdapter.Fill(dataSet, TableName);
                ItemsRating = dataSet.Tables[TableName].DefaultView;
            }
            catch (SQLiteException)
            {
                var screen = Application.Current.Windows.OfType<DBBrowser>().SingleOrDefault(x => x.IsActive);
                screen.ShowMessageAsync("ERROR", "Table doesn't exists", MessageDialogStyle.Affirmative, _settings);
            }
        }

        private void FilterDbMethod(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                var dataSet = new DataSet();
                var com =
                    "SELECT Nick, Games, Score, Rating, BestPlayer, BestChoice, Win, TechRed, TechBlack, Lose, RedUgadayka, BlackUgadayka, WinRow, WinDon, WinSheriff, WinRed, WinBlack, Fouls, WithoutFouls, KilledAtFirstDay, ThreeZv, Ban, IsMember FROM " +
                    TableName + " WHERE NickNormalized LIKE '" + input.ToLower() + "%'";
                var dataAdapter = new SQLiteDataAdapter(com, _con);
                dataAdapter.Fill(dataSet, TableName);
                ItemsRating = dataSet.Tables[TableName].DefaultView;
            }
            else ShowDb();
        }

        private static void ShowPlayerInfoMethod(string currRow)
        {
            var stats = new PlayerStats
            {
                DataContext = new PlayerStatsViewModel(currRow)
            };
            stats.Show();
        }

        private static void MemberStatusCheckedMehtod(string nick)
        {
            MemberStatusChanged(nick, true);
        }

        private static void MemberStatusUncheckedMehtod(string nick)
        {
            MemberStatusChanged(nick, false);
        }

        private static void MemberStatusChanged(string nick, bool check)
        {
            using (var context = new SaluteDbContext())
            {
                var inSeason = context.SeasonRatingDbSet.FirstOrDefault(t => t.Nick == nick);
                var inSeries = context.SeriesRatingDbSet.FirstOrDefault(t => t.Nick == nick);
                var inWeek = context.WeekRatingDbSet.FirstOrDefault(t => t.Nick == nick);
                var inAllTime = context.AllTimeRatingDbSet.FirstOrDefault(t => t.Nick == nick);
                var inSeasonFiim = context.SeasonRatingFiimDbSet.FirstOrDefault(t => t.Nick == nick);
                var inSeriesFiim = context.SeriesRatingFiimDbSet.FirstOrDefault(t => t.Nick == nick);
                var inWeekFiim = context.WeekRatingFiimDbSet.FirstOrDefault(t => t.Nick == nick);
                var inAllTimeFim = context.AllTimeRatingFiimDbSet.FirstOrDefault(t => t.Nick == nick);
                if (inSeason != null)
                {
                    inSeason.IsMember = check;
                }
                if (inSeries != null)
                {
                    inSeries.IsMember = check;
                }
                if (inWeek != null)
                {
                    inWeek.IsMember = check;
                }
                if (inAllTime != null)
                {
                    inAllTime.IsMember = check;
                }
                if (inSeasonFiim != null)
                {
                    inSeasonFiim.IsMember = check;
                }
                if (inSeriesFiim != null)
                {
                    inSeriesFiim.IsMember = check;
                }
                if (inWeekFiim != null)
                {
                    inWeekFiim.IsMember = check;
                }
                if (inAllTimeFim != null)
                {
                    inAllTimeFim.IsMember = check;
                }
                context.SaveChanges();
            }
        }

        private void ExpanderClosingMethod()
        {
            FilterText = string.Empty;
            ShowDb();
        }
    }
}
