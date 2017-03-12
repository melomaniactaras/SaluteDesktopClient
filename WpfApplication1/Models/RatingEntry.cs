using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using WpfApplication1.Common;
using static WpfApplication1.Common.CommonMethods;

namespace WpfApplication1.Models
{
    public abstract class RatingEntry : IRating
    {
        protected RatingEntry() { }

        protected RatingEntry(PlayerEntry player, IList<PlayerEntry> players, bool isBestWay, bool redWin,
            bool blackWin, bool techRedWin, bool techBlackWin,
            ICollection<int?> ugadaykaContainer, bool ugadaykaOn, bool threeZv, bool falseCom)
        {
            var tablePlace = players.IndexOf(player) + 1;

            Nick = player.Nick;
            NickNormalized = player.Nick.ToLower();
            Games = 1;
            Score = player.Result;
            Rating = null;
            BestPlayer = player.Reflection.HasValue && player.Reflection >= 0 ? 1 : default(int?);
            BestChoice = isBestWay && player.PositionInKillQueue == 1 ? 1 : default(int?);
            if ((redWin && player.IsRedTeam()) || (blackWin && player.IsBlackTeam()))
            {
                Win = 1;
                WinRow = 1;
                FalseComWin = falseCom ? 1 : default(int?);
            }
            TechRed = techRedWin && player.IsRedTeam() ? 1 : default(int?);
            TechBlack = techBlackWin && player.IsBlackTeam() ? 1 : default(int?);
            if ((redWin && player.IsBlackTeam()) || (blackWin && player.IsRedTeam()))
            {
                Lose = 1;
                FalseComLose = falseCom ? 1 : default(int?);
            }
            RedUgadayka = ugadaykaContainer.Contains(tablePlace) && player.IsRedTeam() && redWin && ugadaykaOn
                ? 1
                : default(int?);
            BlackUgadayka = ugadaykaContainer.Contains(tablePlace) && player.IsBlackTeam() && blackWin && ugadaykaOn
                ? 1
                : default(int?);
            WinDon = blackWin && player.Role == GetLocalized("Don") ? 1 : default(int?);
            WinSheriff = redWin && player.Role == GetLocalized("Sheriff") ? 1 : default(int?);
            WinRed = redWin && player.Role == GetLocalized("Red") ? 1 : default(int?);
            WinBlack = blackWin && player.Role == GetLocalized("Black") ? 1 : default(int?);
            LoseDon = redWin && player.Role == GetLocalized("Don") ? 1 : default(int?);
            LoseSheriff = blackWin && player.Role == GetLocalized("Sheriff") ? 1 : default(int?);
            LoseBlack = redWin && player.Role == GetLocalized("Black") ? 1 : default(int?);
            LoseRed = blackWin && player.Role == GetLocalized("Red") ? 1 : default(int?);
            KilledAtFirstDay = player.PositionInKillQueue == 1 ? 1 : default(int?);
            CheckedFirst = player.CheckedAtNight == 1 ? 1 : default(int?);
            CheckedSecond = player.CheckedAtNight == 2 ? 1 : default(int?);
            ThreeZv = threeZv && player.Role == GetLocalized("Sheriff") ? 1 : default(int?);
            Ban = player.Foul == 4 ? 1 : default(int?);
            FalseCom = falseCom ? 1 : default(int?);
            if (player.Foul != null && player.Foul != 0)
            {
                Fouls = player.Foul;
            }
            else if (player.Foul == null || player.Foul == 0)
            {
                WithoutFouls = 1;
            }
            IsMember = true;
        }

        [Key]
        public int ID { get; set; }
        public string Nick { get; set; }
        public string NickNormalized { get; set; }
        public int Games { get; set; }
        public double? Score { get; set; }
        public double? Rating { get; set; }
        public int? BestPlayer { get; set; }
        public int? BestChoice { get; set; }
        public int? Win { get; set; }
        public int? TechRed { get; set; }
        public int? TechBlack { get; set; }
        public int? Lose { get; set; }
        public int? RedUgadayka { get; set; }
        public int? BlackUgadayka { get; set; }
        public int? WinRow { get; set; }
        public int? WinDon { get; set; }
        public int? WinSheriff { get; set; }
        public int? WinRed { get; set; }
        public int? WinBlack { get; set; }
        public int? LoseDon { get; set; }
        public int? LoseSheriff { get; set; }
        public int? LoseRed { get; set; }
        public int? LoseBlack { get; set; }
        public int? Fouls { get; set; }
        public int? WithoutFouls { get; set; }
        public int? KilledAtFirstDay { get; set; }
        public int? CheckedFirst { get; set; }
        public int? CheckedSecond { get; set; }
        public int? ThreeZv { get; set; }
        public int? Ban { get; set; }
        public string Position { get; set; }
        public int? FalseCom { get; set; }
        public int? FalseComWin { get; set; }
        public int? FalseComLose { get; set; }
        public bool? IsMember { get; set; }
    }
}
