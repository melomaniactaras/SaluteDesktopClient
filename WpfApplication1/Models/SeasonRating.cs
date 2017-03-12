using System.Collections.Generic;

namespace WpfApplication1.Models
{
    public class SeasonRating : RatingEntry
    {
        public SeasonRating() { }

        public SeasonRating(PlayerEntry player, IList<PlayerEntry> players, bool isBestWay, bool redWin, bool blackWin, bool techRedWin, bool techBlackWin,
            ICollection<int?> ugadaykaContainer, bool ugadaykaOn, bool threeZv, bool falseCom)
            : base(player, players, isBestWay, redWin, blackWin, techRedWin, techBlackWin, ugadaykaContainer, ugadaykaOn, threeZv, falseCom) { }
    }
}
