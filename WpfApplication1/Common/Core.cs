using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using WpfApplication1.DAL;
using WpfApplication1.Models;
using static WpfApplication1.Common.CommonMethods;

namespace WpfApplication1.Common
{
    public static class Core
    {
        public static ObservableCollection<FiimPlayerEntry> FiimResultCount(ObservableCollection<PlayerEntry> players, bool redWin, bool blackWin, ObservableCollection<int?> bestWay)
        {
            var result = new ObservableCollection<FiimPlayerEntry>(players.Select(t => new FiimPlayerEntry
            {
                Nick = t.Nick,
                Role = t.Role,
                PositionInKillQueue = t.PositionInKillQueue,
                Foul = t.Foul,
                Reflection = t.Reflection
            }));
            foreach (var p in result)
            {
                if (p.Role == GetLocalized("Red") || p.Role == GetLocalized("Sheriff"))
                {
                    p.ScoreMain = redWin ? 2 : 0;
                }
                else if (p.Role == GetLocalized("Black") || p.Role == GetLocalized("Don"))
                {
                    p.ScoreMain = redWin ? 0 : 2;
                }
            }
            var indexes = result.Where(t => t.IsBlackTeam()).Select(result.IndexOf);
            var matches = indexes.Cast<int?>().Count(i => (i == bestWay[0] - 1) || (i == bestWay[1] - 1) || (i == bestWay[2] - 1));
            var firstKilled = result.SingleOrDefault(t => t.PositionInKillQueue == 1 && (t.Foul < 4 || t.Foul == null));
            if (firstKilled != null)
            {
                switch (matches)
                {
                    case 3:
                        firstKilled.ScoreAdditional += 0.5;
                        break;
                    case 2:
                        firstKilled.ScoreAdditional += 0.25;
                        break;
                }
            }
            var firstInWinner = result.SingleOrDefault(t => t.Reflection == 1 && (t.Foul < 4 || t.Foul == null));
            var secondInWinner = result.SingleOrDefault(t => t.Reflection == 2 && (t.Foul < 4 || t.Foul == null));
            var firstInLosers = result.SingleOrDefault(t => t.Reflection == 3 && (t.Foul < 4 || t.Foul == null));
            if (firstInWinner != null) firstInWinner.ScoreAdditional += 1;
            if (secondInWinner != null) secondInWinner.ScoreAdditional += 0.5;
            if (firstInLosers != null) firstInLosers.ScoreAdditional += 0.5;
            foreach (var p in result.Where(p => p.Foul == 4))
            {
                p.ScoreMain = 0;
                p.ScoreAdditional = -1;
            };
            return result;
        }

        public static void ResultCount(ObservableCollection<PlayerEntry> players, bool redWin, bool blackWin)
        {
            if (redWin && blackWin)
            {
                System.Windows.MessageBox.Show(GetLocalized("SelectWinnerHeader"));
                return;
            }
            foreach (var p in players)
            {
                if (p.Role == GetLocalized("Red"))
                {
                    p.Result = redWin ? 3 : 0;
                }
                else if (p.Role == GetLocalized("Black"))
                {
                    p.Result = redWin ? 0 : 3;
                }
                else if (p.Role == GetLocalized("Don"))
                {
                    p.Result = redWin ? -1 : 4;
                }
                else if (p.Role == GetLocalized("Sheriff"))
                {
                    p.Result = redWin ? 4 : -1;
                }
            }
        }

        public static bool BestWayCount(ObservableCollection<PlayerEntry> players, ObservableCollection<int?> bestWay)
        {
            var indexes = players.Where(t => t.IsBlackTeam()).Select(players.IndexOf);
            var matches = indexes.Cast<int?>().Count(i => (i == bestWay[0] - 1) || (i == bestWay[1] - 1) || (i == bestWay[2] - 1));
            var firstKilled = players.SingleOrDefault(t => t.PositionInKillQueue == 1 && (t.Foul < 4 || t.Foul == null));
            if (firstKilled == null)
                return false;
            switch (matches)
            {
                case 3:
                    firstKilled.Result += 1;
                    return true;
                case 2:
                    firstKilled.Result += 0.5;
                    break;
            }
            return false;
        }

        public static void FoulCheck(ObservableCollection<PlayerEntry> players)
        {
            foreach (var p in players.Where(p => p.Foul == 4)) p.Result = -1;
        }

        public static void DonChecks(ObservableCollection<PlayerEntry> players, bool donCheckSuccess, bool blackWin)
        {
            if (!donCheckSuccess || !blackWin)
                return;
            var don = players.SingleOrDefault(t => t.Role == GetLocalized("Don") && (t.Foul < 4 || t.Foul == null));
            if (don == null)
                return;
            don.Result += 0.5;
        }

        public static void ThreeZvCount(ObservableCollection<PlayerEntry> players, bool isThreeZv)
        {
            if (!isThreeZv)
                return;
            var sheriff = players.SingleOrDefault(t => t.Role == GetLocalized("Sheriff") && (t.Foul < 4 || t.Foul == null));
            if (sheriff == null)
                return;
            sheriff.Result++;
        }

        public static void TechnicalCheck(ObservableCollection<PlayerEntry> players, bool techRed, bool techBlack)
        {
            if (techBlack) foreach (var p in players.Where(p => p.IsBlackTeam() && (p.Foul < 4 || p.Foul == null))) p.Result++;
            else if (techRed) foreach (var p in players.Where(p => p.IsRedTeam() && (p.Foul < 4 || p.Foul == null))) p.Result++;
        }

        public static void CheckUgadayka(bool ugadaykaEnabled, ObservableCollection<PlayerEntry> players, ObservableCollection<int?> ugadayka,
            bool redWin, bool blackWin)
        {
            if (redWin && ugadaykaEnabled) foreach (var i in from int i in ugadayka where players[i - 1].IsRedTeam() && (players[i - 1].Foul < 4 || players[i - 1].Foul == null) select i) players[i - 1].Result++;
            else if (blackWin && ugadaykaEnabled) foreach (var i in from int i in ugadayka where players[i - 1].IsBlackTeam() && (players[i - 1].Foul < 4 || players[i - 1].Foul == null) select i) players[i - 1].Result++;
        }

        public static bool CheckFirstKill(ObservableCollection<PlayerEntry> players, bool blackWin)
        {
            var don = players.SingleOrDefault(t => t.Role == GetLocalized("Don") && (t.Foul < 4 || t.Foul == null));
            var sheriff = players.SingleOrDefault(t => t.PositionInKillQueue == 1 && t.Role == GetLocalized("Sheriff"));
            if (blackWin && sheriff != null && don != null)
            {
                don.Result++;
            }
            if (sheriff == null || sheriff.Foul == 4)
                return false;
            sheriff.Result++;
            return true;
        }

        public static void CheckReflect(ObservableCollection<PlayerEntry> players)
        {
            var firstInWinner = players.SingleOrDefault(t => t.Reflection == 1 && (t.Foul < 4 || t.Foul == null));
            var secondInWinner = players.SingleOrDefault(t => t.Reflection == 2 && (t.Foul < 4 || t.Foul == null));
            var firstInLosers = players.SingleOrDefault(t => t.Reflection == 3 && (t.Foul < 4 || t.Foul == null));
            if (firstInWinner != null) firstInWinner.Result += 1;
            if (secondInWinner != null) secondInWinner.Result += 0.5;
            if (firstInLosers != null) firstInLosers.Result += 0.5;
        }

        public static void CheckFalseCom(ObservableCollection<PlayerEntry> players, bool blackWin, int? falseCom)
        {
            if (falseCom == null || falseCom == 0 || !blackWin || players == null || !players.Any())
                return;
            var falseSheriff = players[falseCom.Value - 1];
            falseSheriff.Result++;
        }

        public static GameResultCheck CheckResult(ObservableCollection<PlayerEntry> players)
        {
            var aliveReds = players.Count(x => x.KilledAtNight == null && x.KilledAtDay == null && x.IsRedTeam());
            var aliveBlacks = players.Count(x => x.KilledAtNight == null && x.KilledAtDay == null && x.IsBlackTeam());
            var alivePlayers = players.Count(x => x.KilledAtNight == null && x.KilledAtDay == null);
            var redsKilledAtDay = players.Count(x => x.IsRedTeam() && x.KilledAtDay == true);
            var result = new GameResultCheck
            {
                BlackWin = aliveReds == aliveBlacks,
                TechnicalBlackWin = aliveReds == aliveBlacks && aliveBlacks == 3,
                RedWin = aliveBlacks == 0,
                TechnicalRedWin = aliveBlacks == 0 && redsKilledAtDay == 0,
                Ugadayka = alivePlayers == 3
            };
            return result;
        }

        public static List<string> GetNicksFromDb()
        {
            var nickCollection = new List<string>();
            var context = new SaluteDbContext();
            nickCollection.AddRange(context.AllTimeRatingDbSet.Select(alr => alr.Nick));
            return nickCollection;
        }
    }
}
