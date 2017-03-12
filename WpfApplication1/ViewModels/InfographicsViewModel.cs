using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using OxyPlot;
using OxyPlot.Series;
using WpfApplication1.Annotations;
using WpfApplication1.Common;
using WpfApplication1.DAL;
using WpfApplication1.Models;
using static WpfApplication1.Common.CommonMethods;

namespace WpfApplication1.ViewModels
{
    public class InfographicsViewModel : ViewModelBase
    {
        private InfographicsCreation _ic;

        private readonly MetroWindow _infoWindow = Application.Current.Windows.OfType<Infographics>().FirstOrDefault();

        private readonly MetroDialogSettings _settings = new MetroDialogSettings
        {
            ColorScheme = MetroDialogColorScheme.Accented,
            AnimateShow = true,
            AnimateHide = true
        };

        public InfographicsViewModel()
        {
            GamesForWeek = null;
            if (SystemParameters.PrimaryScreenHeight * 0.9 < 800) ScaleValue = SystemParameters.PrimaryScreenHeight * 0.9 / 2268;
            else ScaleValue = 0.42;
            GetOffRecords();
            GetLeader();
            GetRatings();
            AllNicks = new ObservableCollection<string>(Core.GetNicksFromDb());
            CreateInfographics();
        }

        #region SizeAndScaleProperties

        private bool _ratingType = true;
        public bool RatingType
        {
            get { return _ratingType; }
            set
            {
                if (_ratingType == value) return;
                _ratingType = value;
                GetRatings();
                CreateInfographics();
                GetLeader();
                RaisePropertyChanged();
            }
        }

        private double _infoHeight = 2268;
        public double InfoHeight
        {
            get { return _infoHeight; }
            set { Set(nameof(InfoHeight), ref _infoHeight, value); }
        }

        private double _infoWidth = 1237;
        public double InfoWidth
        {
            get { return _infoWidth; }
            set { Set(nameof(InfoWidth), ref _infoWidth, value); }
        }

        private double _scaleValue = 0.4;
        public double ScaleValue
        {
            get { return _scaleValue; }
            set { Set(nameof(ScaleValue), ref _scaleValue, value); }
        }

        private double _scaleRatingField = 1;
        public double ScaleRatingField
        {
            get { return _scaleRatingField; }
            set { Set(nameof(ScaleRatingField), ref _scaleRatingField, value); }
        }

        private double _scaleAchievementsField = 1;
        public double ScaleAchievementsField
        {
            get { return _scaleAchievementsField; }
            set { Set(nameof(ScaleAchievementsField), ref _scaleAchievementsField, value); }
        }

        #endregion

        #region Properties

        private string _series;
        public string Series
        {
            get { return _series; }
            set { Set(nameof(Series), ref _series, value); }
        }

        private int _redWind;
        public int RedWin
        {
            get { return _redWind; }
            set { Set(nameof(RedWin), ref _redWind, value); }
        }

        private int _blackWin;
        public int BlackWin
        {
            get { return _blackWin; }
            set { Set(nameof(BlackWin), ref _blackWin, value); }
        }

        private int _totalGames;
        public int TotalGames
        {
            get { return _totalGames; }
            set { Set(nameof(TotalGames), ref _totalGames, value); }
        }

        private int _playersCount;
        public int PlayersCount
        {
            get { return _playersCount; }
            set { Set(nameof(PlayersCount), ref _playersCount, value); }
        }

        private int SeasonMinGames { get; set; }

        private int SeriesMinGames { get; set; }

        #endregion

        #region Containers

        private ObservableCollection<string> _leaders = new ObservableCollection<string>();
        public ObservableCollection<string> Leaders
        {
            get { return _leaders; }
            set { Set(nameof(Leaders), ref _leaders, value); }
        }

        private ObservableCollection<DateTime> _dates = new ObservableCollection<DateTime>();
        public ObservableCollection<DateTime> Dates
        {
            get { return _dates; }
            set { Set(nameof(Dates), ref _dates, value); }
        }

        private ObservableCollection<string> _allSeries = new ObservableCollection<string> { GetLocalized("Winter"), GetLocalized("Spring"), GetLocalized("Summer"), GetLocalized("Autumn") };
        public ObservableCollection<string> AllSeries
        {
            get { return _allSeries; }
            set { Set(nameof(AllSeries), ref _allSeries, value); }
        }

        private ObservableCollection<string> _firstOffRating = new ObservableCollection<string>();
        public ObservableCollection<string> FirstOffRating
        {
            get { return _firstOffRating; }
            set { Set(nameof(FirstOffRating), ref _firstOffRating, value); }
        }

        private ObservableCollection<string> _secondOffRating = new ObservableCollection<string>();
        public ObservableCollection<string> SecondOffRating
        {
            get { return _secondOffRating; }
            set { Set(nameof(SecondOffRating), ref _secondOffRating, value); }
        }

        private ObservableCollection<string> _thirdOffRating = new ObservableCollection<string>();
        public ObservableCollection<string> ThirdOffRating
        {
            get { return _thirdOffRating; }
            set { Set(nameof(ThirdOffRating), ref _thirdOffRating, value); }
        }

        private ObservableCollection<string> _fourthOffRating = new ObservableCollection<string>();
        public ObservableCollection<string> FourthOffRating
        {
            get { return _fourthOffRating; }
            set { Set(nameof(FourthOffRating), ref _fourthOffRating, value); }
        }

        private ObservableCollection<string> _fifthOffRating = new ObservableCollection<string>();
        public ObservableCollection<string> FifthOffRating
        {
            get { return _fifthOffRating; }
            set { Set(nameof(FifthOffRating), ref _fifthOffRating, value); }
        }

        private ObservableCollection<Models.RatingItem> _seasonRating = new ObservableCollection<Models.RatingItem>();
        public ObservableCollection<Models.RatingItem> SeasonRating
        {
            get { return _seasonRating; }
            set { Set(nameof(SeasonRating), ref _seasonRating, value); }
        }

        private ObservableCollection<Models.RatingItem> _seriesRating = new ObservableCollection<Models.RatingItem>();
        public ObservableCollection<Models.RatingItem> SeriesRating
        {
            get { return _seriesRating; }
            set { Set(nameof(SeriesRating), ref _seriesRating, value); }
        }

        private ObservableCollection<AchievementEntry> _achievements = new ObservableCollection<AchievementEntry>();
        public ObservableCollection<AchievementEntry> Achievements
        {
            get { return _achievements; }
            set { Set(nameof(Achievements), ref _achievements, value); }
        }

        private ObservableCollection<AchievementEntry> _comboWf = new ObservableCollection<AchievementEntry>();
        public ObservableCollection<AchievementEntry> ComboWf
        {
            get { return _comboWf; }
            set { Set(nameof(ComboWf), ref _comboWf, value); }
        }

        private ObservableCollection<AchievementEntry> _comboWin = new ObservableCollection<AchievementEntry>();
        public ObservableCollection<AchievementEntry> ComboWin
        {
            get { return _comboWin; }
            set { Set(nameof(ComboWin), ref _comboWin, value); }
        }

        private ObservableCollection<AchievementEntry> _epicWf = new ObservableCollection<AchievementEntry>();
        public ObservableCollection<AchievementEntry> EpicWf
        {
            get { return _epicWf; }
            set { Set(nameof(EpicWf), ref _epicWf, value); }
        }

        private ObservableCollection<AchievementEntry> _epicWin = new ObservableCollection<AchievementEntry>();
        public ObservableCollection<AchievementEntry> EpicWin
        {
            get { return _epicWin; }
            set { Set(nameof(EpicWin), ref _epicWin, value); }
        }

        private ObservableCollection<bool?> _leadersGender = new ObservableCollection<bool?> { null, null, null, null, null, null };
        public ObservableCollection<bool?> LeadersGender
        {
            get { return _leadersGender; }
            set { Set(nameof(LeadersGender), ref _leadersGender, value); }
        }

        private ObservableCollection<string> _allNicks = new ObservableCollection<string>();
        public ObservableCollection<string> AllNicks
        {
            get { return _allNicks; }
            set { Set(nameof(AllNicks), ref _allNicks, value); }
        }

        private PlotModel _gamesForWeek = new PlotModel();
        public PlotModel GamesForWeek
        {
            get { return _gamesForWeek; }
            set { Set(nameof(GamesForWeek), ref _gamesForWeek, value); }
        }

        private ObservableCollection<int> _offRating = new ObservableCollection<int>();
        public ObservableCollection<int> OffRating
        {
            get { return _offRating; }
            set { Set(nameof(OffRating), ref _offRating, value); }
        }

        #endregion

        #region RelayCommands

        public SaluteDbContext Context { get; set; } = new SaluteDbContext();

        public RelayCommand InfographicsOk => new RelayCommand(InfographicsOkMethod);

        public RelayCommand CreateButton => new RelayCommand(CreateButtonMethod);

        public RelayCommand ExportButton => new RelayCommand(ExportInfographicsMethod);

        public RelayCommand InfographicsClosing => new RelayCommand(InfographicsClosingMethod);

        public GalaSoft.MvvmLight.Command.RelayCommand<KeyEventArgs> ComboWfKeyPressed => new GalaSoft.MvvmLight.Command.RelayCommand<KeyEventArgs>(ComboWfKeyPressedMethod);

        public GalaSoft.MvvmLight.Command.RelayCommand<KeyEventArgs> ComboWinKeyPressed => new GalaSoft.MvvmLight.Command.RelayCommand<KeyEventArgs>(ComboWinKeyPressedMethod);

        public GalaSoft.MvvmLight.Command.RelayCommand<KeyEventArgs> EpicWfKeyPressed => new GalaSoft.MvvmLight.Command.RelayCommand<KeyEventArgs>(EpicWfKeyPressedMethod);

        public GalaSoft.MvvmLight.Command.RelayCommand<KeyEventArgs> EpicWinKeyPressed => new GalaSoft.MvvmLight.Command.RelayCommand<KeyEventArgs>(EpicWinKeyPressedMethod);

        public GalaSoft.MvvmLight.Command.RelayCommand<string> ClearAchievements => new GalaSoft.MvvmLight.Command.RelayCommand<string>(ClearAchievementsMethod); 

        public GalaSoft.MvvmLight.CommandWpf.RelayCommand<SelectedDatesCollection> CalendarDatesChanged => new GalaSoft.MvvmLight.CommandWpf.RelayCommand<SelectedDatesCollection>(CalendarDatesChangedMethod);

        #endregion

        #region Methods

        private void CreateInfographics()
        {
            try
            {
                RedWin = Context.WeekRatingDbSet.Sum(x => x.WinSheriff) ?? 0;
                BlackWin = Context?.WeekRatingDbSet?.Sum(x => x.WinDon) ?? 0;
                TotalGames = RedWin + BlackWin;
                var playersCount = Context.WeekRatingDbSet.Count();
                CreatePlotModel(RedWin, BlackWin, playersCount);
            }
            catch (NotSupportedException)
            {
                CreatePlotModel(0, 0, 0);
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException)
            {
                CreatePlotModel(0, 0, 0);
            }
        }

        private void CreatePlotModel(int redWin, int blackWin, int playersCount)
        {
            var plot = new PlotModel
            {
                Title = null,
                TitleColor = OxyColor.FromRgb(255, 255, 255),
                TitlePadding = 0,
                PlotAreaBorderThickness = new OxyThickness(0, 0, 0, 0),
                TitleHorizontalAlignment = TitleHorizontalAlignment.CenteredWithinPlotArea,
                IsLegendVisible = false,
            };
            var ps = new PieSeries
            {
                InsideLabelPosition = 0.5,
                StrokeThickness = 1,
                StartAngle = 0,
                AngleSpan = 360,
                OutsideLabelFormat = "",
                InsideLabelColor = OxyColor.FromRgb(255, 255, 255),
                FontSize = 28,
                TickLabelDistance = 0,
                Stroke = OxyColors.Transparent,
                TextColor = OxyColors.White,
            };
            var redslice = new PieSlice
            {
                Label = $"{redWin}",
                Value = redWin,
                Fill = OxyColors.Red
            };
            var blackslice = new PieSlice
            {
                Label = $"{blackWin}",
                Value = blackWin,
                Fill = OxyColors.Black,
            };
            ps.Slices.Add(redslice);
            ps.Slices.Add(blackslice);
            plot.Series.Add(ps);
            GamesForWeek = plot;
            PlayersCount = playersCount;
        }

        private void ExportInfographicsMethod()
        {
            var dlg = new SaveFileDialog
            {
                FileName = $"Infographics__{DateTime.Now.ToShortDateString().Replace("/", "")}",
                DefaultExt = ".png",
                Filter = "Png files (*.png)|*.png"
            };
            var result = dlg.ShowDialog();
            if (result != true) return;
            var screen = Application.Current.Windows.OfType<Infographics>()
                .SingleOrDefault(x => x.IsActive);
            using (var stream = File.Open(dlg.FileName, FileMode.OpenOrCreate))
            {
                var w = (int)Math.Round(screen.Width * 1 / ScaleValue, 0);
                var h = (int)Math.Round(screen.Height * 1 / ScaleValue, 0);
                var visual = new DrawingVisual();
                using (var context = visual.RenderOpen())
                {
                    context.DrawRectangle(Brushes.Black, null, new Rect(new Point(), new Size(screen.Width, screen.Height)));
                    context.DrawRectangle(new VisualBrush(screen), null,
                        new Rect(new Point(), new Size(screen.Width, screen.Height)));
                }
                visual.Transform = new ScaleTransform(w / screen.ActualWidth, h / screen.ActualHeight);
                var rtb = new RenderTargetBitmap(w, h, 96, 96, PixelFormats.Pbgra32);
                rtb.Render(visual);
                var enc = new PngBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(rtb));
                enc.Save(stream);
            }
            screen.ShowMessageAsync("SUCCESS", "Exporting complete", MessageDialogStyle.Affirmative, _settings);
        }

        private void ClearAchievementsMethod(string parameter)
        {
            if (string.IsNullOrEmpty(parameter)) return;
            switch (parameter)
            {
                case "ComboWf":
                    ComboWf.Clear();
                    foreach (var item in Achievements.Where(x => x.TypeOfAchievements == AchievementsType.ComboFw).ToList())
                    {
                        Achievements.Remove(item);
                    }
                    if (Achievements.Count <= 9) ScaleAchievementsField = 1;
                    return;
                case "ComboWin":
                    ComboWin.Clear();
                    foreach (var item in Achievements.Where(x => x.TypeOfAchievements == AchievementsType.ComboWin).ToList())
                    {
                        Achievements.Remove(item);
                    }
                    if (Achievements.Count <= 9) ScaleAchievementsField = 1;
                    return;
                case "EpicWf":
                    EpicWf.Clear();
                    foreach (var item in Achievements.Where(x => x.TypeOfAchievements == AchievementsType.EpicFw).ToList())
                    {
                        Achievements.Remove(item);
                    }
                    if (Achievements.Count <= 9) ScaleAchievementsField = 1;
                    return;
                case "EpicWin":
                    EpicWin.Clear();
                    foreach (var item in Achievements.Where(x => x.TypeOfAchievements == AchievementsType.EpicWin).ToList())
                    {
                        Achievements.Remove(item);
                    }
                    if (Achievements.Count <= 9) ScaleAchievementsField = 1;
                    return;
                default:
                    return;
            }
        }

        private void CountScale()
        {
            if (Achievements.Count <= 2)
            {
                ScaleAchievementsField = 1.5;
            }
            else if (Achievements.Count > 4 && Achievements.Count <= 9)
            {
                ScaleAchievementsField = 1;
            }
            else if (Achievements.Count > 9 && Achievements.Count <= 16)
            {
                ScaleAchievementsField = 0.77;
            }
            else if (Achievements.Count > 16 && Achievements.Count <= 21)
            {
                ScaleAchievementsField = 0.50;
            }
        }

        private void CreateButtonMethod()
        {
            _ic = new InfographicsCreation { Owner = _infoWindow };
            _ic.Show();
        }

        private void GetLeader()
        {
            Leaders = new ObservableCollection<string>();
            if (RatingType)
            {
                var seasonLeaders =
                    Context.SeasonRatingDbSet.Where(x => x.Games >= SeasonMinGames && x.IsMember == true)
                        .OrderByDescending(x => x.Rating)
                        .ToList();
                if (seasonLeaders.Count < 3) return;
                for (var i = 0; i < 3; i++)
                {
                    Leaders.Add(seasonLeaders[i].Nick);
                }
            }
            else
            {
                var seasonLeaders = Context.SeasonRatingFiimDbSet.Where(x => x.Games >= SeasonMinGames && x.IsMember == true).OrderByDescending(x => x.Rating).ToList();
                if (seasonLeaders.Count < 3) return;
                for (var i = 0; i < 3; i++)
                {
                    Leaders.Add(seasonLeaders[i].Nick);
                }
            }
        }

        private void GetRatings()
        {
            List<Models.RatingItem> season;
            if (RatingType)
            {
                var seasonRatings =
                    Context.SeasonRatingDbSet.Where(x => x.Games >= SeasonMinGames && x.IsMember == true)
                        .OrderByDescending(x => x.Rating)
                        .ToList();
                season = seasonRatings.Select(item => new Models.RatingItem
                {
                    Nick = item.Nick,
                    Games = item.Games,
                    Score = item.Score ?? 0,
                    Rating = item.Rating ?? 0,
                    Width = 0
                }).ToList();
            }
            else
            {
                var seasonRatings =
                    Context.SeasonRatingFiimDbSet.Where(x => x.Games >= SeasonMinGames && x.IsMember == true)
                        .OrderByDescending(x => x.Rating)
                        .ToList();
                season = seasonRatings.Select(item => new Models.RatingItem
                {
                    Nick = item.Nick,
                    Games = item.Games,
                    Score = (item.ScoreMain ?? 0) + item.ScoreAdditional,
                    Rating = item.Rating ?? 0,
                    Width = 0
                }).ToList();
            }
            SeasonRating = new ObservableCollection<Models.RatingItem>(season);
            List<Models.RatingItem> series;
            if (RatingType)
            {
                var seriesRatings =
                    Context.SeriesRatingDbSet.Where(x => x.Games >= SeriesMinGames && x.IsMember == true)
                        .OrderByDescending(x => x.Rating)
                        .ToList();
                series = seriesRatings.Select(item => new Models.RatingItem
                {
                    Nick = item.Nick,
                    Games = item.Games,
                    Score = item.Score ?? 0,
                    Rating = item.Rating ?? 0,
                    Width = 0
                }).ToList();
            }
            else
            {
                var seriesRatings =
                    Context.SeasonRatingFiimDbSet.Where(x => x.Games >= SeriesMinGames && x.IsMember == true)
                        .OrderByDescending(x => x.Rating)
                        .ToList();
                series = seriesRatings.Select(item => new Models.RatingItem
                {
                    Nick = item.Nick,
                    Games = item.Games,
                    Score = (item.ScoreMain ?? 0) + item.ScoreAdditional,
                    Rating = item.Rating ?? 0,
                    Width = 0
                }).ToList();
            }
            SeriesRating = new ObservableCollection<Models.RatingItem>(series);
            GetLength(SeasonRating);
            GetLength(SeriesRating);
            GetScaleRatingValue();
        }

        private void GetLength(IEnumerable<Models.RatingItem> source)
        {
            var enumerable = source as Models.RatingItem[] ?? source.ToArray();
            if (!enumerable.Any()) return;
            var ratingItems = source as Models.RatingItem[] ?? enumerable.ToArray();
            var max = ratingItems.Max(x => x.Rating);
            var koef = (InfoWidth / 2 - InfoWidth / 6) / max;
            foreach (var item in ratingItems)
            {
                item.Width = item.Rating * koef;
            }
        }

        private void GetScaleRatingValue()
        {
            var difference = SeasonRating.Count - 37;
            if (difference >= 1)
            {
                ScaleRatingField = 1 - difference * 0.025;
            }
        }

        private void GetOffRecords()
        {
            try
            {
                var xml = new XmlDocument();
                xml.Load($@"{Directory.GetCurrentDirectory()}\Settings.xml");
                var seasonBalNode = xml.SelectSingleNode("/Settings/SeasonBal");
                if (seasonBalNode != null) SeasonMinGames = Convert.ToInt32(seasonBalNode.InnerText);
                var seriesBalNode = xml.SelectSingleNode("/Settings/SeriesBal");
                if (seriesBalNode != null) SeriesMinGames = Convert.ToInt32(seriesBalNode.InnerText);
                var offRatingSource = Context.SeasonRatingDbSet.Where(x => x.Games < SeasonMinGames && x.IsMember == true).GroupBy(x => x.Games).OrderByDescending(x => x.Key).Take(5).ToList();
                foreach (var item in offRatingSource)
                {
                    OffRating.Add(item.Key);
                }
                if (OffRating.Count > 0)
                {
                    var firstDigit = OffRating[0];
                    foreach (var item in Context.SeasonRatingDbSet.Where(item => item.Games == firstDigit && item.IsMember == true))
                    {
                        FirstOffRating.Add(item.Nick);
                    }
                }
                if (OffRating.Count > 1)
                {
                    var secondDigit = OffRating[1];
                    foreach (var item in Context.SeasonRatingDbSet.Where(item => item.Games == secondDigit && item.IsMember == true))
                    {
                        SecondOffRating.Add(item.Nick);
                    }
                }
                if (OffRating.Count > 2)
                {
                    var thirdDigit = OffRating[2];
                    foreach (var item in Context.SeasonRatingDbSet.Where(item => item.Games == thirdDigit && item.IsMember == true))
                    {
                        ThirdOffRating.Add(item.Nick);
                    }
                }
                if (OffRating.Count > 3)
                {
                    var fourthDigit = OffRating[3];
                    foreach (var item in Context.SeasonRatingDbSet.Where(item => item.Games == fourthDigit && item.IsMember == true))
                    {
                        FourthOffRating.Add(item.Nick);
                    }
                }
                if (OffRating.Count > 4)
                {
                    var fifthDigit = OffRating[4];
                    foreach (var item in Context.SeasonRatingDbSet.Where(item => item.Games == fifthDigit && item.IsMember == true))
                    {
                        FifthOffRating.Add(item.Nick);
                    }
                }
            }
            catch (FileNotFoundException)
            {
                _infoWindow.Focus();
                _infoWindow.ShowMessageAsync("ERROR", GetLocalized("SettingsNotFoundError"),
                    MessageDialogStyle.Affirmative, _settings);
            }
        }

        #endregion

        #region EventHandlers

        private static void InfographicsClosingMethod()
        {
            SimpleIoc.Default.Unregister<InfographicsViewModel>();
        }

        private void CalendarDatesChangedMethod(SelectedDatesCollection source)
        {
            Dates.Clear();
            foreach (var item in source)
            {
                Dates.Add(item.Date);
            }
        }

        private void ComboWfKeyPressedMethod([CanBeNull] KeyEventArgs e)
        {
            if (e?.Key != Key.Enter) return;
            var entry = new AchievementEntry
            {
                Nick = (e.Source as AutoCompleteBox)?.Text,
                TypeOfAchievements = AchievementsType.ComboFw
            };
            Achievements.Add(entry);
            ComboWf.Add(entry);
            ((AutoCompleteBox)e.Source).Text = string.Empty;
            CountScale();
        }

        private void ComboWinKeyPressedMethod(KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            var entry = new AchievementEntry
            {
                Nick = (e.Source as AutoCompleteBox)?.Text,
                TypeOfAchievements = AchievementsType.ComboWin
            };
            Achievements.Add(entry);
            ComboWin.Add(entry);
            ((AutoCompleteBox)e.Source).Text = string.Empty;
            CountScale();
        }

        private void EpicWfKeyPressedMethod(KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            var nick = (e.Source as AutoCompleteBox)?.Text;
            var entry = new AchievementEntry
            {
                Nick = nick,
                TypeOfAchievements = AchievementsType.EpicFw,
                WinSeries = Context.SeasonRatingDbSet.FirstOrDefault(t => t.Nick == nick)?.WinRow ?? 0
            };
            Achievements.Add(entry);
            EpicWf.Add(entry);
            ((AutoCompleteBox)e.Source).Text = string.Empty;
            CountScale();
        }

        private void EpicWinKeyPressedMethod(KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            var entry = new AchievementEntry
            {
                Nick = (e.Source as AutoCompleteBox)?.Text,
                TypeOfAchievements = AchievementsType.EpicWin
            };
            Achievements.Add(entry);
            EpicWin.Add(entry);
            ((AutoCompleteBox)e.Source).Text = string.Empty;
            CountScale();
        }

        private void InfographicsOkMethod()
        {
            _ic.Close();
        }

        #endregion
    }
}
