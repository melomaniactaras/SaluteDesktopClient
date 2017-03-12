using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using WpfApplication1.Models;

namespace WpfApplication1.ViewModels
{
    public partial class MainWindowViewModel
    {
        private readonly DispatcherTimer _audioTimer = new DispatcherTimer();
        private readonly MediaPlayer _audioPlayer = new MediaPlayer();
        private bool _isDragging;

        #region Commands

        public RelayCommand PlayerOpen => new RelayCommand(PlayerOpenMethod);

        public RelayCommand OpenSongs => new RelayCommand(OpenSongsMethod);

        public RelayCommand Play => new RelayCommand(PlayMethod);

        public RelayCommand Pause => new RelayCommand(PauseMethod);

        public RelayCommand Stop => new RelayCommand(StopMethod);

        public RelayCommand DragStarted => new RelayCommand(DragStartedMethod);

        public RelayCommand DragCompleted => new RelayCommand(DragCompletedMethod);

        public RelayCommand NextSong => new RelayCommand(NextSongMethod);

        public RelayCommand PrevSong => new RelayCommand(PrevSongMethod);

        public RelayCommand<KeyEventArgs> KeyDown => new RelayCommand<KeyEventArgs>(KeyDownMethod);

        public RelayCommand<KeyEventArgs> PreviewKeyDown => new RelayCommand<KeyEventArgs>(PreviewKeyDownMethod);

        public RelayCommand<MouseButtonEventArgs> MouseDClick => new RelayCommand<MouseButtonEventArgs>(MouseDoubleClickMethod);

        #endregion

        #region Properties

        private TimeSpan _currentLapsedTime;
        public TimeSpan CurrentLapsedTime
        {
            get { return _currentLapsedTime; }
            set { Set(nameof(CurrentLapsedTime), ref _currentLapsedTime, value); }
        }

        private bool _isPlayerVisible;
        public bool IsPlayerVisible
        {
            get { return _isPlayerVisible; }
            set { Set(nameof(IsPlayerVisible), ref _isPlayerVisible, value); }
        }

        private string _songTitle;
        public string SongTitle
        {
            get { return _songTitle; }
            set
            {
                var newvalue = value.Length > 27 ? value.Substring(0, 25).Trim() + "..." : value;
                if (_songTitle == newvalue) return;
                _songTitle = newvalue;
                RaisePropertyChanged();
            }
        }

        private string _artist;
        public string Artist
        {
            get { return _artist; }
            set { Set(nameof(Artist), ref _artist, value); }
        }

        private double _volumeValue = 1;
        public double VolumeValue
        {
            get { return _volumeValue; }
            set
            {
                if (Math.Abs(_volumeValue - value) < double.Epsilon) return;
                _volumeValue = value;
                _audioPlayer.Volume = value;
                VolumePercent = (int)value * 100;
                RaisePropertyChanged();
            }
        }

        private double _volumePercent = 100.00;
        public double VolumePercent
        {
            get { return _volumePercent; }
            set
            {
                if (Math.Abs(_volumePercent - value) < double.Epsilon) return;
                _volumePercent = value;
                _audioPlayer.Volume = value / 100;
                RaisePropertyChanged();
            }
        }

        private bool _audioPlaying;

        private SongEntry _currentSong = new SongEntry();
        public SongEntry CurrentSong
        {
            get { return _currentSong; }
            set { Set(nameof(CurrentSong), ref _currentSong, value); }
        }

        private ObservableCollection<SongEntry> _songs = new ObservableCollection<SongEntry>();
        public ObservableCollection<SongEntry> Songs
        {
            get { return _songs; }
            set
            {
                if (_songs == value) return;
                _songs = value;
                foreach (var item in value) { item.Index = value.IndexOf(item) + 1; }
                RaisePropertyChanged();
            }
        }

        private double _seekBarValue;
        public double SeekBarValue
        {
            get { return _seekBarValue; }
            set { Set(nameof(SeekBarValue), ref _seekBarValue, value); }
        }

        private double _seekBarMaximum;
        public double SeekBarMaximum
        {
            get { return _seekBarMaximum; }
            set { Set(nameof(SeekBarMaximum), ref _seekBarMaximum, value); }
        }

        #endregion

        private void DragStartedMethod()
        {
            _isDragging = true;
        }

        private void MouseDoubleClickMethod(MouseButtonEventArgs e)
        {
            PlayMethod();
        }

        private void KeyDownMethod(KeyEventArgs e)
        {
            if (e.Key == Key.Delete && Songs.Count != 0)
            {
                try
                {
                    var firstToDelete = Songs.Where(x => x.IsSelected).Min(x => x.Index) - 1;
                    foreach (var x in Songs.ToList().Where(x => x.IsSelected))
                    {
                        Songs.Remove(x);
                    }
                    if (Songs.Count == 0) CurrentSong = new SongEntry();
                    else if (firstToDelete == 0) CurrentSong = Songs[0];
                    else if (Songs.Count == firstToDelete) CurrentSong = Songs[0];
                    else if (Songs.Count > firstToDelete) CurrentSong = Songs[firstToDelete];
                    foreach (var item in Songs)
                    {
                        item.Index = Songs.IndexOf(item) + 1;
                    }
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            else if (e.Key == Key.Enter && CurrentSong != null)
            {
                PlayMethod();
            }
        }

        private void PreviewKeyDownMethod(KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (_audioPlaying)
                {
                    _audioPlayer.Pause();
                    _audioPlaying = false;
                }
                else
                {
                    _audioPlayer.Play();
                    _audioPlaying = true;
                }
            }
            else if (e.Key == Key.A && Keyboard.Modifiers == ModifierKeys.Control)
            {
                foreach (var x in Songs)
                {
                    x.IsSelected = true;
                }
            }
        }

        private void DragCompletedMethod()
        {
            _isDragging = false;
            _audioPlayer.Position = TimeSpan.FromSeconds(SeekBarValue);
        }

        private void PrevSongMethod()
        {
            var currentIndex = Songs.IndexOf(CurrentSong);
            if (Songs.Count != 0 && currentIndex != 0)
            {
                CurrentSong = Songs[currentIndex - 1];
            }
            if (_audioPlaying) PlayMethod();
        }

        private void PlayMethod()
        {
            if (CurrentSong == null)
            {
                CurrentSong = new SongEntry();
                if (Songs.Count > 0) CurrentSong = Songs[0];
            }
            if (!_audioPlaying)
            {
                if (!_audioPlayer.HasAudio || _audioPlayer.Source != CurrentSong.Uri)
                {
                    _audioPlayer.Open(CurrentSong.Uri);
                }
                _audioTimer.Start();
                SongTitle = _currentSong?.SongName ?? string.Empty;
                Artist = _currentSong?.ArtistName ?? string.Empty;
                if (_currentSong?.Duration.TotalSeconds != null)
                    SeekBarMaximum = (double) _currentSong?.Duration.TotalSeconds;
                _audioPlayer.Play();
                _audioPlaying = true;
                return;
            }
            if (_audioPlaying && _audioPlayer.HasAudio && _audioPlayer.Source != CurrentSong.Uri)
            {
                _audioTimer.Stop();
                _audioPlayer.Open(CurrentSong.Uri);
                _audioPlayer.Play();
                _audioTimer.Start();
                return;
            }
            _audioPlaying = false;
            _audioTimer.Stop();
            _audioPlayer.Pause();
        }

        private void PauseMethod()
        {
            if (!_audioPlaying) return;
            _audioPlaying = false;
            _audioTimer.Stop();
            _audioPlayer.Pause();
        }

        private void StopMethod()
        {
            _audioPlayer.Stop();
            _audioTimer.Stop();
            _audioPlaying = false;
            _audioPlayer.Position = TimeSpan.Zero;
            SeekBarValue = 0.0;
        }

        private void OpenSongsMethod()
        {
            var dialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Audio files (*.mp3)|*.mp3|All files (*.*)|*.*",
            };
            if (dialog.ShowDialog() != true) return;
            foreach (var item in dialog.FileNames)
            {
                var tagfile = TagLib.File.Create(item);
                Songs.Add(new SongEntry
                {
                    SongName = string.IsNullOrEmpty(tagfile.Tag.Title) ? tagfile.Name.Substring(tagfile.Name.LastIndexOf("\\", StringComparison.Ordinal) + 1) : tagfile.Tag.Title,
                    ArtistName = tagfile.Tag.Performers.Length > 0 ? tagfile.Tag.Performers[0] : "unknown",
                    Duration = tagfile.Properties.Duration == TimeSpan.Zero ? TimeSpan.Zero : tagfile.Properties.Duration,
                    IsSelected = false,
                    Uri = new Uri(item, UriKind.Relative)
                });
            }
            foreach (var item in Songs) { item.Index = Songs.IndexOf(item) + 1; }
            CurrentSong = Songs[0];
        }

        private bool _isPlayerPinned = true;
        public bool IsPlayerPinned
        {
            get { return _isPlayerPinned; }
            set
            {
                if (_isPlayerPinned == value) return;
                _isPlayerPinned = value;
                if (_isPlayerPinned)
                {
                    AudioPlayer.Top = MainWindowTop + (MainWindowHeight - AudioPlayer.Height);
                    AudioPlayer.Left = MainWindowLeft + MainWindowWidth;
                }
                else
                {
                    AudioPlayer.Left = AudioPlayer.Left + 20;
                    AudioPlayer.Top = AudioPlayer.Top + 20;
                }
                RaisePropertyChanged();
            }
        }

        public void PlayerOpenMethod()
        {
            if (!IsPlayerVisible) return;
            IsPlayerVisible = true;
            AudioPlayer.Top = MainWindowTop + (MainWindowHeight - AudioPlayer.Height);
            AudioPlayer.Left = MainWindowLeft + MainWindowWidth;
            AudioPlayer.Show();
        }

        private void _audioTimer_Tick(object sender, EventArgs e)
        {
            if (_isDragging) return;
            SeekBarValue = _audioPlayer.Position.TotalSeconds;
            CurrentLapsedTime = TimeSpan.FromSeconds(SeekBarMaximum - SeekBarValue);
            if (!(Math.Abs(SeekBarValue - SeekBarMaximum) < 1.00)) return;
            _audioTimer.Stop();
            _audioPlayer.Stop();
            NextSongMethod();
        }

        private void NextSongMethod()
        {
            var currentIndex = Songs.IndexOf(CurrentSong);
            if (Songs?.Count > currentIndex + 1)
            {
                CurrentSong = Songs[currentIndex + 1];
            }
            else if (Songs != null && Songs.Count != 0)
            {
                CurrentSong = Songs[0];
            }
            if (_audioPlaying) PlayMethod();
        }
    }
}
