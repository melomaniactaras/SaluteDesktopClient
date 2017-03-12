namespace WpfApplication1.Models
{
    public interface IRating
    {
        int ID { get; set; }
        string Nick { get; set; }
        string NickNormalized { get; set; }
        int Games { get; set; }
        double? Score { get; set; }
        double? Rating { get; set; }
        int? BestPlayer { get; set; }
        int? BestChoice { get; set; }
        int? Win { get; set; }
        int? TechRed { get; set; }
        int? TechBlack { get; set; }
        int? Lose { get; set; }
        int? RedUgadayka { get; set; }
        int? BlackUgadayka { get; set; }
        int? WinRow { get; set; }
        int? WinDon { get; set; }
        int? WinSheriff { get; set; }
        int? WinRed { get; set; }
        int? WinBlack { get; set; }
        int? LoseDon { get; set; }
        int? LoseSheriff { get; set; }
        int? LoseRed { get; set; }
        int? LoseBlack { get; set; }
        int? Fouls { get; set; }
        int? WithoutFouls { get; set; }
        int? KilledAtFirstDay { get; set; }
        int? CheckedFirst { get; set; }
        int? CheckedSecond { get; set; }
        int? ThreeZv { get; set; }
        int? Ban { get; set; }
        string Position { get; set; }
        int? FalseCom { get; set; }
        int? FalseComWin { get; set; }
        int? FalseComLose { get; set; }
        bool? IsMember { get; set; }
    }
}
