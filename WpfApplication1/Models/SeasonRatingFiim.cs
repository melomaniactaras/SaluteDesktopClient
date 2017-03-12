namespace WpfApplication1.Models
{
    public class SeasonRatingFiim : FiimRatingEntry
    {
        public SeasonRatingFiim() { }
        public SeasonRatingFiim(FiimPlayerEntry player, bool redWin, bool blackWin) : base(player, redWin, blackWin) { }
    }
}
