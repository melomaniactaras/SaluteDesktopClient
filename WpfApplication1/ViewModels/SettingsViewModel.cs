using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Xml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using WpfApplication1.DAL;
using static WpfApplication1.Common.CommonMethods;

namespace WpfApplication1.ViewModels
{
    internal class SettingsViewModel : ViewModelBase
    {
        private readonly SaluteDbContext _context = new SaluteDbContext();
        private readonly MetroWindow _settingswindow = Application.Current.Windows.OfType<MySettings>().FirstOrDefault();
        private readonly MetroDialogSettings _settings = new MetroDialogSettings
        {
            ColorScheme = MetroDialogColorScheme.Accented,
            AnimateShow = true,
            AnimateHide = true
        };
        public SettingsViewModel()
        {
            var gap = GetGap();
            SeasonBal = gap.SeasonMinGames;
            SeriesBal = gap.SeriesMinGames;
            AllTimeBal = gap.AllTimeMinGames;
            NicksCollection = new ObservableCollection<string>(_context.AllTimeRatingDbSet.Select(x => x.Nick).ToList());
            NickBefore = null;
            NickAfter = null;
            Culture = GetCulture();
            ApplyRatio = IsApplyRatio();
        }

        #region Properties

        private bool _applyRatio;
        public bool ApplyRatio
        {
            get { return _applyRatio; }
            set { Set(nameof(_applyRatio), ref _applyRatio, value); }
        }

        private CultureInfo _culture;
        public CultureInfo Culture
        {
            get { return _culture; }
            set { Set(nameof(Culture), ref _culture, value); }
        }

        private bool _autoArchive;
        public bool AutoArchive
        {
            get { return _autoArchive; }
            set { Set(nameof(AutoArchive), ref _autoArchive, value); }
        }

        private int _seasonBal;
        public int SeasonBal
        {
            get { return _seasonBal; }
            set { Set(nameof(SeasonBal), ref _seasonBal, value); }
        }

        private int _seriesBal;
        public int SeriesBal
        {
            get { return _seriesBal; }
            set { Set(nameof(SeriesBal), ref _seriesBal, value); }
        }

        private int _allTimeBal;
        public int AllTimeBal
        {
            get { return _allTimeBal; }
            set { Set(nameof(AllTimeBal), ref _allTimeBal, value); }
        }

        private string _nickBefore;
        public string NickBefore
        {
            get { return _nickBefore; }
            set { Set(nameof(NickBefore), ref _nickBefore, value); }
        }

        private string _nickAfter;
        public string NickAfter
        {
            get { return _nickAfter; }
            set { Set(nameof(NickAfter), ref _nickAfter, value); }
        }

        private string _extraScoreNick;
        public string ExtraScoreNick
        {
            get { return _extraScoreNick; }
            set { Set(nameof(ExtraScoreNick), ref _extraScoreNick, value); }
        }

        private int? _extraScoreValue;
        public int? ExtraScoreValue
        {
            get { return _extraScoreValue; }
            set { Set(nameof(ExtraScoreValue), ref _extraScoreValue, value); }
        }

        private ObservableCollection<string> _nicksCollection;
        public ObservableCollection<string> NicksCollection
        {
            get { return _nicksCollection; }
            set { Set(nameof(NicksCollection), ref _nicksCollection, value); }
        }

        #endregion

        #region Commands

        public RelayCommand ChangeSettings => new RelayCommand(ChangeSettingsMethod);

        public RelayCommand DropSeasonTable => new RelayCommand(DropSeasonTableMethod);

        public RelayCommand DropSeriesTable => new RelayCommand(DropSeriesTableMethod);

        #endregion

        public async void ChangeSettingsMethod()
        {
            var needsRestart = false;
            var dbBeenChanged = false;
            var seasonRate = 0;
            var seriesRate = 0;
            var allTimeRate = 0;
            var xml = new XmlDocument();
            xml.Load($@"{System.IO.Directory.GetCurrentDirectory()}\Settings.xml");
            var seasonBalNode = xml.SelectSingleNode("/Settings/SeasonBal");
            if (seasonBalNode != null) seasonRate = Convert.ToInt32(seasonBalNode.InnerText);
            var seriesBalNode = xml.SelectSingleNode("/Settings/SeriesBal");
            if (seriesBalNode != null) seriesRate = Convert.ToInt32(seriesBalNode.InnerText);
            var allTimeBalNode = xml.SelectSingleNode("/Settings/AllTimeBal");
            if (allTimeBalNode != null) allTimeRate = Convert.ToInt32(allTimeBalNode.InnerText);
            if (seasonRate != 0 && seasonRate != SeasonBal)
            {
                foreach (var s in _context.SeasonRatingDbSet)
                {
                    s.Rating = s.Games < SeasonBal ? 0 : Math.Round((s.Score ?? 0) / s.Games * 100, 3);
                }
                dbBeenChanged = true;
            }
            if (seriesRate != 0 && seriesRate != SeriesBal)
            {
                foreach (var s in _context.SeriesRatingDbSet)
                {
                    s.Rating = s.Games < SeriesBal ? 0 : Math.Round((s.Score ?? 0) / s.Games * 100, 3);
                }
                dbBeenChanged = true;
            }
            if (allTimeRate != 0 && allTimeRate != AllTimeBal)
            {
                foreach (var s in _context.AllTimeRatingDbSet)
                {
                    s.Rating = s.Games < AllTimeBal ? 0 : Math.Round((s.Score ?? 0) / s.Games * 100, 3);
                }
                dbBeenChanged = true;
            }
            if (dbBeenChanged)
                await _settingswindow.ShowMessageAsync("SUCCESS", GetLocalized("DbRefreshedMessage"), MessageDialogStyle.Affirmative, _settings);
            if (seasonBalNode != null) seasonBalNode.InnerText = SeasonBal.ToString();
            if (seriesBalNode != null) seriesBalNode.InnerText = SeriesBal.ToString();
            if (allTimeBalNode != null) allTimeBalNode.InnerText = AllTimeBal.ToString();
            if (NickBefore != NickAfter && !string.IsNullOrEmpty(NickBefore) && !string.IsNullOrEmpty(NickAfter))
            {
                var foundInSeason = _context.SeasonRatingDbSet.FirstOrDefault(x => x.Nick == NickBefore);
                if (foundInSeason != null)
                {
                    foundInSeason.Nick = NickAfter;
                    foundInSeason.NickNormalized = NickAfter.ToLower();
                }
                var foundInSeries = _context.SeriesRatingDbSet.FirstOrDefault(x => x.Nick == NickBefore);
                if (foundInSeries != null)
                {
                    foundInSeries.Nick = NickAfter;
                    foundInSeries.NickNormalized = NickAfter.ToLower();
                }
                var foundInAllTime = _context.AllTimeRatingDbSet.FirstOrDefault(x => x.Nick == NickBefore);
                if (foundInAllTime != null)
                {
                    foundInAllTime.Nick = NickAfter;
                    foundInAllTime.NickNormalized = NickAfter.ToLower();
                }
                var foundInWeek = _context.WeekRatingDbSet.FirstOrDefault(t => t.Nick == NickBefore);
                if (foundInWeek != null)
                {
                    foundInWeek.Nick = NickAfter;
                    foundInWeek.NickNormalized = NickAfter.ToLower();
                }
                await _settingswindow.ShowMessageAsync("SUCCESS", GetLocalized("NickChangedMessage"), MessageDialogStyle.Affirmative,
                    _settings);
            }
            if (!string.IsNullOrEmpty(ExtraScoreNick) && NicksCollection.Contains(ExtraScoreNick) && ExtraScoreValue != null)
            {
                var seasonTarget = _context.SeasonRatingDbSet.FirstOrDefault(t => t.Nick == ExtraScoreNick);
                if (seasonTarget != null)
                {
                    seasonTarget.Score += ExtraScoreValue.Value;
                    seasonTarget.Rating = Math.Round((seasonTarget.Score ?? 0) / seasonTarget.Games * 100, 3);
                    await _settingswindow.ShowMessageAsync("SUCCESS", GetLocalized("PtsAddedMessage"), MessageDialogStyle.Affirmative, _settings);
                }
                var seriesTarget = _context.SeriesRatingDbSet.FirstOrDefault(t => t.Nick == ExtraScoreNick);
                if (seriesTarget != null)
                {
                    seriesTarget.Score += ExtraScoreValue.Value;
                    seriesTarget.Rating = Math.Round((seriesTarget.Score ?? 0) / seriesTarget.Games * 100, 3);
                    await _settingswindow.ShowMessageAsync("SUCCESS", GetLocalized("PtsAddedMessage"), MessageDialogStyle.Affirmative, _settings);
                }
                var weekTarget = _context.WeekRatingDbSet.FirstOrDefault(t => t.Nick == ExtraScoreNick);
                if (weekTarget != null)
                {
                    weekTarget.Score += ExtraScoreValue.Value;
                    weekTarget.Rating = Math.Round((weekTarget.Score ?? 0) / weekTarget.Games * 100, 3);
                    await _settingswindow.ShowMessageAsync("SUCCESS", GetLocalized("PtsAddedMessage"), MessageDialogStyle.Affirmative, _settings);
                }
                var allTimeTarget = _context.AllTimeRatingDbSet.FirstOrDefault(t => t.Nick == ExtraScoreNick);
                if (allTimeTarget != null)
                {
                    allTimeTarget.Score += ExtraScoreValue.Value;
                    allTimeTarget.Rating = Math.Round((allTimeTarget.Score ?? 0) / allTimeTarget.Games * 100, 3);
                    await _settingswindow.ShowMessageAsync("SUCCESS", GetLocalized("PtsAddedMessage"), MessageDialogStyle.Affirmative, _settings);
                }
            }
            if (!Culture.Equals(GetCulture()))
            {
                needsRestart = true;
                var languageNode = xml.SelectSingleNode("/Settings/Language");
                if (languageNode != null) languageNode.InnerText = Culture.Name;
            }
            if (ApplyRatio != IsApplyRatio())
            {
                var ratioNode = xml.SelectSingleNode("/Settings/UseFiimRatio");
                if (ratioNode != null) ratioNode.InnerText = ApplyRatio ? "True" : "False";
            }
            xml.Save($@"{System.IO.Directory.GetCurrentDirectory()}\Settings.xml");
            _context.SaveChanges();
            if (!needsRestart) return;
            var result = await ShowMetroDialog("ADD TO DB", GetLocalized("ChangeLanguageMessage"),
                                MessageDialogStyle.AffirmativeAndNegative)
                                .ConfigureAwait(true);
            if (result == MessageDialogResult.Negative)
            {
                return;
            }
            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        public void DropSeasonTableMethod()
        {
            foreach (var s in _context.SeasonRatingDbSet) _context.SeasonRatingDbSet.Remove(s);
            foreach (var s in _context.SeasonRatingFiimDbSet) _context.SeasonRatingFiimDbSet.Remove(s);
            _context.SaveChanges();
            _settingswindow.ShowMessageAsync("SUCCESS", GetLocalized("SeasonDroppedMessage"), MessageDialogStyle.Affirmative, _settings);
        }

        public void DropSeriesTableMethod()
        {
            foreach (var s in _context.SeriesRatingDbSet) _context.SeriesRatingDbSet.Remove(s);
            foreach (var s in _context.SeriesRatingFiimDbSet) _context.SeriesRatingFiimDbSet.Remove(s);
            _context.SaveChanges();
            _settingswindow.ShowMessageAsync("SUCCESS", GetLocalized("SeriesDroppedMessage"), MessageDialogStyle.Affirmative, _settings);
        }
    }
}
