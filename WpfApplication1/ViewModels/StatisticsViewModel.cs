using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using WpfApplication1.Common;
using WpfApplication1.DAL;
using WpfApplication1.Models;
using static WpfApplication1.Common.CommonMethods;

namespace WpfApplication1.ViewModels
{
    internal class StatisticsViewModel : ViewModelBase
    {
        private readonly SaluteDbContext _context = new SaluteDbContext();
        
        public StatisticsViewModel()
        {
            if (SystemParameters.PrimaryScreenHeight*0.9 < 800) ScreenHeight = SystemParameters.PrimaryScreenHeight*0.9;
            else ScreenHeight = 800;
            ScreenWidth = ScreenHeight * 1.10;
            ScreenLeft = (SystemParameters.WorkArea.Width - ScreenWidth)/2 + SystemParameters.WorkArea.Left;
            ScreenTop = (SystemParameters.WorkArea.Height - ScreenHeight)/2 + SystemParameters.WorkArea.Top;
            GetMinGames();
            ShowSeasonStatistics();
        }

        #region Properties

        private double _screenWidth;
        public double ScreenWidth
        {
            get { return _screenWidth; }
            set { Set(nameof(ScreenWidth), ref _screenWidth, value); }
        }

        private double _screenHeight;
        public double ScreenHeight
        {
            get { return _screenHeight; }
            set { Set(nameof(ScreenHeight), ref _screenHeight, value); }
        }

        private double _screenLeft;
        public double ScreenLeft
        {
            get { return _screenLeft; }
            set { Set(nameof(ScreenLeft), ref _screenLeft, value); }
        }

        private double _screenTop;
        public double ScreenTop
        {
            get { return _screenTop; }
            set { Set(nameof(ScreenTop), ref _screenTop, value); }
        }

        public int SeasonMinGames { get; set; }
        public int SeriesMinGames { get; set; }
        public int AllTimeMinGames { get; set; }

        private string _title;
        public string CustomTitle
        {
            get { return _title; }
            set { Set(nameof(CustomTitle), ref _title, value); }
        }

        private string _totalGamesCount;
        public string TotalGamesCount
        {
            get { return _totalGamesCount; }
            set { Set(nameof(TotalGamesCount), ref _totalGamesCount, value); }
        }

        private int _totalPlayersCount;
        public int TotalPlayersCount
        {
            get { return _totalPlayersCount; }
            set { Set(nameof(TotalPlayersCount), ref _totalPlayersCount, value); }
        }

        private int _totalFoulsCount;
        public int TotalFoulsCount
        {
            get { return _totalFoulsCount; }
            set { Set(nameof(TotalFoulsCount), ref _totalFoulsCount, value); }
        }

        private int _totalBansCount;
        public int TotalBansCount
        {
            get { return _totalBansCount; }
            set { Set(nameof(TotalBansCount), ref _totalBansCount, value); }
        }

        private int _bestPlayerCount;
        public int BestPlayerCount
        {
            get { return _bestPlayerCount; }
            set { Set(nameof(BestPlayerCount), ref _bestPlayerCount, value); }
        }

        private int _bestWayTotalCount;
        public int BestWayTotalCount
        {
            get { return _bestWayTotalCount; }
            set { Set(nameof(BestWayTotalCount), ref _bestWayTotalCount, value); }
        }

        private int _threeZvCount;
        public int ThreeZvCount
        {
            get { return _threeZvCount; }
            set { Set(nameof(ThreeZvCount), ref _threeZvCount, value); }
        }

        private string _ugadaykaCount;
        public string UgadaykaCount
        {
            get { return _ugadaykaCount; }
            set { Set(nameof(UgadaykaCount), ref _ugadaykaCount, value); }
        }

        private ObservableCollection<TotalGamesItem> _sourceTotalGames;
        public ObservableCollection<TotalGamesItem> SourceTotalGames
        {
            get { return _sourceTotalGames; }
            set { Set(nameof(SourceTotalGames), ref _sourceTotalGames, value); }
        }

        private ObservableCollection<AbsoluteItem> _gamesCount = new ObservableCollection<AbsoluteItem>();
        public ObservableCollection<AbsoluteItem> GamesCount
        {
            get { return _gamesCount; }
            set { Set(nameof(GamesCount), ref _gamesCount, value); }
        }

        private ObservableCollection<AbsoluteItem> _bestWayCount = new ObservableCollection<AbsoluteItem>();
        public ObservableCollection<AbsoluteItem> BestWayCount
        {
            get { return _bestWayCount; }
            set { Set(nameof(BestWayCount), ref _bestWayCount, value); }
        }

        private ObservableCollection<AverageItem> _winLoseRatioCollection = new ObservableCollection<AverageItem>();
        public ObservableCollection<AverageItem> WinLoseRatioCollection
        {
            get { return _winLoseRatioCollection; }
            set { Set(nameof(WinLoseRatioCollection), ref _winLoseRatioCollection, value); }
        }

        private ObservableCollection<AverageItem> _bestPlayerRatioCollection = new ObservableCollection<AverageItem>();
        public ObservableCollection<AverageItem> BestPlayerRatioCollection
        {
            get { return _bestPlayerRatioCollection; }
            set { Set(nameof(BestPlayerRatioCollection), ref _bestPlayerRatioCollection, value); }
        }

        private ObservableCollection<AverageItem> _foulsAverageCollection = new ObservableCollection<AverageItem>();
        public ObservableCollection<AverageItem> FoulsAverageCollection
        {
            get { return _foulsAverageCollection; }
            set { Set(nameof(FoulsAverageCollection), ref _foulsAverageCollection, value); }
        }

        private ObservableCollection<AverageItem> _killedRatioCollection = new ObservableCollection<AverageItem>();
        public ObservableCollection<AverageItem> KilledRatioCollection
        {
            get { return _killedRatioCollection; }
            set { Set(nameof(KilledRatioCollection), ref _killedRatioCollection, value); }
        }

        private ObservableCollection<AverageItem> _checkedRatioCollection = new ObservableCollection<AverageItem>();
        public ObservableCollection<AverageItem> CheckedRatioCollection
        {
            get { return _checkedRatioCollection; }
            set { Set(nameof(CheckedRatioCollection), ref _checkedRatioCollection, value); }
        }

        private ObservableCollection<AverageItem> _bestFalseComCollection = new ObservableCollection<AverageItem>();
        public ObservableCollection<AverageItem> BestFalseComCollection
        {
            get { return _bestFalseComCollection; }
            set { Set(nameof(BestFalseComCollection), ref _bestFalseComCollection, value); }
        }

        private ObservableCollection<AverageItem> _averageBestSheriff = new ObservableCollection<AverageItem>();
        public ObservableCollection<AverageItem> AverageBestSheriff
        {
            get { return _averageBestSheriff; }
            set { Set(nameof(AverageBestSheriff), ref _averageBestSheriff, value); }
        }

        private ObservableCollection<AverageItem> _averageBestDon = new ObservableCollection<AverageItem>();
        public ObservableCollection<AverageItem> AverageBestDon
        {
            get { return _averageBestDon; }
            set { Set(nameof(AverageBestDon), ref _averageBestDon, value); }
        }

        private PlotModel _totalGames;
        public PlotModel TotalGames
        {
            get { return _totalGames; }
            set { Set(nameof(TotalGames), ref _totalGames, value); }
        }

        private PlotModel _totalGamesExplanation;
        public PlotModel TotalGamesExplanation
        {
            get { return _totalGamesExplanation; }
            set { Set(nameof(TotalGamesExplanation), ref _totalGamesExplanation, value); }
        }

        private PlotModel _ugadayka;
        public PlotModel Ugadayka
        {
            get { return _ugadayka; }
            set { Set(nameof(Ugadayka), ref _ugadayka, value); }
        }

        #endregion

        #region Commands

        public RelayCommand SeasonButton => new RelayCommand(ShowSeasonStatistics);

        public RelayCommand SeriesButton => new RelayCommand(ShowSeriesStatistics);

        public RelayCommand WeekButton => new RelayCommand(ShowWeekStatistics);

        public RelayCommand AllTimeButton => new RelayCommand(ShowAllTimeStatistics);

        public RelayCommand ExportButton => new RelayCommand(ExportMethod);

        #endregion

        public PlotModel GetTotalGames()
        {
            var plot = new PlotModel
            {
                Title = null,
                TitleColor = OxyColor.FromRgb(255,255,255),
                TitlePadding = 0,
                PlotAreaBorderThickness = new OxyThickness(0, 0, 0, 0),
                TitleHorizontalAlignment = TitleHorizontalAlignment.CenteredWithinPlotArea,
                IsLegendVisible = false,
            };
            var ps = new PieSeries
            {
                InsideLabelPosition = 0.7,
                StrokeThickness = 1,
                StartAngle = 0,
                AngleSpan = 360,
                InsideLabelColor = OxyColor.FromRgb(255,255,255),
                FontSize = 10,
                Stroke = OxyColors.Transparent,
                TextColor = OxyColors.White,
            };
            foreach (var p in from i in SourceTotalGames.Where(x => x.Value != 0) where i.Value != null select new PieSlice
            {
                Label = i.Title,
                Value = (double)i.Value,
                Fill = i.FillColor,
            })
            {
                ps.Slices.Add(p);
            }
            plot.Series.Add(ps);
            return plot;
        }

        public PlotModel GetUgadaykaPm(int redwins, int blackwins)
        {
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
                Label = GetLocalized("StRedSlice1") + "\n" + GetLocalized("StRedSlice2"),
                Value = redwins,
                Fill = OxyColors.Red
            };
            var blackslice = new PieSlice
            {
                Label = GetLocalized("StBlackSlice1") + "\n" + GetLocalized("StBlackSlice2"),
                Value = blackwins,
                Fill = OxyColors.Black,
            };
            ps.Slices.Add(redslice);
            ps.Slices.Add(blackslice);
            plot.Series.Add(ps);
            return plot;
        }

        public PlotModel GetTotalGamesExplanation()
        {
            var plot = new PlotModel
            {
                PlotAreaBorderThickness = new OxyThickness(0, 0, 0, 0),
                IsLegendVisible = false,
            };
            plot.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Left,
                IsAxisVisible = false,
                GapWidth = 0.5,
                TextColor = OxyColor.FromRgb(0, 0, 0)
            });
            plot.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                IsAxisVisible = false,
                MinimumPadding = 0,
                AbsoluteMinimum = 0,
                TickStyle = TickStyle.None,
                FontSize = 10,
                TextColor = OxyColor.FromRgb(0, 0, 0)
            });
            var series = new List<BarSeries>();
            for (var i = 0; i < 4; i++)
            {
                series.Add(new BarSeries
                {
                    FillColor = OxyColor.FromRgb(255, 0, 0),
                    LabelFormatString = "{0}",
                    StrokeColor = OxyColors.Transparent,
                    LabelPlacement = LabelPlacement.Middle,
                    TextColor = OxyColor.FromRgb(255, 255, 255),
                    FontWeight = OxyPlot.FontWeights.Bold,
                    LabelMargin = 10,
                    FontSize = 10,
                    BarWidth = 1,
                    IsStacked = true,
                });
            }
            var firsttotalgames = SourceTotalGames[0].Value;
            var secondtotalgames = SourceTotalGames[1].Value;
            var thirdtotalgames = SourceTotalGames[2].Value;
            var fourthtotalgames = SourceTotalGames[3].Value;
            if (firsttotalgames != null && firsttotalgames != 0)
            {
                var red = new BarItem
                {
                    Color = OxyColor.FromRgb(255, 0, 0),
                    Value = (double) firsttotalgames
                };
                series[0].Items.Add(red);
                plot.Series.Add(series[0]);
            }
            if (secondtotalgames != null && secondtotalgames != 0)
            {
                var techred = new BarItem
                {
                    Color = OxyColor.FromRgb(255, 100, 100),
                    Value = (double)secondtotalgames
                };
                series[1].Items.Add(techred);
                plot.Series.Add(series[1]);
            }
            if (thirdtotalgames != null && thirdtotalgames != 0)
            {
                var black = new BarItem
                {
                    Color = OxyColor.FromRgb(0, 0, 0),
                    Value = (double)thirdtotalgames
                };
                series[2].Items.Add(black);
                plot.Series.Add(series[2]);
            }
            if (fourthtotalgames == null || fourthtotalgames == 0) return plot;
            var techblack = new BarItem
            {
                Color = OxyColor.FromRgb(50, 50, 50),
                Value = (double)fourthtotalgames
            };
            series[3].Items.Add(techblack);
            plot.Series.Add(series[3]);
            return plot;
        }

        private void GeneralShowStatistics(IReadOnlyCollection<IRating> dbset, int minGames)
        {
            var winred = dbset.Sum(x => x.WinSheriff);
            var winblack = dbset.Sum(x => x.WinDon);
            var techred = dbset.Sum(x => x.TechRed);
            var techblack = dbset.Sum(x => x.TechBlack);
            TotalGamesCount = $"{GetLocalized("StTotalGames")} {winred + winblack}";
            SourceTotalGames = new ObservableCollection<TotalGamesItem>
            {
                new TotalGamesItem
                {
                    Title = GetLocalized("StRedLabel"),
                    Value = winred - techred / 7,
                    FillColor = OxyColor.FromRgb(255, 0, 0)
                },
                new TotalGamesItem
                {
                    Title = GetLocalized("StRedLabel") + "\n" + GetLocalized("StClearLabel"),
                    Value = techred/7,
                    FillColor = OxyColor.FromRgb(255, 100, 100)
                },
                new TotalGamesItem
                {
                    Title = GetLocalized("StBlackLabel"),
                    Value = winblack - techblack / 3,
                    FillColor = OxyColor.FromRgb(0, 0, 0)
                },
                new TotalGamesItem
                {
                    Title = GetLocalized("StBlackLabel") + "\n" + GetLocalized("StClearLabel"),
                    Value = techblack/3,
                    FillColor = OxyColor.FromRgb(50, 50, 50)
                }
            };
            TotalGames = GetTotalGames();
            TotalGamesExplanation = GetTotalGamesExplanation();
            TotalGamesExplanation = GetTotalGamesExplanation();
            GamesCount = dbset.Where(x => x.Games != 0).OrderByDescending(x => x.Games).Take(10).Select(t => new AbsoluteItem
            {
                Nick = t.Nick.ToStatisticLimitedString(),
                Count = t.Games
            }).ToObservable();
            AverageBestSheriff = dbset.Where(t => t.WinSheriff != null && t.Games > minGames && t.IsMember == true)
                .ToDictionary(t => t.Nick, t => t.WinSheriff != null ? (double) t.WinSheriff/(t.WinSheriff + (t.LoseSheriff ?? 0) ?? 0) * 100 : 0).OrderByDescending(t => t.Value).Take(4).Select(t => new AverageItem
                {
                    Nick = t.Key.ToStatisticLimitedString(),
                    Ratio = t.Value.RoundTwo()
                }).ToObservable();
            AverageBestDon = dbset.Where(t => t.WinDon != null && t.Games > minGames && t.IsMember == true)
                .ToDictionary(t => t.Nick, t => t.WinDon != null ? (double)t.WinDon / (t.WinDon + (t.LoseDon ?? 0) ?? 0) * 100 : 0).OrderByDescending(t => t.Value).Take(4).Select(t => new AverageItem
                {
                    Nick = t.Key.ToStatisticLimitedString(),
                    Ratio = t.Value.RoundTwo()
                }).ToObservable();
            var redu = dbset.Sum(x => x.RedUgadayka) / 2 ?? 0;
            var blacku = dbset.Sum(x => x.BlackUgadayka) ?? 0;
            UgadaykaCount = $"{GetLocalized("StUgadPart1")} {redu + blacku} {GetLocalized("StUgadPart2")}";
            Ugadayka = GetUgadaykaPm(redu, blacku);
            BestWayCount = dbset.Where(t => t.IsMember == true && t.BestChoice > 0).OrderByDescending(x => x.BestChoice).Take(4).Select(t => new AbsoluteItem
            {
                Nick = t.Nick.ToStatisticLimitedString(),
                Count = t.BestChoice ?? 0
            }).ToObservable();
            TotalPlayersCount = dbset.Count;
            TotalFoulsCount = dbset.Sum(x => x.Fouls) ?? 0;
            TotalBansCount = dbset.Sum(x => x.Ban) ?? 0;
            BestPlayerCount = dbset.Sum(x => x.BestPlayer) ?? 0;
            BestWayTotalCount = dbset.Sum(x => x.BestChoice) ?? 0;
            ThreeZvCount = dbset.Sum(x => x.ThreeZv) ?? 0;
            WinLoseRatioCollection = dbset.Where(sr => sr.Win != null && sr.Lose != null && sr.Games >= minGames && sr.IsMember == true)
                .ToDictionary(sr => sr.Nick, sr => sr.Lose != null ? (sr.Win != null ? (double)sr.Win / (double)sr.Lose : 0) : 0)
                .OrderByDescending(t => t.Value).Take(4).Select(t => new AverageItem
                {
                    Nick = t.Key.ToStatisticLimitedString(),
                    Ratio = t.Value.RoundTwo()
                }).ToObservable();
            BestPlayerRatioCollection = dbset.Where(sr => sr.BestPlayer != null && sr.Games >= minGames && sr.IsMember == true)
                    .ToDictionary(sr => sr.Nick, sr => sr.BestPlayer != null ? (double)sr.BestPlayer / sr.Games * 100 : 0)
                    .OrderByDescending(t => t.Value).Take(10).Select(t => new AverageItem
                    {
                        Nick = t.Key.ToStatisticLimitedString(),
                        Ratio = t.Value.RoundTwo()
                    }).ToObservable();
            FoulsAverageCollection = dbset.Where(sr => sr.Fouls != null && sr.Games >= minGames && sr.IsMember == true)
                .ToDictionary(sr => sr.Nick, sr => sr.Fouls != null ? (double)sr.Fouls / sr.Games : 0).OrderBy(t => t.Value).Take(4).Select(t => new AverageItem
                {
                    Nick = t.Key.ToStatisticLimitedString(),
                    Ratio = t.Value.RoundTwo()
                }).ToObservable();
            KilledRatioCollection = dbset.Where(sr => sr.KilledAtFirstDay != null && sr.Games >= minGames && sr.IsMember == true)
                .ToDictionary(sr => sr.Nick, sr => sr.KilledAtFirstDay != null ? (double)sr.KilledAtFirstDay / sr.Games * 100 : 0)
                .OrderByDescending(t => t.Value).Take(4).Select(t => new AverageItem
                {
                    Nick = t.Key.ToStatisticLimitedString(),
                    Ratio = t.Value.RoundTwo()
                    }).ToObservable();
            CheckedRatioCollection = dbset.Where(x => x.CheckedFirst != null && x.Games >= minGames && x.IsMember == true)
                .ToDictionary(t => t.Nick, t => t.CheckedFirst != null ? (double)t.CheckedFirst / t.Games * 100 : 0)
                .OrderByDescending(t => t.Value).Take(4).Select(t => new AverageItem
                {
                    Nick = t.Key.ToStatisticLimitedString(),
                    Ratio = t.Value.RoundTwo()
                }).ToObservable();
            BestFalseComCollection = dbset.Where(t => t.FalseComWin != null && t.Games >= minGames && t.IsMember == true)
                .ToDictionary(t => t.Nick, t => t.FalseComWin != null && t.FalseCom != null && t.FalseCom != 0 ? (double)t.FalseComWin / (t.FalseCom ?? 0) * 100 : 0)
                .OrderByDescending(t => t.Value).Take(4).Select(t => new AverageItem
                {
                    Nick = t.Key.ToStatisticLimitedString(),
                    Ratio = t.Value.RoundTwo()
                }).ToObservable();
        }

        private void ShowSeasonStatistics()
        {
            try
            {
                var iRatingList = Enumerable.Cast<IRating>(_context.SeasonRatingDbSet).ToList();
                GeneralShowStatistics(iRatingList, SeasonMinGames);
            }
            catch (Exception)
            {
                ShowErrorMessage("ERROR", GetLocalized("StSeasonError"));
                var emptyList = new List<IRating>();
                GeneralShowStatistics(emptyList, 1);
            }
            finally
            {
                CustomTitle = GetLocalized("StSeasonTitle");
            }
        }

        private void ShowSeriesStatistics()
        {
            try
            {
                var iRatingList = Enumerable.Cast<IRating>(_context.SeriesRatingDbSet).ToList();
                GeneralShowStatistics(iRatingList, SeriesMinGames);
            }
            catch (Exception)
            {
                ShowErrorMessage("ERROR", GetLocalized("StSeriesError"));
                var emptyList = new List<IRating>();
                GeneralShowStatistics(emptyList, 1);
            }
            finally
            {
                CustomTitle = GetLocalized("StSeriesTitle");
            }
        }

        private void ShowAllTimeStatistics()
        {
            try
            {
                var iRatingList = Enumerable.Cast<IRating>(_context.AllTimeRatingDbSet).ToList();
                GeneralShowStatistics(iRatingList, AllTimeMinGames);
            }
            catch (Exception)
            {
                ShowErrorMessage("ERROR", GetLocalized("StAllTimeError"));
                var emptyList = new List<IRating>();
                GeneralShowStatistics(emptyList, 1);
            }
            finally
            {
                CustomTitle = GetLocalized("StAllTimeTitle");
            }
        }

        private void ShowWeekStatistics()
        {
            try
            {
                var iRatingList = Enumerable.Cast<IRating>(_context.WeekRatingDbSet).ToList();
                GeneralShowStatistics(iRatingList, 1);
            }
            catch (Exception)
            {
                ShowErrorMessage("ERROR", GetLocalized("StWeekError"));
                var emptyList = new List<IRating>();
                GeneralShowStatistics(emptyList, 1);
            }
            finally
            {
                CustomTitle = GetLocalized("StWeekTitle");
            }
        }

        private void GetMinGames()
        {
            try
            {
                var gap = GetGap();
                SeasonMinGames = gap.SeasonMinGames;
                SeriesMinGames = gap.SeriesMinGames;
                AllTimeMinGames = gap.AllTimeMinGames;
            }
            catch (FileNotFoundException)
            {
                ShowErrorMessage("ERROR", GetLocalized("SettingsNotFoundError"));
            }
        }

        private static void ExportMethod()
        {
            Export("StatisticsExport");
        }
    }
}
