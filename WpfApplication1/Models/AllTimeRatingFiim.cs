namespace WpfApplication1.Models
{
    public class AllTimeRatingFiim : FiimRatingEntry
    {
        public AllTimeRatingFiim() { }

        public AllTimeRatingFiim(FiimPlayerEntry player, bool redWin, bool blackWin) : base(player, redWin, blackWin) { }
    }
}
