using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using WpfApplication1.Models;

namespace WpfApplication1.ViewModels
{
    public partial class MainWindowViewModel
    {
        public RelayCommand<string> AddToVote => new RelayCommand<string>(AddToVoteMethod);

        public RelayCommand<string> DeleteFromVote => new RelayCommand<string>(DeleteFromVoteMethod);

        public RelayCommand ClearVoteList => new RelayCommand(ClearVoteListMethod);

        public RelayCommand DuplicateNicks => new RelayCommand(CheckNicksDuplicate);

        public RelayCommand CountResult => new RelayCommand(CountResultMethod);

        public RelayCommand<string> CheckEmptyNick => new RelayCommand<string>(CheckEmptyNickMethod);

        public RelayCommand RefreshAutoComplete => new RelayCommand(RefreshAutoCompleteMethod);

        public RelayCommand RolesCheck => new RelayCommand(ItemsAviailable);

        public RelayCommand<PlayerEntry> RolesClick => new RelayCommand<PlayerEntry>(RolesClickMethod);

        public RelayCommand BestPlayersSet => new RelayCommand(BestPlayersSetHandler);

        public RelayCommand ClearRoles => new RelayCommand(ClearRolesMethod);

        public RelayCommand FillRoles => new RelayCommand(FillRolesMethod);

        public RelayCommand<string> KillButtonClick => new RelayCommand<string>(KillButtonVoid);

        public RelayCommand<object> FoulValueChanged => new RelayCommand<object>(OnFoulValueChanged);

        public RelayCommand<object> OnlyDigitsUgadayka => new RelayCommand<object>(OnlyDigitsUgadaykaMethod);

        public RelayCommand<object> OnlyDigitsBestWay => new RelayCommand<object>(OnlyDigitsBestWayMethod);

        public RelayCommand<KeyEventArgs> BestWayKeyDown => new RelayCommand<KeyEventArgs>(BestWayKeyDownMethod);

        public RelayCommand ClearApplication => new RelayCommand(ClearApplicationMethod);

        public RelayCommand ApplicationClose => new RelayCommand(ApplicationCloseMethod);

        public RelayCommand SaveToFile => new RelayCommand(SaveToFileMethod);

        public RelayCommand LoadFromFile => new RelayCommand(LoadFromFileMethod);

        public RelayCommand SaveToDb => new RelayCommand(SaveToDbMethod);

        public RelayCommand NewWeek => new RelayCommand(NewWeekMethod);
        public RelayCommand BackupDb => new RelayCommand(BackupDbMethod);

        public RelayCommand OpenDb => new RelayCommand(OpenDbMethod);
        
        public RelayCommand ShowGraphs => new RelayCommand(ShowGraphsMethod);

        public RelayCommand ShowSettings => new RelayCommand(ShowSettingsMethod);

        public RelayCommand ShowStatistics => new RelayCommand(ShowStatisticsMethod);

        public RelayCommand CreateInfographics => new RelayCommand(CreateInfographicsMethod); 

        public RelayCommand ShowDonCheck => new RelayCommand(ShowDonCheckMethod);

        public RelayCommand ShowSheriffCheck => new RelayCommand(ShowSheriffCheckMethod);

        public RelayCommand<string> CheckButtonPress => new RelayCommand<string>(CheckButtonPressMethod);

        public RelayCommand CheckOkButton => new RelayCommand(CheckOkButtonMethod);

        public RelayCommand<TextChangedEventArgs> KillQueueChanged => new RelayCommand<TextChangedEventArgs>(KillQueueChangedMethod);
    }
}
