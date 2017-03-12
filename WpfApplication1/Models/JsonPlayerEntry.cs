namespace WpfApplication1.Models
{
    internal class JsonPlayerEntry
    {
        public string Nick { get; set; }
        public string Role { get; set; }
        public int? Foul { get; set; }
        public int? Reflection { get; set; }
        public double? Result { get; set; }
        public int? PositionInKillQueue { get; set; }
        public bool? KilledAtDay { get; set; }
        public bool? KilledAtNight { get; set; }
        public int? CheckedAtNight { get; set; }

    }
}
