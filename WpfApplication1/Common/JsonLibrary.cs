using System;
using System.Collections.ObjectModel;
using System.Linq;
using WpfApplication1.Models;
using Newtonsoft.Json;

namespace WpfApplication1.Common
{
    internal class JsonLibrary
    {
        public static string JsonSerialize(ObservableCollection<PlayerEntry> players, int? tableN, int? gameN,
            ObservableCollection<int?> bestWayContainer, ObservableCollection<int?> killedAtNight, bool ugad, ObservableCollection<int?> ugadaykaContainer,
            bool redWin, bool techRed, bool blackWin, bool techBlack, bool sheriffFirstNight, bool threeZv, bool authomate, int? falseCom)
        {
            var convertedPlayers = players.Select(t => new JsonPlayerEntry
            {
                Result = t.Result,
                Foul = t.Foul,
                Role = t.Role,
                Nick = t.Nick,
                Reflection = t.Reflection,
                PositionInKillQueue = t.PositionInKillQueue,
                KilledAtNight = t.KilledAtNight,
                KilledAtDay = t.KilledAtDay,
                CheckedAtNight = t.CheckedAtNight
            });
            var target = new JsonProtocol
            {
                Players = convertedPlayers.ToList(),
                FalseCom = falseCom,
                Ugadayka = ugadaykaContainer.ToList(),
                BestPlayers =
                    players.Where(p => p.Reflection >= 0 && (p.Foul < 4 || p.Foul == null)).Select(t => t.Nick).ToList(),
                BestWay = bestWayContainer.ToList(),
                Date = DateTime.Now,
                FirstNightSheriff = sheriffFirstNight,
                Game = gameN,
                Table = tableN,
                NightKill = killedAtNight.ToList(),
                ThreeZv = threeZv,
                UgadaykaOn = ugad,
                Winner = redWin ? techRed ? "TechRed" : "Red" : techBlack ? "TechBlack" : "Black"
            };
            return JsonConvert.SerializeObject(target);
        }

        public static JsonProtocol JsonDeserialize(string jsonProtocol)
        {
            try
            {
                return JsonConvert.DeserializeObject<JsonProtocol>(jsonProtocol);
            }
            catch (Exception ex)
            {
                CommonMethods.ShowErrorMessage("DESERIALIZE JSON", "Error while deserializing json file: " + ex.Message);
                return null;
            }
        }
    }
}
