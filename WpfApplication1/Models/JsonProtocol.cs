using System;
using System.Collections.Generic;

namespace WpfApplication1.Models
{
    internal class JsonProtocol
    {
        public DateTime Date { get; set; }
        public int? Game { get; set; }
        public int? Table { get; set; }
        public List<JsonPlayerEntry> Players { get; set; }
        public List<int?> BestWay { get; set; }
        public List<int?> NightKill { get; set; }   
        public bool UgadaykaOn { get; set; }
        public List<int?> Ugadayka { get; set; }
        public List<string> BestPlayers { get; set; }
        public string Winner { get; set; }
        public bool FirstNightSheriff { get; set; }
        public bool ThreeZv { get; set; }
        public int? FalseCom { get; set; }
    }
}
