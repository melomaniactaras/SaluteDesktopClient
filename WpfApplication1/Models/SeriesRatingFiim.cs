namespace WpfApplication1.Models
{
    public class SeriesRatingFiim : FiimRatingEntry
    {
        public SeriesRatingFiim() {}

        public SeriesRatingFiim(FiimPlayerEntry player, bool redWin, bool blackWin) : base(player, redWin, blackWin) { }
    }
}
