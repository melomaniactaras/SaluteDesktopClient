using System.ComponentModel.DataAnnotations;
using WpfApplication1.Common;

namespace WpfApplication1.Models
{
    public abstract class FiimRatingEntry : IFiimRating
    {
        protected FiimRatingEntry() { }
        protected FiimRatingEntry(FiimPlayerEntry player, bool redWin, bool blackWin)
        {
            Nick = player.Nick;
            NickNormalized = player.Nick.ToLower();
            Games = 1;
            ScoreMain = player.ScoreMain;
            ScoreAdditional = player.ScoreAdditional;
            Rating = null;
            RedWin = redWin && player.IsRedTeam() ? 1 : 0;
            BlackWin = blackWin && player.IsBlackTeam() ? 1 : 0;
            IsMember = true;
        }

        [Key]
        public int ID { get; set; }
        public string Nick { get; set; }
        public string NickNormalized { get; set; }
        public int Games { get; set; }
        public double? ScoreMain { get; set; }
        public double ScoreAdditional { get; set; }
        public double? Rating { get; set; }
        public int? RedWin { get; set; }
        public int? BlackWin { get; set; }
        public bool? IsMember { get; set; }
    }
}
