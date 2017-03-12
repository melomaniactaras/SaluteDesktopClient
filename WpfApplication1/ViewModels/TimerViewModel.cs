using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;

namespace WpfApplication1.ViewModels
{
    public partial class MainWindowViewModel
    {
        private readonly DispatcherTimer _dt = new DispatcherTimer();
        private readonly Stopwatch _stopWatch = new Stopwatch();
        private string _currentTime = string.Empty;
        private readonly MediaPlayer _mediaplayer = new MediaPlayer();

        #region Properties

        private FontWeight _timerFontWeight;
        public FontWeight TimerFontWeight
        {
            get { return _timerFontWeight; }
            set { Set(nameof(TimerFontWeight), ref _timerFontWeight, value); }
        }

        private string _timerText;
        public string TimerText
        {
            get { return _timerText; }
            set { Set(nameof(TimerText), ref _timerText, value); }
        }

        private bool _mute;
        public bool Mute
        {
            get { return _mute; }
            set { Set(nameof(Mute), ref _mute, value); }
        }

        private bool _muteOff;
        public bool MuteOff
        {
            get { return _muteOff; }
            set { Set(nameof(MuteOff), ref _muteOff, value); }
        }

        private bool _timerToggleSwitched;
        public bool TimerToggleSwitched
        {
            get { return _timerToggleSwitched; }
            set { Set(nameof(TimerToggleSwitched), ref _timerToggleSwitched, value); }
        }

        private bool _isTopmost = true;
        public bool IsTopmost
        {
            get { return _isTopmost; }
            set { Set(nameof(IsTopmost), ref _isTopmost, value); }
        }

        private bool _isPinned = true;
        public bool IsPinned
        {
            get { return _isPinned; }
            set
            {
                if (_isPinned == value) return;
                _isPinned = value;
                if (_isPinned)
                {
                    Timer.Top = MainWindowTop;
                    Timer.Left = MainWindowLeft + MainWindowWidth;
                }
                else
                {
                    Timer.Left = Timer.Left + 20;
                    Timer.Top = Timer.Top - 20;
                }
                RaisePropertyChanged();
            }
        }

        #endregion

        #region Commands

        public RelayCommand TimerClear => new RelayCommand(ClearTimer);

        public RelayCommand TimerStartButton => new RelayCommand(StartTimer);

        public RelayCommand TimerStopButton => new RelayCommand(StopTimer);

        public RelayCommand MuteButton => new RelayCommand(MuteButtonMethod);

        public RelayCommand UnMuteButton => new RelayCommand(UnMuteButtonMethod);

        #endregion

        private void MuteButtonMethod()
        {
            Mute = false;
            MuteOff = true;
        }

        private void UnMuteButtonMethod()
        {
            MuteOff = false;
            Mute = true;
        }

        public void dt_Tick(object sender, EventArgs e)
        {
            if (!_stopWatch.IsRunning) return;
            var ts = _stopWatch.Elapsed;
            _currentTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}:{ts.Milliseconds/10:00}";
            if (!TimerToggleSwitched)
            {
                if (ts.Seconds > 50)
                {
                    TimerFontWeight = FontWeights.ExtraBold;
                }
                if (!MuteOff)
                {
                    switch (ts.Seconds)
                    {
                        case 49:
                            _mediaplayer.Open(new Uri(@"Sounds\10.mp3", UriKind.Relative));
                            _mediaplayer.Play();
                            IsTimerVisible = true;
                            break;
                        case 54:
                            _mediaplayer.Open(new Uri(@"Sounds\5.mp3", UriKind.Relative));
                            _mediaplayer.Play();
                            break;
                        case 59:
                            _mediaplayer.Open(new Uri(@"Sounds\Fin.mp3", UriKind.Relative));
                            _mediaplayer.Play();
                            break;
                    }
                }
                if (ts.Minutes == 1)
                {
                    _stopWatch.Stop();
                }
            }
            else
            {
                if (ts.Seconds > 20) { TimerFontWeight = FontWeights.ExtraBold; }
                if (!MuteOff)
                {
                    switch (ts.Seconds)
                    {
                        case 19:
                            _mediaplayer.Open(new Uri(@"Sounds\10.mp3", UriKind.Relative));
                            _mediaplayer.Play();
                            IsTimerVisible = true;
                            break;
                        case 24:
                            _mediaplayer.Open(new Uri(@"Sounds\5.mp3", UriKind.Relative));
                            _mediaplayer.Play();
                            break;
                        case 29:
                            _mediaplayer.Open(new Uri(@"Sounds\Fin.mp3", UriKind.Relative));
                            _mediaplayer.Play();
                            break;
                    }
                }
                if (ts.Seconds == 30) { _stopWatch.Stop(); }
            }
            TimerText = _currentTime;
        }

        public void StartTimer()
        {
            if (_stopWatch.IsRunning) _stopWatch.Stop();
            else
            {
                _stopWatch.Start();
                _dt.Start();
            }
        }

        public void StopTimer()
        {
            _stopWatch.Reset();
            TimerText = "00:00:00:00";
            TimerFontWeight = FontWeights.Regular;
        }

        public void ClearTimer()
        {
            if (!IsTimerVisible) return;
            IsTimerVisible = true;
            Timer.Show();
        }
    }
}
