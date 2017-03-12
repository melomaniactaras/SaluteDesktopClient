using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using WpfApplication1.Common;
using WpfApplication1.DAL;
using WpfApplication1.Models;
using static WpfApplication1.Common.CommonMethods;

namespace WpfApplication1.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        private int _killQueueCurrent = 1;
        public Window1 Timer = new Window1();
        public Player AudioPlayer = new Player();
        public bool IsTimerOpened = false;

        private readonly MetroWindow _mainwindow = Application.Current.MainWindow as MetroWindow;

        private readonly MetroDialogSettings _setting = new MetroDialogSettings
        {
            ColorScheme = MetroDialogColorScheme.Accented,
            AnimateShow = true,
            AnimateHide = true
        };

        private int Miskills { get; set; }

        public MainWindowViewModel()
        {
            var contextCreation = new BackgroundWorker();
            contextCreation.DoWork += (o, ea) =>
            {
                Dispatcher.CurrentDispatcher.Invoke(() =>
                {
                    var context = new SaluteDbContext();
                    var voidselection = from a in context.SeasonRatingDbSet select a;
                    var anothervoidselection = from a in context.SeriesRatingDbSet select a;
                    var justanotherselection = from a in context.AllTimeRatingDbSet select a;
                    var justanotherweekselection = from a in context.WeekRatingDbSet select a;
                });
            };
            contextCreation.RunWorkerAsync();
            Timer.DataContext = this;
            AudioPlayer.DataContext = this;
            SetDefaultState();
        }

        #region Containers

        private ObservableCollection<string> _nicksAutoComplete;

        public ObservableCollection<string> NicksAutoComplete
        {
            get { return _nicksAutoComplete; }
            set { Set(nameof(NicksAutoComplete), ref _nicksAutoComplete, value); }
        }

        private ObservableCollection<string> _onVote;

        public ObservableCollection<string> OnVote
        {
            get { return _onVote; }
            set { Set(nameof(OnVote), ref _onVote, value); }
        }

        private ObservableCollection<string> _notOnVote;

        public ObservableCollection<string> NotOnVote
        {
            get { return _notOnVote; }
            set { Set(nameof(NotOnVote), ref _notOnVote, value); }
        }

        private ObservableCollection<int?> _killedAtNight;

        public ObservableCollection<int?> KilledAtNight
        {
            get { return _killedAtNight; }
            set { Set(nameof(KilledAtNight), ref _killedAtNight, value); }
        }

        private ObservableCollection<bool> _killedAtNightEnabled;

        public ObservableCollection<bool> KilledAtNightEnabled
        {
            get { return _killedAtNightEnabled; }
            set { Set(nameof(KilledAtNightEnabled), ref _killedAtNightEnabled, value); }
        }

        private ObservableCollection<int?> _ugadaykaContainer;

        public ObservableCollection<int?> UgadaykaContainer
        {
            get { return _ugadaykaContainer; }
            set { Set(nameof(UgadaykaContainer), ref _ugadaykaContainer, value); }
        }

        private ObservableCollection<int?> _bestWayContainer;

        public ObservableCollection<int?> BestWayContainer
        {
            get { return _bestWayContainer; }
            set { Set(nameof(BestWayContainer), ref _bestWayContainer, value); }
        }

        private ObservableCollection<PlayerEntry> _players;

        public ObservableCollection<PlayerEntry> Players
        {
            get { return _players; }
            set { Set(nameof(Players), ref _players, value); }
        }

        private ObservableCollection<string> _bestPlayersList;

        public ObservableCollection<string> BestPlayersList
        {
            get { return _bestPlayersList; }
            set { Set(nameof(BestPlayersList), ref _bestPlayersList, value); }
        }

        #endregion

        #region MainWindowProperties

        public bool IsBestWay { get; set; }

        private bool SheriffGoneFirstDay { get; set; }

        private bool _tempFocused;

        public bool TempFocused
        {
            get { return _tempFocused; }
            set { Set(nameof(TempFocused), ref _tempFocused, value); }
        }

        private WindowState _currentWindowState = WindowState.Normal;

        public WindowState CurrentWindowState
        {
            get { return _currentWindowState; }
            set
            {
                if (_currentWindowState == value) return;
                _currentWindowState = value;
                switch (_currentWindowState)
                {
                    case WindowState.Minimized:
                        if (IsPinned && Timer.WindowState != WindowState.Minimized)
                            Timer.WindowState = WindowState.Minimized;
                        if (IsPlayerPinned && AudioPlayer.WindowState != WindowState.Minimized)
                            AudioPlayer.WindowState = WindowState.Minimized;
                        break;
                    case WindowState.Normal:
                        if (IsPinned && Timer.WindowState != WindowState.Normal) Timer.WindowState = WindowState.Normal;
                        if (IsPlayerPinned && AudioPlayer.WindowState != WindowState.Normal)
                            AudioPlayer.WindowState = WindowState.Normal;
                        if (IsPlayerPinned)
                        {
                            AudioPlayer.Height = MainWindowHeight - Timer.ActualHeight;
                            AudioPlayer.Top = MainWindowTop + (MainWindowHeight - AudioPlayer.Height);
                            AudioPlayer.Left = MainWindowLeft + MainWindowWidth;
                        }
                        break;
                }
                RaisePropertyChanged();
            }
        }

        private double _mainWindowHeight;

        public double MainWindowHeight
        {
            get { return _mainWindowHeight; }
            set
            {
                if (Math.Abs(_mainWindowHeight - value) < double.Epsilon) return;
                _mainWindowHeight = value;
                if (Math.Abs(value) > double.Epsilon && Math.Abs(Timer.ActualHeight) > double.Epsilon)
                {
                    if (value - Timer.ActualHeight > 0) AudioPlayer.Height = value - Timer.ActualHeight;
                }
                else AudioPlayer.Height = 420;
                RaisePropertyChanged();
            }
        }

        private double _mainWindowTop;

        public double MainWindowTop
        {
            get { return _mainWindowTop; }
            set
            {
                if (Math.Abs(_mainWindowTop - value) < double.Epsilon) return;
                _mainWindowTop = value;
                if (IsPinned) Timer.Top = MainWindowTop;
                if (IsPlayerPinned) AudioPlayer.Top = MainWindowTop + (MainWindowHeight - AudioPlayer.Height);
                RaisePropertyChanged();
            }
        }

        private double _mainWindowLeft;

        public double MainWindowLeft
        {
            get { return _mainWindowLeft; }
            set
            {
                if (Math.Abs(_mainWindowLeft - value) < double.Epsilon) return;
                _mainWindowLeft = value;
                if (IsPinned) Timer.Left = value + MainWindowWidth;
                if (IsPlayerPinned) AudioPlayer.Left = value + MainWindowWidth;
                RaisePropertyChanged();
            }
        }

        private double _mainWindowWidth;

        public double MainWindowWidth
        {
            get { return _mainWindowWidth; }
            set
            {
                if (Math.Abs(_mainWindowWidth - value) < double.Epsilon) return;
                _mainWindowWidth = value;
                if (IsPinned) Timer.Left = value + MainWindowLeft;
                RaisePropertyChanged();
            }
        }

        private bool _isTimerVisible;

        public bool IsTimerVisible
        {
            get { return _isTimerVisible; }
            set { Set(nameof(IsTimerVisible), ref _isTimerVisible, value); }
        }

        private string _dateNow;

        public string DateNow
        {
            get { return _dateNow; }
            set { Set(nameof(DateNow), ref _dateNow, value); }
        }

        private string _timeNow;

        public string TimeNow
        {
            get { return _timeNow; }
            set { Set(nameof(TimeNow), ref _timeNow, value); }
        }

        private int? _tableNumber;

        public int? TableNumber
        {
            get { return _tableNumber; }
            set { Set(nameof(TableNumber), ref _tableNumber, value); }
        }

        private int? _gameNumber;

        public int? GameNumber
        {
            get { return _gameNumber; }
            set { Set(nameof(GameNumber), ref _gameNumber, value); }
        }

        private string _browserMode;

        public string BrowserMode
        {
            get { return _browserMode; }
            set { Set(nameof(BrowserMode), ref _browserMode, value); }
        }

        private bool _ugadaykaEnabled;

        public bool UgadaykaEnabled
        {
            get { return _ugadaykaEnabled; }
            set { Set(nameof(UgadaykaEnabled), ref _ugadaykaEnabled, value); }
        }

        private bool _techRedWin;

        public bool TechRedWin
        {
            get { return _techRedWin; }
            set
            {
                if (_techRedWin == value) return;
                _techRedWin = value;
                if (_techRedWin) TechBlackWin = false;
                RaisePropertyChanged();
            }
        }

        private bool _techBlackWin;

        public bool TechBlackWin
        {
            get { return _techBlackWin; }
            set
            {
                if (_techBlackWin == value) return;
                _techBlackWin = value;
                if (_techBlackWin) TechRedWin = false;
                RaisePropertyChanged();
            }
        }

        private bool _techBlackEnabled;

        public bool TechBlackEnabled
        {
            get { return _techBlackEnabled; }
            set { Set(nameof(TechBlackEnabled), ref _techBlackEnabled, value); }
        }

        private bool _techRedEnabled;

        public bool TechRedEnabled
        {
            get { return _techRedEnabled; }
            set { Set(nameof(TechRedEnabled), ref _techRedEnabled, value); }
        }

        private bool _redWin;

        public bool RedWin
        {
            get { return _redWin; }
            set
            {
                if (_redWin == value) return;
                _redWin = value;
                if (_redWin)
                {
                    TechBlackEnabled = false;
                    TechRedEnabled = true;
                    TechBlackWin = false;
                }
                RaisePropertyChanged();
            }
        }

        private bool _blackWin;

        public bool BlackWin
        {
            get { return _blackWin; }
            set
            {
                if (_blackWin == value) return;
                _blackWin = value;
                if (_blackWin)
                {
                    TechRedEnabled = false;
                    TechBlackEnabled = true;
                    TechRedWin = false;
                }
                RaisePropertyChanged();
            }
        }

        private bool _isToggleChecked;
        public bool IsToggleChecked
        {
            get { return _isToggleChecked; }
            set
            {
                if (_isToggleChecked == value) return;
                _isToggleChecked = value;
                MiskillEnabled = value;
                RaisePropertyChanged();
            }
        }

        private bool _sheriffFirstNight;

        public bool SheriffFirstNight
        {
            get { return _sheriffFirstNight; }
            set { Set(nameof(SheriffFirstNight), ref _sheriffFirstNight, value); }
        }

        private bool _miskillEnabled;

        public bool MiskillEnabled
        {
            get { return _miskillEnabled; }
            set { Set(nameof(MiskillEnabled), ref _miskillEnabled, value); }
        }

        private bool _threeZv;

        public bool ThreeZv
        {
            get { return _threeZv; }
            set { Set(nameof(ThreeZv), ref _threeZv, value); }
        }

        private string _bestPlayerString;

        public string BestPlayerString
        {
            get { return _bestPlayerString; }
            set { Set(nameof(BestPlayerString), ref _bestPlayerString, value); }
        }

        private bool _isBusyIndicator;

        public bool IsBusyIndicator
        {
            get { return _isBusyIndicator; }
            set { Set(nameof(IsBusyIndicator), ref _isBusyIndicator, value); }
        }

        #endregion

        #region MainWindowMethods

        public void KillQueueChangedMethod(TextChangedEventArgs e)
        {
            foreach (var item in Players.Where(item => item.PositionInKillQueue == 1))
            {
                item.PositionInKillQueue = null;
            }
            if (!e.Changes.Any()) return;
            var textBox = e.Source as TextBox;
            if (textBox == null) return;
            var newvalue = textBox.Text;
            if (newvalue == string.Empty || Players == null || !Players.Any()) return;
            Players[Convert.ToInt32(newvalue) - 1].PositionInKillQueue = 1;
        }

        public void KillButtonVoid(string numberOfButton)
        {
            if (IsToggleChecked)
            {
                if (string.Equals(numberOfButton, "X"))
                {
                    Miskills++;
                    if (_killQueueCurrent < 4)
                    {
                        KilledAtNightEnabled[_killQueueCurrent - 1] = false;
                    }
                    _killQueueCurrent++;
                }
                else
                {
                    var count = Convert.ToInt32(numberOfButton);
                    Players[count].KillButtonVisibilityProperty = false;
                    Players[count].BackgroundColor = (SolidColorBrush) new BrushConverter().ConvertFrom("#5EC9C9C9");
                    Players[count].PositionInKillQueue = _killQueueCurrent;
                    Players[count].KilledAtNight = true;
                    _killQueueCurrent++;

                }
                foreach (var pe in Players)
                {
                    if (pe.PositionInKillQueue == 1) KilledAtNight[0] = Players.IndexOf(pe) + 1;
                    if (pe.PositionInKillQueue == 2) KilledAtNight[1] = Players.IndexOf(pe) + 1;
                    if (pe.PositionInKillQueue == 3) KilledAtNight[2] = Players.IndexOf(pe) + 1;
                }
            }
            else
            {
                var count = Convert.ToInt32(numberOfButton);
                Players[count].BackgroundColor = (SolidColorBrush) new BrushConverter().ConvertFrom("#5EC9C9C9");
                Players[count].KillButtonVisibilityProperty = false;
                Players[count].KilledAtDay = true;
                if (Players[count].Role == GetLocalized("Sheriff") && (_killQueueCurrent == 1 || _killQueueCurrent == 2))
                    SheriffGoneFirstDay = true;
            }
            if (!CheckIfEnd()) return;
            Evaluate();
            IsToggleChecked = true;
        }

        public void OnlyDigitsUgadaykaMethod(object source)
        {
            var count = Convert.ToInt32(source);
            if (UgadaykaContainer[count] == null) return;
            if ((UgadaykaContainer[count] > 10) || (UgadaykaContainer[count] < 1))
            {
                UgadaykaContainer[count] = null;
            }
        }

        public void OnlyDigitsBestWayMethod(object source)
        {
            var count = Convert.ToInt32(source);
            if (BestWayContainer[count] == null) return;
            if ((BestWayContainer[count] > 10) || (BestWayContainer[count] < 1))
            {
                BestWayContainer[count] = null;
            }
        }

        private static void BestWayKeyDownMethod(KeyEventArgs e)
        {
            var source = e.Source as TextBox;
            if (e.Key == Key.Enter)
            {
                source?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        public async void OnFoulValueChanged(object numberOfFoulBox)
        {
            try
            {
                var count = Convert.ToInt32(numberOfFoulBox);
                if (Players[count].Foul != 4) return;
                var result =
                    await
                        ShowMetroDialog(GetLocalized("DisqualifyLabel"), GetLocalized("Disqualify") + (count + 1) + "?",
                                MessageDialogStyle.AffirmativeAndNegative)
                            .ConfigureAwait(false);
                if (result == MessageDialogResult.Affirmative)
                {
                    Players[count].BackgroundColor = Brushes.LightGray;
                    Players[count].KillButtonVisibilityProperty = false;
                    Players[count].KilledAtDay = true;
                    if (!CheckIfEnd()) return;
                    Evaluate();
                    IsToggleChecked = true;
                }
                else Players[count].Foul = 3;
            }
            catch (ArgumentOutOfRangeException)
            {
                foreach (var p in Players) p.Foul = null;
            }
        }

        public void ClearRolesMethod()
        {
            foreach (var player in Players)
            {
                player.Role = null;
                player.RolesAviailable = new ObservableCollection<string> { GetLocalized("Red"), GetLocalized("Black"), GetLocalized("Sheriff"), GetLocalized("Don") };
            }
        }

        public void FillRolesMethod()
        {
            var don = 0;
            var sheriff = 0;
            var mafia = 0;
            foreach (var player in Players)
            {
                if (player.Role == GetLocalized("Don")) don++;
                if (player.Role == GetLocalized("Black")) mafia++;
                if (player.Role == GetLocalized("Sheriff")) sheriff++;
            }
            if ((don != 1) || (sheriff != 1) || (mafia != 2)) return;
            foreach (
                var player in
                Players.Where(player => (player.Role != GetLocalized("Black")) && (player.Role != GetLocalized("Don")) && (player.Role != GetLocalized("Sheriff"))))
                player.Role = GetLocalized("Red");
        }

        public void BestPlayersSetHandler()
        {
            foreach (var p in Players)
            {
                for (var i = 1; i <= 3; i++)
                {
                    if (Players.Any(t => t.Reflection == i) && p.Reflection != i)
                    {
                        p.BestPlayersAvailable.Remove($"{GetLocalized("Best")} {i}");
                    }
                    else if (!p.BestPlayersAvailable.Contains($"{GetLocalized("Best")} {i}"))
                    {
                        ResetBestPlayersList(p, $"{GetLocalized("Best")} {i}");
                    }
                }
            }
        }

        private static void ResetBestPlayersList(PlayerEntry player, string best)
        {
            player.BestPlayersAvailable.Add(best);
            player.BestPlayersAvailable =
                new ObservableCollection<string>(
                    player.BestPlayersAvailable.OrderBy(
                            x => x == " " ? 1 : x == $"{GetLocalized("Best")} 1" ? 2 : x == $"{GetLocalized("Best")} 2" ? 3 : x == $"{GetLocalized("Best")} 3" ? 4 : 5)
                        .ToList());
        }

        public void RolesClickMethod(PlayerEntry player)
        {
            player.IsDropDownOpened = false;
            if (player.Role == null)
            {
                if (player.RolesAviailable.Contains(GetLocalized("Don")))
                {
                    player.Role = GetLocalized("Don");
                }
                else if (player.RolesAviailable.Contains(GetLocalized("Sheriff")))
                {
                    player.Role = GetLocalized("Sheriff");
                }
                else if (player.RolesAviailable.Contains(GetLocalized("Black")))
                {
                    player.Role = GetLocalized("Black");
                }
                else if (player.RolesAviailable.Contains(GetLocalized("Red")))
                {
                    player.Role = GetLocalized("Red");
                }
                return;
            }
            if (player.Role == GetLocalized("Don"))
            {
                if (player.RolesAviailable.Contains(GetLocalized("Sheriff")))
                {
                    player.Role = GetLocalized("Sheriff");
                }
                else if (player.RolesAviailable.Contains(GetLocalized("Black")))
                {
                    player.Role = GetLocalized("Black");
                }
                return;
            }
            if (player.Role == GetLocalized("Sheriff"))
            {
                if (player.RolesAviailable.Contains(GetLocalized("Black")))
                {
                    player.Role = GetLocalized("Black");
                }
                return;
            }
            if (player.Role != GetLocalized("Black")) return;
            if (player.RolesAviailable.Contains(GetLocalized("Red")))
            {
                player.Role = GetLocalized("Red");
            }
        }

        public void ItemsAviailable()
        {
            int sheriff = 0, red = 0, mafia = 0, don = 0;
            for (var i = 0; i < 10; i++)
            {
                if (Players[i].Role == GetLocalized("Sheriff"))
                {
                    sheriff++;
                }
                else if (Players[i].Role == GetLocalized("Red"))
                {
                    red++;
                }
                else if (Players[i].Role == GetLocalized("Don"))
                {
                    don++;
                }
                else if (Players[i].Role == GetLocalized("Black"))
                {
                    mafia++;
                }
            }
            if (red == 6)
            {
                foreach (var player in Players.Where(player => player.Role != GetLocalized("Red")))
                    player.RolesAviailable.Remove(GetLocalized("Red"));
            }
            else
            {
                foreach (
                    var player in
                    Players.Where(player => (player.Role != GetLocalized("Red")) && (!player.RolesAviailable.Contains(GetLocalized("Red")))))
                {
                    player.RolesAviailable.Add(GetLocalized("Red"));
                    player.RolesAviailable =
                        new ObservableCollection<string>(
                            player.RolesAviailable.OrderBy(
                                    x => x == GetLocalized("Red") ? 1 : x == GetLocalized("Black") ? 2 : x == GetLocalized("Sheriff") ? 3 : x == GetLocalized("Don") ? 4 : 5)
                                .ToList());
                }
            }
            if (mafia == 2)
            {
                foreach (var player in Players.Where(player => player.Role != GetLocalized("Black")))
                    player.RolesAviailable.Remove(GetLocalized("Black"));
            }
            else
            {
                foreach (
                    var player in
                    Players.Where(player => (player.Role != GetLocalized("Black")) && (!player.RolesAviailable.Contains(GetLocalized("Black")))))
                {
                    player.RolesAviailable.Add(GetLocalized("Black"));
                    player.RolesAviailable =
                        new ObservableCollection<string>(
                            player.RolesAviailable.OrderBy(
                                    x => x == GetLocalized("Red") ? 1 : x == GetLocalized("Black") ? 2 : x == GetLocalized("Sheriff") ? 3 : x == GetLocalized("Don") ? 4 : 5)
                                .ToList());
                }
            }
            if (sheriff == 1)
            {
                foreach (var player in Players.Where(player => player.Role != GetLocalized("Sheriff")))
                    player.RolesAviailable.Remove(GetLocalized("Sheriff"));
            }
            else
            {
                foreach (
                    var player in
                    Players.Where(player => (player.Role != GetLocalized("Sheriff")) && (!player.RolesAviailable.Contains(GetLocalized("Sheriff")))))
                {
                    player.RolesAviailable.Add(GetLocalized("Sheriff"));
                    player.RolesAviailable =
                        new ObservableCollection<string>(
                            player.RolesAviailable.OrderBy(
                                    x => x == GetLocalized("Red") ? 1 : x == GetLocalized("Black") ? 2 : x == GetLocalized("Sheriff") ? 3 : x == GetLocalized("Don") ? 4 : 5)
                                .ToList());
                }
            }
            if (don == 1)
            {
                foreach (var player in Players.Where(player => player.Role != GetLocalized("Don")))
                    player.RolesAviailable.Remove(GetLocalized("Don"));
            }
            else
            {
                foreach (
                    var player in
                    Players.Where(player => player.Role != GetLocalized("Don") && !player.RolesAviailable.Contains(GetLocalized("Don"))))
                {
                    player.RolesAviailable.Add(GetLocalized("Don"));
                    player.RolesAviailable =
                        new ObservableCollection<string>(
                            player.RolesAviailable.OrderBy(
                                    x => x == GetLocalized("Red") ? 1 : x == GetLocalized("Black") ? 2 : x == GetLocalized("Sheriff") ? 3 : x == GetLocalized("Don") ? 4 : 5)
                                .ToList());
                }
            }
        }

        public void CheckNicksDuplicate()
        {
            NicksAutoComplete = new ObservableCollection<string>(Core.GetNicksFromDb());
            foreach (var player in Players.Where(player => NicksAutoComplete.Contains(player.Nick)))
            {
                NicksAutoComplete.Remove(player.Nick);
            }
        }

        public void RefreshAutoCompleteMethod()
        {
            foreach (var player in Players) player.Nick = null;
            NicksAutoComplete = new ObservableCollection<string>(Core.GetNicksFromDb());
        }

        public void CheckEmptyNickMethod(string currentNick)
        {
            if (string.IsNullOrEmpty(currentNick)) CheckNicksDuplicate();
        }

        public void ClearApplicationMethod()
        {
            _audioTimer.Stop();
            _audioPlaying = false;
            _audioPlayer.Stop();
            if (Songs != null && Songs.Count != 0) XmlPlaylist.SavePlaylist(Songs);
            TempFocused = true;
            SetDefaultState();
            StopTimer();
        }

        public void SetDefaultState()
        {
            _dt.Tick += dt_Tick;
            _dt.Interval = new TimeSpan(0, 0, 0, 0, 1);
            NicksAutoComplete = new ObservableCollection<string>(Core.GetNicksFromDb());
            TechBlackWin = false;
            TechRedWin = false;
            RedWin = false;
            BlackWin = false;
            SheriffFirstNight = false;
            BestPlayerString = $"{GetLocalized("Best")}: ";
            UgadaykaEnabled = false;
            ThreeZv = false;
            DateNow = $"{GetLocalized("Date")}: {DateTime.Now.ToShortDateString()}";
            TimeNow = $"{GetLocalized("Time")}: {DateTime.Now.ToShortTimeString()}";
            IsToggleChecked = true;
            Players = new ObservableCollection<PlayerEntry>();
            for (var i = 0; i < 10; i++)
            {
                Players.Add(new PlayerEntry());
            }
            UgadaykaContainer = new ObservableCollection<int?> {null, null, null};
            BestWayContainer = new ObservableCollection<int?> {null, null, null};
            KilledAtNight = new ObservableCollection<int?> {null, null, null};
            KilledAtNightEnabled = new ObservableCollection<bool> {true, true, true};
            TableNumber = null;
            GameNumber = null;
            IsTimerVisible = false;
            _killQueueCurrent = 1;
            NotOnVote = new ObservableCollection<string> {"1", "2", "3", "4", "5", "6", "7", "8", "9", "10"};
            OnVote = new ObservableCollection<string>();
            BrowserMode = null;
            IsBestWay = false;
            IsBusyIndicator = false;
            SheriffGoneFirstDay = false;
            DonCheckSuccess = false;
            MuteOff = true;
            Mute = false;
            Miskills = 0;
            FirstCheckedByDon = null;
            FirstCheckedBySheriff = null;
            FalseCom = null;
            SecondCheckedBySheriff = null;
            IsPlayerVisible = false;
            _audioTimer.Interval = TimeSpan.FromMilliseconds(800);
            _audioTimer.Tick += _audioTimer_Tick;
            _audioPlayer.Volume = 1;
            var savedPlaylist = XmlPlaylist.GetLastPlaylist();
            if (savedPlaylist != null && savedPlaylist.Count != 0)
                Songs = new ObservableCollection<SongEntry>(savedPlaylist);
        }

        public void ApplicationCloseMethod()
        {
            if (Songs != null && Songs.Count != 0) XmlPlaylist.SavePlaylist(Songs);
            Application.Current.Shutdown();
        }

        public void CountResultMethod()
        {
            foreach (var p in Players) p.Result = null;
            BestPlayerString = string.Empty;
            Evaluate();
        }

        public void Evaluate()
        {
            if (!(RedWin || BlackWin)) return;
            Core.ResultCount(Players, RedWin, BlackWin);
            IsBestWay = Core.BestWayCount(Players, BestWayContainer);
            Core.FoulCheck(Players);
            Core.ThreeZvCount(Players, ThreeZv);
            Core.DonChecks(Players, DonCheckSuccess, BlackWin);
            Core.CheckUgadayka(UgadaykaEnabled, Players, UgadaykaContainer, RedWin, BlackWin);
            SheriffFirstNight = Core.CheckFirstKill(Players, BlackWin);
            Core.CheckReflect(Players);
            if (FalseCom != null && FalseCom != 0)
            {
                var falseSheriff = Players[FalseCom.Value - 1];
                if (falseSheriff.IsRedTeam())
                {
                    _mainwindow.ShowMessageAsync("ERROR", GetLocalized("WrongFalseComMessage"), MessageDialogStyle.Affirmative, _setting);
                }
                else
                {
                    Core.CheckFalseCom(Players, BlackWin, FalseCom);
                }
            }
            if (TechRedWin || TechBlackWin)
                Core.TechnicalCheck(Players, TechRedWin, TechBlackWin);
            if (SheriffGoneFirstDay)
            {
                var sheriff = Players.FirstOrDefault(t => t.Role == GetLocalized("Sheriff"));
                if (sheriff != null) sheriff.Result = sheriff.Result - 1;
            }
            BestPlayerString = $"{GetLocalized("BestPlayer")}: ";
            foreach (var p in Players.Where(p => p.Reflection >= 0 && (p.Foul < 4 || p.Foul == null)))
                BestPlayerString += p.Nick + " ";
        }

        public async Task<MessageDialogResult> ShowMetroDialog(string title, string message, MessageDialogStyle style)
        {
            return await _mainwindow.ShowMessageAsync(title, message, style, _setting);
        }

        public bool CheckIfEnd()
        {
            if (Miskills == 3)
            {
                RedWin = true;
                TechRedWin = true;
                return true;
            }
            var results = Core.CheckResult(Players);
            BlackWin = results.BlackWin;
            TechBlackWin = results.TechnicalBlackWin;
            RedWin = results.RedWin;
            TechRedWin = results.TechnicalRedWin;
            if (!results.Ugadayka) return BlackWin || RedWin;
            UgadaykaEnabled = true;
            UgadaykaContainer[0] =
                Players.IndexOf(Players.First(x => (x.KilledAtDay == null) && (x.KilledAtNight == null))) + 1;
            UgadaykaContainer[1] =
                Players.IndexOf(
                    Players.Where(x => (x.KilledAtDay == null) && (x.KilledAtNight == null)).Skip(1).Take(1).First()) +
                1;
            UgadaykaContainer[2] =
                Players.IndexOf(
                    Players.Where(x => (x.KilledAtDay == null) && (x.KilledAtNight == null)).Skip(2).Take(1).First()) +
                1;
            return BlackWin || RedWin;
        }

        public void SaveToFileMethod()
        {
            if (Players.Count(x => string.IsNullOrEmpty(x.Nick)) != 0)
            {
                _mainwindow.ShowMessageAsync("ERROR", GetLocalized("WrongNicksMessage"), MessageDialogStyle.Affirmative, _setting);
                return;
            }
            var dlg = new SaveFileDialog
            {
                FileName =
                    $"Protocol_{DateTime.Now.ToShortDateString()}_{TableNumber}_{GameNumber}_{DateTime.Now.ToShortTimeString().Replace(":", "")}",
                DefaultExt = ".xml",
                Filter = "Xml files (*.xml)|*.xml"
            };
            var result = dlg.ShowDialog();
            if (result != true)
                return;
            var fileName = dlg.FileName;
            var saveFile = XmlLibrary.AddPlayers(Players, DateNow, TimeNow, TableNumber, GameNumber, BestWayContainer,
                KilledAtNight, UgadaykaEnabled,
                UgadaykaContainer, RedWin, TechRedWin, BlackWin, TechBlackWin, SheriffFirstNight, ThreeZv, false, FalseCom);
            saveFile.Save(fileName);
        }

        public void AuthomaticallySaveToFile()
        {
            var filename =
                $@"{Directory.GetCurrentDirectory()}\SavedProtocols\Protocol_{DateTime.Now.ToShortDateString()
                    .Replace("/", "")}_{DateTime.Now.ToShortTimeString().Replace("/", "").Replace(":", "-")}_{TableNumber}_{GameNumber}.xml";
            var savefile = XmlLibrary.AddPlayers(Players, DateNow, TimeNow, TableNumber, GameNumber, BestWayContainer,
                KilledAtNight, UgadaykaEnabled,
                UgadaykaContainer, RedWin, TechRedWin, BlackWin, TechBlackWin, SheriffFirstNight, ThreeZv, true, FalseCom);
            savefile.Save(filename);
        }

        public void LoadFromFileMethod()
        {
            var dlg = new OpenFileDialog
            {
                InitialDirectory = $@"{Directory.GetCurrentDirectory()}\SavedProtocols",
                DefaultExt = ".xml",
                Filter = "XML documents (.xml)|*.xml"
            };
            var result = dlg.ShowDialog();
            if (result != true)
                return;
            TempFocused = true;
            SetDefaultState();
            var filePath = dlg.FileName;
            var outList = XmlLibrary.XmlRead(filePath, Players, BestWayContainer,
                KilledAtNight, UgadaykaContainer);
            int tryParseVal;
            if (outList.ContainsKey("Time")) TimeNow = outList["Time"];
            if (outList.ContainsKey("Date")) DateNow = outList["Date"];
            if (outList.ContainsKey("Game"))
                GameNumber = int.TryParse(outList["Game"], out tryParseVal) ? tryParseVal : (int?) null;
            if (outList.ContainsKey("Table"))
                TableNumber = int.TryParse(outList["Table"], out tryParseVal) ? tryParseVal : (int?) null;
            if (outList.ContainsKey("UgadaykaOn") && outList["UgadaykaOn"] == "True") UgadaykaEnabled = true;
            if (outList.ContainsKey("GameResult"))
            {
                switch (outList["GameResult"])
                {
                    case "Red":
                        RedWin = true;
                        break;
                    case "Black":
                        BlackWin = true;
                        break;
                    case "RedTech":
                        RedWin = true;
                        TechRedWin = true;
                        break;
                    case "BlackTech":
                        BlackWin = true;
                        TechBlackWin = true;
                        break;
                }
            }
            if (outList.ContainsKey("FirstNightSheriff") && outList["FirstNightSheriff"] == "True")
                SheriffFirstNight = true;
            if (outList.ContainsKey("ThreeZv") && outList["ThreeZv"] == "True") ThreeZv = true;
            if (outList.ContainsKey("BestPlayer")) BestPlayerString = outList["BestPlayer"];
            if (outList.ContainsKey("FalseCom")) FalseCom = int.TryParse(outList["FalseCom"], out tryParseVal) ? tryParseVal : (int?)null;
            BrowserMode = "Browsing file " + filePath;
            MuteOff = true;
            Mute = false;
        }

        public void AddToVoteMethod(string current)
        {
            NotOnVote.Remove(current);
            OnVote.Add(current);
        }

        public void DeleteFromVoteMethod(string current)
        {
            OnVote.Remove(current);
            NotOnVote.Add(current);
            NotOnVote = new ObservableCollection<string>(NotOnVote.OrderBy(Convert.ToInt32).ToList());
        }

        public void ClearVoteListMethod()
        {
            NotOnVote = new ObservableCollection<string> {"1", "2", "3", "4", "5", "6", "7", "8", "9", "10"};
            OnVote = new ObservableCollection<string>();
        }

        public async void SaveToDbMethod()
        {
            try
            {
                if (Players.Count(x => (x.Nick == null) || (x.Nick == string.Empty)) != 0)
                {
                    await
                        _mainwindow.ShowMessageAsync("ERROR", GetLocalized("WrongNicksMessage"), MessageDialogStyle.Affirmative,
                            _setting);
                    return;
                }
                var duplicatingNicks = Players.GroupBy(x => x.Nick.ToLower()).Select(y => y.FirstOrDefault()).ToList();
                if (duplicatingNicks.Count != 10)
                {
                    await
                        _mainwindow.ShowMessageAsync("ERROR", GetLocalized("SameNicksMessage"), MessageDialogStyle.Affirmative,
                            _setting);
                    return;
                }
                if (Players.Where(x => string.IsNullOrEmpty(x.Role)).ToList().Count != 0)
                {
                    await
                        _mainwindow.ShowMessageAsync("ERROR", GetLocalized("WrongRolesMessage"), MessageDialogStyle.Affirmative,
                            _setting);
                    return;
                }
                if (Players.Where(x => x.Result == null).ToList().Count != 0)
                {
                    await
                        _mainwindow.ShowMessageAsync("ERROR", GetLocalized("WrongResultsMessage"),
                            MessageDialogStyle.Affirmative, _setting);
                    return;
                }
                if (!RedWin && !BlackWin)
                {
                    await
                        _mainwindow.ShowMessageAsync("ERROR", GetLocalized("SelectWinnerMessage"),
                            MessageDialogStyle.Affirmative, _setting);
                    return;
                }
            }
            catch (Exception)
            {
                await _mainwindow.ShowMessageAsync("ERROR", GetLocalized("Error"), MessageDialogStyle.Affirmative, _setting);
                return;
            }
            var nicksInDb = Core.GetNicksFromDb();
            var newNicksToAdd = Players.Where(player => !nicksInDb.Contains(player.Nick)).ToList();
            if (newNicksToAdd.Count != 0)
            {
                var outNewNicks = new StringBuilder();
                foreach (var newNick in newNicksToAdd)
                {
                    outNewNicks.AppendLine(newNick.Nick);
                }
                var result =
                    await
                        ShowMetroDialog("ADD TO DB", $"{GetLocalized("AddNewPlayersMessage")}\n\n" + outNewNicks,
                                MessageDialogStyle.AffirmativeAndNegative)
                            .ConfigureAwait(true);
                if (result == MessageDialogResult.Negative)
                {
                    return;
                }
            }
            var finalresult =
                await
                    ShowMetroDialog("SAVE TO DB", GetLocalized("AddToDbMessage"), MessageDialogStyle.AffirmativeAndNegative)
                        .ConfigureAwait(true);
            if (finalresult == MessageDialogResult.Affirmative)
            {
                try
                {
                    int minSeason;
                    var s = $@"{Directory.GetCurrentDirectory()}\Settings.xml";
                    var xml = new XmlDocument();
                    xml.Load(s);
                    var seasonMinValue = xml.SelectSingleNode("/Settings/SeasonBal");
                    var seriesMinValue = xml.SelectSingleNode("/Settings/SeriesBal");
                    if (seasonMinValue != null && seriesMinValue != null)
                    {
                        int.TryParse(seasonMinValue.InnerText, out minSeason);
                    }
                    else
                    {
                        await
                            _mainwindow.ShowMessageAsync("ERROR", GetLocalized("SettingsNotFoundError"),
                                MessageDialogStyle.Affirmative, _setting);
                        return;
                    }
                    var worker = new BackgroundWorker();
                    worker.DoWork += (o, ea) =>
                    {
                        Dispatcher.CurrentDispatcher.Invoke(() =>
                        {
                            SaluteRepository.DbUpdate(Players, IsBestWay, RedWin, BlackWin, TechRedWin, TechBlackWin,
                                UgadaykaContainer, UgadaykaEnabled, ThreeZv, minSeason, FalseCom);
                            SaluteRepository.SetPosition(Players);
                            SaluteRepository.FiimDbUpdate(Core.FiimResultCount(Players, RedWin, BlackWin, BestWayContainer), RedWin, BlackWin);
                            if (IsApplyRatio())
                            {
                                SaluteRepository.ApplyRatio();
                            }
                        });
                    };
                    worker.RunWorkerCompleted += async (o, ea) =>
                    {
                        IsBusyIndicator = false;
                        var newprotocolresult =
                            await ShowMetroDialog("SUCCESS", $"{GetLocalized("DbSuccessMessage")}\n{GetLocalized("CreateNewProtocolMessage")}",
                                        MessageDialogStyle.AffirmativeAndNegative)
                                    .ConfigureAwait(true);
                        if (newprotocolresult != MessageDialogResult.Affirmative) return;
                        TempFocused = true;
                        if (Songs != null && Songs.Count != 0) XmlPlaylist.SavePlaylist(Songs);
                        SetDefaultState();
                        StopTimer();
                        MuteOff = true;
                        Mute = false;
                    };
                    IsBusyIndicator = true;
                    worker.RunWorkerAsync();
                    AuthomaticallySaveToFile();
                }
                catch (FileNotFoundException)
                {
                    await
                        _mainwindow.ShowMessageAsync("ERROR", GetLocalized("SettingsNotFoundError"), MessageDialogStyle.Affirmative,
                            _setting);
                }
            }
        }

        public void OpenDbMethod()
        {
            var db = new DBBrowser {DataContext = new DbBrowserViewModel()};
            db.Show();
        }

        public async void NewWeekMethod()
        {
            try
            {
                var context = new SaluteDbContext();
                foreach (var s in context.WeekRatingDbSet) context.WeekRatingDbSet.Remove(s);
                foreach (var s in context.WeekRatingFiimDbSet) context.WeekRatingFiimDbSet.Remove(s);
                var dropweektable =
                    await
                        ShowMetroDialog("NEW WEEK", GetLocalized("StartNewWeekMessage"),
                            MessageDialogStyle.AffirmativeAndNegative).ConfigureAwait(true);
                if (dropweektable != MessageDialogResult.Affirmative) return;
                context.SaveChanges();
                await
                    _mainwindow.ShowMessageAsync("NEW WEEK STARTED", GetLocalized("NewWeekSuccessMessage"),
                        MessageDialogStyle.Affirmative, _setting);
            }
            catch (NotSupportedException)
            {
                await
                    _mainwindow.ShowMessageAsync("NEW WEEK ERROR", GetLocalized("NewWeekErrorMessage"),
                        MessageDialogStyle.Affirmative, _setting);
            }
            catch (System.Data.Entity.Core.EntityCommandExecutionException)
            {
                await
                    _mainwindow.ShowMessageAsync("NEW WEEK ERROR", GetLocalized("NewWeekErrorMessage"),
                        MessageDialogStyle.Affirmative, _setting);
            }
        }

        public async void BackupDbMethod()
        {
            try
            {
                var path = $@"{Directory.GetCurrentDirectory()}\DB\databaseFile.db3";
                var outDir = $@"{Directory.GetCurrentDirectory()}\DBArchive";
                var outPath =
                    $@"{Directory.GetCurrentDirectory()}\DBArchive\databaseFile{DateTime.Now.ToShortDateString()
                        .Replace("/", "")}.db3";
                if (!Directory.Exists(outDir))
                {
                    Directory.CreateDirectory(outDir);
                }
                var counter = 1;
                while (File.Exists(outPath))
                {
                    outPath =
                        $@"{Directory.GetCurrentDirectory()}\DBArchive\databaseFile{DateTime.Now.ToShortDateString()
                            .Replace("/", "")}({counter}).db3";
                    counter++;
                }
                File.Copy(path, outPath, true);
                await
                    _mainwindow.ShowMessageAsync("BACKUP DB", $"{GetLocalized("DbCopiedMessage")} {outPath}",
                        MessageDialogStyle.Affirmative, _setting);
            }
            catch (Exception ex)
            {
                await
                    _mainwindow.ShowMessageAsync("BACKUP ERROR", $"{GetLocalized("DbCopyErrorMessage")}" + ex.Message,
                        MessageDialogStyle.Affirmative, _setting);
            }
        }

        public void ShowGraphsMethod()
        {
            var c = new Charts();
            c.Show();
        }

        public void CreateInfographicsMethod()
        {
            if (Application.Current.Windows.OfType<Infographics>().FirstOrDefault() != null) return;
            SimpleIoc.Default.Register<InfographicsViewModel>();
            var info = new Infographics();
            info.Show();
        }

        public void ShowSettingsMethod()
        {
            try
            {
                var xml = new XmlDocument();
                xml.Load($@"{Directory.GetCurrentDirectory()}\Settings.xml");
                var seasonBalNode = xml.SelectSingleNode("/Settings/SeasonBal");
                var seriesBalNode = xml.SelectSingleNode("/Settings/SeriesBal");
                if (seasonBalNode == null || seriesBalNode == null) return;
                var ms = new MySettings {Owner = _mainwindow};
                ms.Show();
            }
            catch (FileNotFoundException)
            {
                _mainwindow.ShowMessageAsync("ERROR", GetLocalized("SettingsNotFoundError"), MessageDialogStyle.Affirmative,
                    _setting);
            }
        }

        public void ShowStatisticsMethod()
        {
            var st = new Statistics();
            st.Show();
        }

        #endregion

        #region CheckWindow

        private string _checkWindowHeader;
        public string CheckWindowHeader
        {
            get { return _checkWindowHeader; }
            set { Set(nameof(CheckWindowHeader), ref _checkWindowHeader, value); }
        }

        private int? _firstCheckedBySheriff;
        public int? FirstCheckedBySheriff
        {
            get { return _firstCheckedBySheriff; }
            set { Set(nameof(FirstCheckedBySheriff), ref _firstCheckedBySheriff, value); }
        }

        private int? _secondCheckedBySheriff;
        public int? SecondCheckedBySheriff
        {
            get { return _secondCheckedBySheriff; }
            set { Set(nameof(SecondCheckedBySheriff), ref _secondCheckedBySheriff, value); }
        }

        private int? _firstCheckedByDon;
        public int? FirstCheckedByDon
        {
            get { return _firstCheckedByDon; }
            set { Set(nameof(FirstCheckedByDon), ref _firstCheckedByDon, value); }
        }

        private int? _falseCom;
        public int? FalseCom
        {
            get { return _falseCom; }
            set { Set(nameof(FalseCom), ref _falseCom, value); }
        }

        private bool _donVisibility;
        public bool DonVisibility
        {
            get { return _donVisibility; }
            set { Set(nameof(DonVisibility), ref _donVisibility, value); }
        }

        private bool _sheriffVisibility;
        public bool SheriffVisibility
        {
            get { return _sheriffVisibility; }
            set { Set(nameof(SheriffVisibility), ref _sheriffVisibility, value); }
        }

        private bool DonCheckSuccess { get; set; }

        private bool _cancelVisibility;
        public bool CancelVisibility
        {
            get { return _cancelVisibility; }
            set { Set(nameof(CancelVisibility), ref _cancelVisibility, value); }
        }

        private bool _applyVisibility;
        public bool ApplyVisibility
        {
            get { return _applyVisibility; }
            set { Set(nameof(ApplyVisibility), ref _applyVisibility, value); }
        }

        private ObservableCollection<Brush> _borderBrushes;
        public ObservableCollection<Brush> BorderBrushes
        {
            get { return _borderBrushes; }
            set { Set(nameof(BorderBrushes), ref _borderBrushes, value); }
        }

        private CheckWindow _cw;

        private void ShowDonCheckMethod()
        {
            CheckWindowHeader = GetLocalized("DonCheckHeader");
            DonVisibility = true;
            SheriffVisibility = false;
            ShowCheck();
        }

        private void ShowSheriffCheckMethod()
        {
            CheckWindowHeader = GetLocalized("SheriffCheckHeader");
            SheriffVisibility = true;
            DonVisibility = false;
            ShowCheck();
        }

        private void ShowCheck()
        {
            _mainwindow.Opacity = 0.4;
            _cw = new CheckWindow
            {
                DataContext = this,
                Owner = _mainwindow
            };
            BorderBrushes = new ObservableCollection<Brush>();
            for (var i = 0; i < 10; i++)
            {
                BorderBrushes.Add(Brushes.White);
            }
            ApplyVisibility = false;
            CancelVisibility = false;
            _cw.ShowDialog();
        }

        private void CheckOkButtonMethod()
        {
            if (FirstCheckedBySheriff != null)
            {
                var firstChecked = Players.FirstOrDefault(x => x.CheckedAtNight == 1);
                if (firstChecked != null) firstChecked.CheckedAtNight = null;
                Players[(int)FirstCheckedBySheriff - 1].CheckedAtNight = 1;
            }
            if (SecondCheckedBySheriff != null)
            {
                var secondChecked = Players.FirstOrDefault(x => x.CheckedAtNight == 2);
                if (secondChecked != null) secondChecked.CheckedAtNight = null;
                Players[(int) SecondCheckedBySheriff - 1].CheckedAtNight = 2;
            }
            if (FirstCheckedByDon != null && Players[(int)FirstCheckedByDon - 1].Role == GetLocalized("Sheriff"))
            {
                DonCheckSuccess = true;
            }
            _cw.Close();
            _mainwindow.Opacity = 1;
        }

        private void CheckButtonPressMethod(string commParameter)
        {
            var pressedNumber = 0;
            if (commParameter != null) { pressedNumber = int.Parse(commParameter); }
            if (CheckWindowHeader == GetLocalized("SheriffCheckHeader"))
            {
                Players[pressedNumber].CheckedAtNight = _killQueueCurrent - 1;
                if (Players[pressedNumber].IsBlackTeam())
                {
                    BorderBrushes[pressedNumber] = Brushes.Green;
                    ApplyVisibility = true;
                }
                else
                {
                    BorderBrushes[pressedNumber] = Brushes.Red;
                    CancelVisibility = true;
                }
            }
            else if (CheckWindowHeader == GetLocalized("DonCheckHeader"))
            {
                if (Players[pressedNumber].Role == GetLocalized("Sheriff"))
                {
                    BorderBrushes[pressedNumber] = Brushes.Green;
                    ApplyVisibility = true;
                    DonCheckSuccess = true;
                }
                else
                {
                    BorderBrushes[pressedNumber] = Brushes.Red;
                    CancelVisibility = true;
                }
            }
        }

        #endregion
    }
}
