using System;
using GalaSoft.MvvmLight;

namespace WpfApplication1.Models
{
    public class SongEntry : ObservableObject
    {
        private int _index;
        public int Index
        {
            get { return _index; }
            set { Set(nameof(Index), ref _index, value); }
        }

        private string _songName;
        public string SongName
        {
            get { return _songName; }
            set { Set(nameof(SongName), ref _songName, value); }
        }

        private string _artistName;
        public string ArtistName
        {
            get { return _artistName; }
            set { Set(nameof(ArtistName), ref _artistName, value); }
        }

        private Uri _uri;
        public Uri Uri
        {
            get { return _uri; }
            set { Set(nameof(Uri), ref _uri, value); }
        }

        private TimeSpan _duration;
        public TimeSpan Duration
        {
            get { return _duration; }
            set { Set(nameof(Duration), ref _duration, value); }
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { Set(nameof(IsSelected), ref _isSelected, value); }
        }
    }
}
