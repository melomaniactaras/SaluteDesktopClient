namespace WpfApplication1.Models
{
    public class WeekRatingFiim : FiimRatingEntry
    {
        public WeekRatingFiim() {}

        public WeekRatingFiim(FiimPlayerEntry player, bool redWin, bool blackWin) : base(player, redWin, blackWin) { }
    }
}
