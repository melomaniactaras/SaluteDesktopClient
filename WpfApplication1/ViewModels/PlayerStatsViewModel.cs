using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using WpfApplication1.DAL;
using WpfApplication1.Models;
using WpfApplication1.Common;
using static WpfApplication1.Common.CommonMethods;

namespace WpfApplication1.ViewModels
{
    internal class PlayerStatsViewModel : ViewModelBase
    {
        private readonly SaluteDbContext _context = new SaluteDbContext();
        public PlayerStatsViewModel(string nick)
        {
            PlayerNick = nick;
            SetSeasonStats();
            SeasonLineSet();
            SeriesLineSet();
            SeasonEnabled = _context.SeasonRatingDbSet.Any(x => x.Nick == nick);
            SeriesEnabled = _context.SeriesRatingDbSet.Any(x => x.Nick == nick);
            WeekEnabled = _context.WeekRatingDbSet.Any(x => x.Nick == nick);
            var seasonPlayer = _context.SeasonRatingDbSet.FirstOrDefault(x => x.Nick == nick);
            if (!string.IsNullOrEmpty(seasonPlayer?.Position))
            {
                var positions = SaluteRepository.GetPosition(seasonPlayer.Position);
                positions = positions.Where(x => x != 0).ToList();
                SeasonMaximum = positions.Count != 0 ? positions.Min() : 0;
                SeasonMinimum = positions.Count != 0 ? positions.Max() : 0;
            }
            var seriesPlayer = _context.SeriesRatingDbSet.FirstOrDefault(x => x.Nick == nick);
            if (string.IsNullOrEmpty(seriesPlayer?.Position)) return;
            {
                var positions = SaluteRepository.GetPosition(seriesPlayer.Position);
                positions = positions.Where(x => x != 0).ToList();
                SeriesMaximum = positions.Count != 0 ? positions.Min() : 0;
                SeriesMinimum = positions.Count != 0 ? positions.Max() : 0;
            }
        }

        #region Properties

        private string _currDb;

        public RelayCommand SeasonButton => new RelayCommand(SetSeasonStats);

        public RelayCommand SeriesButton => new RelayCommand(SetSeriesStats);

        public RelayCommand WeekButton => new RelayCommand(SetWeekStats);

        public RelayCommand AllTimeButton => new RelayCommand(SetAllTimeStats);

        public RelayCommand ExportButton => new RelayCommand(ExportMethod);

        private string _playerNick;
        public string PlayerNick
        {
            get { return _playerNick; }
            set { Set(nameof(PlayerNick), ref _playerNick, value); }
        }

        private bool _seasonEnabled;
        public bool SeasonEnabled
        {
            get { return _seasonEnabled; }
            set { Set(nameof(SeasonEnabled), ref _seasonEnabled, value); }
        }

        private bool _seriesEnabled;
        public bool SeriesEnabled
        {
            get { return _seriesEnabled; }
            set { Set(nameof(SeriesEnabled), ref _seriesEnabled, value); }
        }

        private bool _weekEnabled;
        public bool WeekEnabled
        {
            get { return _weekEnabled; }
            set { Set(nameof(WeekEnabled), ref _weekEnabled, value); }
        }

        private int _gamesCount;
        public int GamesCount
        {
            get { return _gamesCount; }
            set { Set(nameof(GamesCount), ref _gamesCount, value); }
        }

        private double _score;
        public double Score
        {
            get { return _score; }
            set { Set(nameof(Score), ref _score, value); }
        }

        private double _rating;
        public double Rating
        {
            get { return _rating; }
            set { Set(nameof(Rating), ref _rating, value); }
        }

        private double _bestPlayer;
        public double BestPlayer
        {
            get { return _bestPlayer; }
            set { Set(nameof(BestPlayer), ref _bestPlayer, value); }
        }

        private double _banRatio;
        public double BanRatio
        {
            get { return _banRatio; }
            set { Set(nameof(BanRatio), ref _banRatio, value); }
        }

        private double _firstNightRatio;
        public double FirstNightRatio
        {
            get { return _firstNightRatio; }
            set { Set(nameof(FirstNightRatio), ref _firstNightRatio, value); }
        }

        private double _foulsRatio;
        public double FoulsRatio
        {
            get { return _foulsRatio; }
            set { Set(nameof(FoulsRatio), ref _foulsRatio, value); }
        }

        private double _checkRatio;
        public double CheckRatio
        {
            get { return _checkRatio; }
            set { Set(nameof(CheckRatio), ref _checkRatio, value); }
        }

        private int _techRed;
        public int TechRed
        {
            get { return _techRed; }
            set { Set(nameof(TechRed), ref _techRed, value); }
        }

        private int _techBlack;
        public int TechBlack
        {
            get { return _techBlack; }
            set { Set(nameof(TechBlack), ref _techBlack, value); }
        }

        private double _winDon;
        public double WinDon
        {
            get { return _winDon; }
            set { Set(nameof(WinDon), ref _winDon, value); }
        }

        private double _winSheriff;
        public double WinSheriff
        {
            get { return _winSheriff; }
            set { Set(nameof(WinSheriff), ref _winSheriff, value); }
        }

        private double _winRed;
        public double WinRed
        {
            get { return _winRed; }
            set { Set(nameof(WinRed), ref _winRed, value); }
        }

        private double _winBlack;
        public double WinBlack
        {
            get { return _winBlack; }
            set { Set(nameof(WinBlack), ref _winBlack, value); }
        }

        private int _redUgad;
        public int RedUgad
        {
            get { return _redUgad; }
            set { Set(nameof(RedUgad), ref _redUgad, value); }
        }

        private int _blackUgad;
        public int BlackUgad
        {
            get { return _blackUgad; }
            set { Set(nameof(BlackUgad), ref _blackUgad, value); }
        }

        private int _seasonMaximum;
        public int SeasonMaximum
        {
            get { return _seasonMaximum; }
            set { Set(nameof(SeasonMaximum), ref _seasonMaximum, value); }
        }

        private int _seasonMinimum;
        public int SeasonMinimum
        {
            get { return _seasonMinimum; }
            set { Set(nameof(SeasonMinimum), ref _seasonMinimum, value); }
        }

        private int _seriesMaximum;
        public int SeriesMaximum
        {
            get { return _seriesMaximum; }
            set { Set(nameof(SeriesMaximum), ref _seriesMaximum, value); }
        }

        private int _seriesMinimum;
        public int SeriesMinimum
        {
            get { return _seriesMinimum; }
            set { Set(nameof(SeriesMinimum), ref _seriesMinimum, value); }
        }

        private double _falseComRatio;
        public double FalseComRatio
        {
            get { return _falseComRatio; }
            set { Set(nameof(FalseComRatio), ref _falseComRatio, value); }
        }

        private PlotModel _seasonLine;
        public PlotModel SeasonLine
        {
            get { return _seasonLine; }
            set { Set(nameof(SeasonLine), ref _seasonLine, value); }
        }

        private PlotModel _seriesLine;
        public PlotModel SeriesLine
        {
            get { return _seriesLine; }
            set { Set(nameof(SeriesLine), ref _seriesLine, value); }
        }

        private PlotModel _winLose;
        public PlotModel WinLose
        {
            get { return _winLose; }
            set { Set(nameof(WinLose), ref _winLose, value); }
        }

        private PlotModel _rolesStat;
        public PlotModel RolesStat
        {
            get { return _rolesStat; }
            set { Set(nameof(RolesStat), ref _rolesStat, value); }
        }

        #endregion

        private void SeasonLineSet()
        {
            var currPlayer = _context.SeasonRatingDbSet.FirstOrDefault(x => x.Nick == PlayerNick);
            if (string.IsNullOrEmpty(currPlayer?.Position)) return;
            var positions = SaluteRepository.GetPosition(currPlayer.Position);
            SeasonLine = LineSet(positions);
        }

        private void SeriesLineSet()
        {
            var currPlayer = _context.SeriesRatingDbSet.FirstOrDefault(x => x.Nick == PlayerNick);
            if (string.IsNullOrEmpty(currPlayer?.Position)) return;
            var positions = SaluteRepository.GetPosition(currPlayer.Position);
            SeriesLine = LineSet(positions);
        }

        private static PlotModel LineSet(IReadOnlyList<int> positions)
        {
            var plot = new PlotModel
            {
                Title = null,
                IsLegendVisible = false,
                PlotAreaBorderThickness = new OxyThickness(0, 0, 0, 0),
            };
            var max = positions.Max();
            var min = positions.Min();
            plot.Axes.Add(new LinearAxis
            {
                TicklineColor = OxyColors.Transparent,
                TextColor = OxyColors.White,
                AxislineColor = OxyColors.White,
                StartPosition = 1,
                EndPosition = 0,
                Minimum = min - 1,
                Maximum = max + 1,
                IntervalLength = 5,
                FontSize = 8,
                MinorTickSize = 0,
            });
            plot.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                TickStyle = TickStyle.None,
                AxislineColor = OxyColors.White,
                TicklineColor = OxyColors.White,
                TextColor = OxyColors.Transparent,
            });
            var lineSeries = new LineSeries
            {
                Color = OxyColors.Red,
                LineStyle = LineStyle.Solid,
                MarkerFill = OxyColors.White,
                MarkerSize = 1.5,
                MarkerStroke = OxyColors.White,
                MarkerStrokeThickness = 0,
                MarkerType = MarkerType.Circle,
                StrokeThickness = 2,
                Title = "Рейтинг сезону",
                TrackerKey = "Tracker1",
                CanTrackerInterpolatePoints = false,
                BrokenLineColor = OxyColors.Transparent
            };
            for (var i = 0; i < positions.Count; i++)
            {
                lineSeries.Points.Add(positions[i] != 0 ? new DataPoint(i, positions[i]) : new DataPoint(i, double.NaN));
            }
            plot.Series.Add(lineSeries);
            return plot;
        }

        private static PlotModel WinLoseSet(IRating player)
        {
            var win = player?.Win ?? 0;
            var lose = player?.Lose ?? 0;
            var plot = new PlotModel
            {
                Title = null,
                IsLegendVisible = false
            };
            var ps = new PieSeries
            {
                InsideLabelPosition = 0.5,
                Stroke = OxyColors.Transparent,
                StartAngle = 0,
                AngleSpan = 360,
                InsideLabelColor = OxyColors.White,
                FontSize = 10,
                TextColor = OxyColors.White,
            };
            var redslice = new PieSlice
            {
                Label = GetLocalized("InfoWinLabel"),
                Value = win,
                Fill = OxyColors.Red
            };
            var blackslice = new PieSlice
            {
                Label = GetLocalized("InfoLoseLabel"),
                Value = lose,
                Fill = OxyColors.Black,
            };
            ps.Slices.Add(redslice);
            ps.Slices.Add(blackslice);
            plot.Series.Add(ps);
            return plot;
        }

        private static PlotModel RolesStatSet(IRating player)
        {
            var red = player != null ? player.WinRed.GetValueOrDefault() + player.LoseRed.GetValueOrDefault() : 0;
            var black = player != null ? player.WinBlack.GetValueOrDefault() + player.LoseBlack.GetValueOrDefault() : 0;
            var sheriff = player != null ? player.WinSheriff.GetValueOrDefault() + player.LoseSheriff.GetValueOrDefault() : 0;
            var don = player != null ? player.WinDon.GetValueOrDefault() + player.LoseDon.GetValueOrDefault() : 0;
            var plot = new PlotModel
            {
                Title = null,
                IsLegendVisible = false
            };
            var ps = new PieSeries
            {
                InsideLabelPosition = 0.6,
                Stroke = OxyColors.Transparent,
                StartAngle = 0,
                AngleSpan = 360,
                InsideLabelColor = OxyColors.White,
                FontSize = 8,
                TextColor = OxyColors.White,
            };
            var redslice = new PieSlice
            {
                Label = GetLocalized("InfoRedSlice"), 
                Value = red,
                Fill = OxyColors.Red
            };
            var blackslice = new PieSlice
            {
                Label = GetLocalized("InfoBlackSlice"),
                Value = black,
                Fill = OxyColors.Black,
            };
            var sheriffslice = new PieSlice
            {
                Label = GetLocalized("InfoSheriffSlice"),
                Value = sheriff,
                Fill = OxyColor.FromRgb(255,100,100)
            };
            var donslice = new PieSlice
            {
                Label = GetLocalized("InfoDonSlice"),
                Value = don,
                Fill = OxyColor.FromRgb(50, 50, 50)
            };
            if (red != 0) ps.Slices.Add(redslice);
            if (black != 0) ps.Slices.Add(blackslice);
            if (don != 0) ps.Slices.Add(donslice);
            if (sheriff != 0) ps.Slices.Add(sheriffslice);
            plot.Series.Add(ps);
            return plot;
        }

        private void SetStats(IRating currPlayer)
        {
            GamesCount = currPlayer.Games;
            Score = currPlayer.Score ?? 0;
            Rating = currPlayer.Rating ?? 0;
            BestPlayer = currPlayer.BestPlayer != null && currPlayer.Games != 0 ? (Extensions.DoubleDivide(currPlayer.BestPlayer, currPlayer.Games) * 100).RoundTwo() : 0;
            BanRatio = currPlayer.Ban != null ? (Extensions.DoubleDivide(currPlayer.Ban, currPlayer.Games) * 100).RoundTwo() : 0;
            FirstNightRatio = currPlayer.KilledAtFirstDay != null ? (Extensions.DoubleDivide(currPlayer.KilledAtFirstDay, currPlayer.Games)*100).RoundTwo() : 0;
            FoulsRatio = currPlayer.Fouls != null ? (Extensions.DoubleDivide(currPlayer.Fouls, currPlayer.Games)).RoundTwo() : 0;
            CheckRatio = currPlayer.CheckedFirst != null ? (Extensions.DoubleDivide(currPlayer.CheckedFirst, currPlayer.Games) * 100).RoundTwo() : 0;
            WinDon = currPlayer.WinDon != null ? (Extensions.DoubleDivide(currPlayer.WinDon, currPlayer.WinDon + (currPlayer.LoseDon ?? 0)) * 100).RoundTwo() : 0;
            WinSheriff = currPlayer.WinSheriff != null ? (Extensions.DoubleDivide(currPlayer.WinSheriff, currPlayer.WinSheriff + (currPlayer.LoseSheriff ?? 0)) * 100).RoundTwo() : 0;
            WinRed = currPlayer.WinRed != null ? (Extensions.DoubleDivide(currPlayer.WinRed, currPlayer.WinRed + (currPlayer.LoseRed ?? 0))*100).RoundTwo() : 0;
            WinBlack = currPlayer.WinBlack != null ? (Extensions.DoubleDivide(currPlayer.WinBlack, currPlayer.WinBlack + (currPlayer.LoseBlack ?? 0))*100).RoundTwo() : 0;
            FalseComRatio = currPlayer.FalseComWin != null ? (Extensions.DoubleDivide(currPlayer.FalseComWin, currPlayer.FalseCom) * 100).RoundTwo() : 0;
            TechRed = currPlayer.TechRed ?? 0;
            TechBlack = currPlayer.TechBlack ?? 0;
            RedUgad = currPlayer.RedUgadayka ?? 0;
            BlackUgad = currPlayer.BlackUgadayka ?? 0;
            WinLose = WinLoseSet(currPlayer);
            RolesStat = RolesStatSet(currPlayer);
        }

        private void SetSeasonStats()
        {
            IRating currPlayer = _context.SeasonRatingDbSet.FirstOrDefault(x => x.Nick == PlayerNick);
            if (currPlayer != null)
            {
                SetStats(currPlayer);
            }
            _currDb = "Season";
        }

        private void SetSeriesStats()
        {
            IRating currPlayer = _context.SeriesRatingDbSet.FirstOrDefault(x => x.Nick == PlayerNick);
            if (currPlayer != null)
            {
                SetStats(currPlayer);
            }
            _currDb = "Series";
        }

        private void SetWeekStats()
        {
            IRating currPlayer = _context.WeekRatingDbSet.FirstOrDefault(x => x.Nick == PlayerNick);
            if (currPlayer != null)
            {
                SetStats(currPlayer);
            }
            _currDb = "Week";
        }

        private void SetAllTimeStats()
        {
            IRating currPlayer = _context.AllTimeRatingDbSet.FirstOrDefault(x => x.Nick == PlayerNick);
            if (currPlayer != null)
            {
                SetStats(currPlayer);
            }
            _currDb = "AllTime";
        }

        private void ExportMethod()
        {
            Export(PlayerNick + "_" + _currDb + "_Statistics");
        }
    }
}

