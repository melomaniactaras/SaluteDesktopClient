using System.Collections.ObjectModel;
using System.Windows.Media;
using GalaSoft.MvvmLight;
using static WpfApplication1.Common.CommonMethods;

namespace WpfApplication1.Models
{
    public class PlayerEntry : ObservableObject
    {
        private string _nick;
        public string Nick
        {
            get { return _nick; }
            set { Set(nameof(Nick), ref _nick, value); }
        }

        private string _role;
        public string Role
        {
            get { return _role; }
            set { Set(nameof(Role), ref _role, value); }
        }

        private int? _foul;
        public int? Foul
        {
            get { return _foul; }
            set { Set(nameof(Foul), ref _foul, value); }
        }

        private int? _reflection;
        public int? Reflection
        {
            get { return _reflection; }
            set
            {
                if (_reflection == value) return;
                if ((value > 0) && (value < 11)) _reflection = value;
                else _reflection = null;
                RaisePropertyChanged();
            }
        }

        private double? _result;
        public double? Result
        {
            get { return _result; }
            set { Set(nameof(Result), ref _result, value); }
        }

        private int? _positionInKillQueue;
        public int? PositionInKillQueue
        {
            get { return _positionInKillQueue; }
            set { Set(nameof(PositionInKillQueue), ref _positionInKillQueue, value); }
        }

        private bool? _killedAtDay;
        public bool? KilledAtDay
        {
            get { return _killedAtDay; }
            set { Set(nameof(KilledAtDay), ref _killedAtDay, value); }
        }

        private bool? _killedAtNight;
        public bool? KilledAtNight
        {
            get { return _killedAtNight; }
            set { Set(nameof(KilledAtNight), ref _killedAtNight, value); }
        }

        private int? _checkedAtNight;
        public int? CheckedAtNight
        {
            get { return _checkedAtNight; }
            set { Set(nameof(CheckedAtNight), ref _checkedAtNight, value); }
        }

        private bool _killButtonVisibilityProperty;
        public bool KillButtonVisibilityProperty
        {
            get { return _killButtonVisibilityProperty; }
            set { Set(nameof(KillButtonVisibilityProperty), ref _killButtonVisibilityProperty, value); }
        }

        private Brush _backgroundColor = new SolidColorBrush();
        public Brush BackgroundColor
        {
            get { return _backgroundColor; }
            set { Set(nameof(BackgroundColor), ref _backgroundColor, value); }
        }

        private ObservableCollection<string> _rolesAviailable;
        public ObservableCollection<string> RolesAviailable
        {
            get { return _rolesAviailable; }
            set { Set(nameof(RolesAviailable), ref _rolesAviailable, value); }
        }

        private ObservableCollection<string> _bestPlayersAvailable;
        public ObservableCollection<string> BestPlayersAvailable
        {
            get { return _bestPlayersAvailable; }
            set { Set(nameof(BestPlayersAvailable), ref _bestPlayersAvailable, value); }
        }

        private bool _isDropDownOpened;
        public bool IsDropDownOpened
        {
            get { return _isDropDownOpened; }
            set { Set(nameof(IsDropDownOpened), ref _isDropDownOpened, value); }
        }

        public PlayerEntry()
        {
            Nick = null;
            Role = null;
            Foul = null;
            Reflection = null;
            Result = null;
            BackgroundColor = Brushes.White;
            KillButtonVisibilityProperty = true;
            PositionInKillQueue = null;
            KilledAtDay = null;
            KilledAtNight = null;
            RolesAviailable = new ObservableCollection<string> { GetLocalized("Red"), GetLocalized("Black"), GetLocalized("Sheriff"), GetLocalized("Don") };
            BestPlayersAvailable = new ObservableCollection<string> { " ", GetLocalized("Best1"), GetLocalized("Best2"), GetLocalized("Best3") };
            CheckedAtNight = null;
            IsDropDownOpened = false;
        }
    }
}
