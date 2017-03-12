namespace WpfApplication1.Models
{
    public interface IFiimRating
    {
        int ID { get; set; }
        string Nick { get; set; }
        string NickNormalized { get; set; }
        int Games { get; set; }
        double? ScoreMain { get; set; }
        double ScoreAdditional { get; set; }
        double? Rating { get; set; }
        int? RedWin { get; set; }
        int? BlackWin { get; set; }
        bool? IsMember { get; set; }
    }
}
