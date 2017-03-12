using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using WpfApplication1.DAL;
using WpfApplication1.Models;
using BarSeries = OxyPlot.Series.BarSeries;
using CategoryAxis = OxyPlot.Axes.CategoryAxis;
using LinearAxis = OxyPlot.Axes.LinearAxis;
using static WpfApplication1.Common.CommonMethods;

namespace WpfApplication1.ViewModels
{
    internal class ChartsViewModel : ViewModelBase
    {
        private readonly SaluteDbContext _context = new SaluteDbContext();
        public string CurrShown;
        public ChartsViewModel()
        {
            ScreenHeight = SystemParameters.PrimaryScreenHeight*0.9;
            ScreenWidth = ScreenHeight/1.5;
            ScreenLeft = (SystemParameters.WorkArea.Width - ScreenWidth)/2 + SystemParameters.WorkArea.Left;
            ScreenTop = (SystemParameters.WorkArea.Height - ScreenHeight)/2 + SystemParameters.WorkArea.Top;
            Gap = GetGap();
            SourceCollection = new ObservableCollection<ChartsItem>();
            RatingType = true;
            GetSeasonData();
        }

        #region Properties

        public Gap Gap { get; set; }

        public RelayCommand SeasonButton => new RelayCommand(GetSeasonData);

        public RelayCommand SeriesButton => new RelayCommand(GetSeriesData);

        public RelayCommand AllTimeButton => new RelayCommand(GetAllTimeData);

        public RelayCommand WeekRatingButton => new RelayCommand(GetWeekData);

        public RelayCommand ExportButton => new RelayCommand(ExportToPng);

        public RelayCommand ChangeRatingType => new RelayCommand(ChangeRatingTypeMethod);

        private bool _ratingType;
        public bool RatingType
        {
            get { return _ratingType; }
            set
            {
                if (_ratingType == value) return;
                _ratingType = value;
                ChangeRatingTypeMethod();
                RaisePropertyChanged();
            }
        }

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

        private ObservableCollection<ChartsItem> _sourceCollection;
        public ObservableCollection<ChartsItem> SourceCollection
        {
            get { return _sourceCollection; }
            set { Set(nameof(SourceCollection), ref _sourceCollection, value); }
        }

        private PlotModel _sourcePlotModel;
        public PlotModel SourcePlotModel
        {
            get { return _sourcePlotModel; }
            set { Set(nameof(SourcePlotModel), ref _sourcePlotModel, value); }
        }

        private string _chartsHeader;
        public string ChartsHeader
        {
            get { return _chartsHeader; }
            set { Set(nameof(ChartsHeader), ref _chartsHeader, value); }
        }

        #endregion

        public void GetSeasonData()
        {
            SourceCollection = new ObservableCollection<ChartsItem>();
            try
            {
                if (RatingType)
                {
                    var values =
                        _context.SeasonRatingDbSet.Where(x => x.Rating != null && x.Rating != 0 && x.IsMember == true).OrderBy(x => x.Rating);
                    var invertedvalues = values.OrderByDescending(x => x.Rating);
                    foreach (var s in values)
                        SourceCollection.Add(new ChartsItem
                        {
                            Nick = invertedvalues.ToList().FindIndex(x => x.Nick == s.Nick) + 1 + ". " + s.Nick,
                            Games = s.Games,
                            Score = s.Score,
                            Rating = s.Rating
                        });
                }
                else
                {
                    var values =
                        _context.SeasonRatingFiimDbSet.Where(x => x.Games > Gap.SeasonMinGames && x.Rating != null && x.Rating != 0 && x.IsMember == true).OrderBy(x => x.Rating);
                    var invertedvalues = values.OrderByDescending(x => x.Rating);
                    foreach (var s in values)
                        SourceCollection.Add(new ChartsItem
                        {
                            Nick = invertedvalues.ToList().FindIndex(x => x.Nick == s.Nick) + 1 + ". " + s.Nick,
                            Games = s.Games,
                            Score = s.ScoreMain + s.ScoreAdditional,
                            Rating = s.Rating
                        });
                }
            }
            catch (NotSupportedException)
            {
                ShowErrorMessage("ERROR", GetLocalized("StSeasonError"));
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException)
            {
                ShowErrorMessage("ERROR", GetLocalized("StSeasonError"));
            }
            finally
            {
                SourcePlotModel = InitializePlot();
                CurrShown = "Season";
                ChartsHeader = GetLocalized("ChSeasonHeader");
            }
        }

        public void GetSeriesData()
        {
            SourceCollection = new ObservableCollection<ChartsItem>();
            try
            {
                if (RatingType)
                {
                    var values =
                        _context.SeriesRatingDbSet.Where(x => x.Rating != null && x.Rating != 0 && x.IsMember == true).OrderBy(x => x.Rating);
                    var invertedvalues = values.OrderByDescending(x => x.Rating);
                    foreach (var a in values)
                        SourceCollection.Add(new ChartsItem
                        {
                            Nick = (invertedvalues.ToList().FindIndex(x => x.Nick == a.Nick) + 1) + ". " + a.Nick,
                            Games = a.Games,
                            Score = a.Score,
                            Rating = a.Rating
                        });
                }
                else
                {
                    var values =
                    _context.SeriesRatingFiimDbSet.Where(x => x.Games > Gap.SeriesMinGames && x.Rating != null && x.Rating != 0 && x.IsMember == true).OrderBy(x => x.Rating);
                    var invertedvalues = values.OrderByDescending(x => x.Rating);
                    foreach (var a in values)
                        SourceCollection.Add(new ChartsItem
                        {
                            Nick = (invertedvalues.ToList().FindIndex(x => x.Nick == a.Nick) + 1) + ". " + a.Nick,
                            Games = a.Games,
                            Score = a.ScoreMain + a.ScoreAdditional,
                            Rating = a.Rating
                        });
                }
            }
            catch (NotSupportedException)
            {
                ShowErrorMessage("ERROR", GetLocalized("StSeriesError"));
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException)
            {
                ShowErrorMessage("ERROR", GetLocalized("StSeriesError"));
            }
            finally
            {
                SourcePlotModel = InitializePlot();
                CurrShown = "Series";
                ChartsHeader = GetLocalized("ChSeriesHeader");
            }
        }

        public void GetAllTimeData()
        {
            SourceCollection = new ObservableCollection<ChartsItem>();
            try
            {
                if (RatingType)
                {
                    var values = _context.AllTimeRatingDbSet.Where(x => x.Rating != null && x.Rating != 0 && x.IsMember == true)
                       .OrderBy(x => x.Rating);
                    var invertedvalues = values.OrderByDescending(x => x.Rating);
                    foreach (var a in values)
                        SourceCollection.Add(new ChartsItem
                        {
                            Nick = invertedvalues.ToList().FindIndex(x => x.Nick == a.Nick) + 1 + ". " + a.Nick,
                            Games = a.Games,
                            Score = a.Score,
                            Rating = a.Rating
                        });
                } else {
                    var values = _context.AllTimeRatingFiimDbSet.Where(x => x.Games > Gap.AllTimeMinGames && x.Rating != null && x.Rating != 0 && x.IsMember == true)
                       .OrderBy(x => x.Rating);
                    var invertedvalues = values.OrderByDescending(x => x.Rating);
                    foreach (var a in values)
                        SourceCollection.Add(new ChartsItem
                        {
                            Nick = invertedvalues.ToList().FindIndex(x => x.Nick == a.Nick) + 1 + ". " + a.Nick,
                            Games = a.Games,
                            Score = a.ScoreMain + a.ScoreAdditional,
                            Rating = a.Rating
                        });
                }
            }
            catch (NotSupportedException)
            {
                ShowErrorMessage("ERROR", GetLocalized("StAllTimeError"));
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException)
            {
                ShowErrorMessage("ERROR", GetLocalized("StAllTimeError"));
            }
            finally
            {
                SourcePlotModel = InitializePlot();
                CurrShown = "AllTime";
                ChartsHeader = GetLocalized("ChAllTimeHeader");
            }
        }

        public void GetWeekData()
        {
            SourceCollection = new ObservableCollection<ChartsItem>();
            try
            {
                if (RatingType)
                {
                    var values =
                        _context.WeekRatingDbSet.Where(x => x.Rating != null && x.Rating != 0 && x.IsMember == true).OrderBy(x => x.Rating);
                    var invertedvalues = values.OrderByDescending(x => x.Rating);
                    foreach (var a in values)
                        SourceCollection.Add(new ChartsItem
                        {
                            Nick = invertedvalues.ToList().FindIndex(x => x.Nick == a.Nick) + 1 + ". " + a.Nick,
                            Games = a.Games,
                            Score = a.Score,
                            Rating = a.Rating
                        });
                }
                else
                {
                    var values =
                    _context.WeekRatingFiimDbSet.Where(x => x.Rating != null && x.Rating != 0 && x.IsMember == true).OrderBy(x => x.Rating);
                    var invertedvalues = values.OrderByDescending(x => x.Rating);
                    foreach (var a in values)
                        SourceCollection.Add(new ChartsItem
                        {
                            Nick = invertedvalues.ToList().FindIndex(x => x.Nick == a.Nick) + 1 + ". " + a.Nick,
                            Games = a.Games,
                            Score = a.ScoreMain + a.ScoreAdditional,
                            Rating = a.Rating
                        });
                }
            }
            catch (NotSupportedException)
            {
                ShowErrorMessage("ERROR", GetLocalized("StWeekError"));
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException)
            {
                ShowErrorMessage("ERROR", GetLocalized("StWeekError"));
            }
            finally
            {
                SourcePlotModel = InitializePlot();
                CurrShown = "Week";
                ChartsHeader = GetLocalized("ChWeekHeader");
            }
        }

        public void ChangeRatingTypeMethod()
        {
            switch (CurrShown)
            {
                case "Season":
                    GetSeasonData();
                    break;
                case "Series":
                    GetSeriesData();
                    break;
                case "AllTime":
                    GetAllTimeData();
                    break;
                case "Week":
                    GetWeekData();
                    break;
            }
        }

        public void ExportToPng()
        {
            Export(CurrShown + "_Rating");
        }

        public PlotModel InitializePlot()
        {
            var plot = new PlotModel
            {
                PlotAreaBorderThickness = new OxyThickness(1,0,0,1),
                PlotAreaBorderColor = OxyColors.White,
                IsLegendVisible = false,
            };
            plot.Axes.Add(new CategoryAxis
            {
                Position = AxisPosition.Left,
                ItemsSource = SourceCollection,
                LabelField = "Nick",
                GapWidth = 0,
                TicklineColor = OxyColors.White,
                TextColor = OxyColor.FromRgb(255, 255, 255)
            });
            plot.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                MinimumPadding = 0,
                AbsoluteMinimum = 0,
                TickStyle = TickStyle.None,
                FontSize = 10,
                TextColor = OxyColor.FromRgb(255, 255, 255)
            });
            plot.Series.Add(new BarSeries
            {
                ItemsSource = SourceCollection,
                ValueField = "Rating",
                FillColor = OxyColor.FromRgb(255, 0, 0),
                LabelFormatString = "{0}",
                StrokeColor = OxyColor.FromRgb(255,255,255),
                StrokeThickness = 1,
                LabelPlacement = LabelPlacement.Base,
                TextColor = OxyColor.FromRgb(255, 255, 255),
                LabelMargin = 10,
                FontSize = 12,
                BarWidth = 1,
            });
            return plot;
        }
    }
}
