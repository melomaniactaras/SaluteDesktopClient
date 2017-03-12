using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using WpfApplication1.Models;
using static WpfApplication1.Common.CommonMethods;

namespace WpfApplication1.Common
{
    public static class XmlLibrary
    {
        private static void CreateElementWithInnerText(XmlDocument file, string elementName, string innerText, XmlNode parent)
        {
            var element = file.CreateElement(elementName);
            element.InnerText = innerText;
            parent.AppendChild(element);
        }

        public static XmlDocument AddPlayers(ObservableCollection<PlayerEntry> players,
            string date, string time, int? tableN, int? gameN, ObservableCollection<int?> bestWayContainer, ObservableCollection<int?> killedAtNight,
            bool ugad, ObservableCollection<int?> ugadaykaContainer, bool redWin, bool techRed, bool blackWin, bool techBlack, bool sheriffFirstNight,
            bool threeZv, bool authomate, int? falseCom)
        {
            var saveFile = new XmlDocument();
            var newProtocol = saveFile.CreateElement("NewProtocol");
            newProtocol.SetAttribute("Date", date);
            newProtocol.SetAttribute("Time", time);
            newProtocol.SetAttribute("Table", tableN.ToString());
            newProtocol.SetAttribute("Game", gameN.ToString());
            for (var i = 0; i < 10; i++)
            {
                var player = saveFile.CreateElement($"Player{i + 1}");
                CreateElementWithInnerText(saveFile, "Nick", players[i].Nick, player);
                CreateElementWithInnerText(saveFile, "Role", players[i].Role.SetUnifiedRole(), player);
                CreateElementWithInnerText(saveFile, "Fouls", players[i].Foul.ToString(), player);
                CreateElementWithInnerText(saveFile, "Reflect", players[i].Reflection.ToString(), player);
                CreateElementWithInnerText(saveFile, "Result", players[i].Result.ToString(), player);
                CreateElementWithInnerText(saveFile, "Background", players[i].BackgroundColor.ToString(), player);
                CreateElementWithInnerText(saveFile, "KillQueuePosition", players[i].PositionInKillQueue.ToString(), player);
                newProtocol.AppendChild(player);
            }
            var bestChoice = saveFile.CreateElement("BestChoice");
            CreateElementWithInnerText(saveFile, "ChoiceOne", bestWayContainer[0].ToString(), bestChoice);
            CreateElementWithInnerText(saveFile, "ChoiceTwo", bestWayContainer[1].ToString(), bestChoice);
            CreateElementWithInnerText(saveFile, "ChoiceThree", bestWayContainer[2].ToString(), bestChoice);
            newProtocol.AppendChild(bestChoice);
            var nightKill = saveFile.CreateElement("Night_Kill");
            CreateElementWithInnerText(saveFile, "FirstNight", killedAtNight[0].ToString(), nightKill);
            CreateElementWithInnerText(saveFile, "SecondNight", killedAtNight[1].ToString(), nightKill);
            CreateElementWithInnerText(saveFile, "ThirdNight", killedAtNight[2].ToString(), nightKill);
            newProtocol.AppendChild(nightKill);
            var ugadayka = saveFile.CreateElement("Ugadayka");
            ugadayka.SetAttribute("Ugadayka_On", ugad.ToString());
            CreateElementWithInnerText(saveFile, "UgadaykaOne", ugadaykaContainer[0].ToString(), ugadayka);
            CreateElementWithInnerText(saveFile, "UgadaykaTwo", ugadaykaContainer[1].ToString(), ugadayka);
            CreateElementWithInnerText(saveFile, "UgadaykaThree", ugadaykaContainer[2].ToString(), ugadayka);
            newProtocol.AppendChild(ugadayka);
            var bestplayers = players.Where(p => p.Reflection >= 0 && (p.Foul < 4 || p.Foul == null)).ToList();
            var bestPlayersNode = saveFile.CreateElement("BestPlayers");
            bestPlayersNode.SetAttribute("Count", bestplayers.Count.ToString());
            switch (bestplayers.Count)
            {
                case 1:
                    CreateElementWithInnerText(saveFile, "BestPlayerOne", bestplayers[0].Nick, bestPlayersNode);
                    break;
                case 2:
                {
                    CreateElementWithInnerText(saveFile, "BestPlayerOne", bestplayers[0].Nick, bestPlayersNode);
                    CreateElementWithInnerText(saveFile, "BestPlayerTwo", bestplayers[1].Nick, bestPlayersNode);
                }
                    break;
                case 3:
                {
                    CreateElementWithInnerText(saveFile, "BestPlayerOne", bestplayers[0].Nick, bestPlayersNode);
                    CreateElementWithInnerText(saveFile, "BestPlayerTwo", bestplayers[1].Nick, bestPlayersNode);
                    CreateElementWithInnerText(saveFile, "BestPlayerThree", bestplayers[2].Nick, bestPlayersNode);

                }
                    break;
            }
            newProtocol.AppendChild(bestPlayersNode);
            CreateElementWithInnerText(saveFile, "WinningTeam", redWin ? techRed ? "RedTech" : "Red" : techBlack ? "BlackTech" : "Black", newProtocol);
            CreateElementWithInnerText(saveFile, "FirstNightSheriff", sheriffFirstNight.ToString(), newProtocol);
            if (threeZv)
            {
                CreateElementWithInnerText(saveFile, "ThreeZv", "True", newProtocol);
            }
            if (falseCom != null)
            {
                CreateElementWithInnerText(saveFile, "FalseCom", falseCom.ToString(), newProtocol);
            }
            saveFile.AppendChild(newProtocol);
            if (authomate) return saveFile;
            var mainwindow = Application.Current.MainWindow as MetroWindow;
            var setting = new MetroDialogSettings
            {
                ColorScheme = MetroDialogColorScheme.Accented,
                AnimateShow = true,
                AnimateHide = true
            };
            mainwindow.ShowMessageAsync("SUCCESS", GetLocalized("FileSaved"), MessageDialogStyle.Affirmative, setting);
            return saveFile;
        }

        public static Dictionary<string, string> XmlRead(string filePath, ObservableCollection<PlayerEntry> players,
            ObservableCollection<int?> bestChoiceContainer, ObservableCollection<int?> nightKill,
            ObservableCollection<int?> ugadaykaContainer)
        {
            var load = new XmlDocument();
            var outList = new Dictionary<string, string>();
            load.Load(filePath);
            var elemList = load.SelectSingleNode("NewProtocol");
            if (elemList?.Attributes != null)
            {
                outList = new Dictionary<string, string>
                {
                    {"Date", elemList.Attributes["Date"].Value},
                    {"Time", elemList.Attributes["Time"].Value},
                    {"Table", elemList.Attributes["Table"].Value},
                    {"Game", elemList.Attributes["Game"].Value}
                };
            }
            for (var i = 0; i < 10; i++)
            {
                var player = load.SelectSingleNode($"/NewProtocol/Player{i + 1}");
                if (player == null) continue;
                players[i].Nick = player.SelectSingleNode("Nick")?.InnerText;
                players[i].Role = player.SelectSingleNode("Role")?.InnerText.GetUnifiedRole();
                int tempFoul;
                players[i].Foul = int.TryParse(player.SelectSingleNode("Fouls")?.InnerText, out tempFoul) ? tempFoul : (int?)null;
                int tempRef;
                players[i].Reflection = int.TryParse(player.SelectSingleNode("Reflect")?.InnerText, out tempRef) ? tempRef : (int?)null;
                double tempResult;
                players[i].Result = double.TryParse(player.SelectSingleNode("Result")?.InnerText, out tempResult) ? tempResult : (double?)null;
                var backgroundNode = player.SelectSingleNode("Background");
                if (backgroundNode != null)
                {
                    var convertFromString = ColorConverter.ConvertFromString(backgroundNode.InnerText);
                    if (convertFromString != null)
                    {
                        var color = (Color)convertFromString;
                        players[i].BackgroundColor = new SolidColorBrush(color);
                    }
                }
                int killQueue;
                players[i].PositionInKillQueue =
                    int.TryParse(player.SelectSingleNode("KillQueuePosition")?.InnerText, out killQueue) ? killQueue : (int?)null;
            }
            int tryParseVal;
            bestChoiceContainer[0] = int.TryParse(load.SelectSingleNode("/NewProtocol/BestChoice/ChoiceOne")?.InnerText, out tryParseVal) ? tryParseVal : (int?)null;
            bestChoiceContainer[1] = int.TryParse(load.SelectSingleNode("/NewProtocol/BestChoice/ChoiceTwo")?.InnerText, out tryParseVal) ? tryParseVal : (int?)null;
            bestChoiceContainer[2] = int.TryParse(load.SelectSingleNode("/NewProtocol/BestChoice/ChoiceThree")?.InnerText, out tryParseVal) ? tryParseVal : (int?)null;
            nightKill[0] = int.TryParse(load.SelectSingleNode("/NewProtocol/Night_Kill/FirstNight")?.InnerText, out tryParseVal) ? tryParseVal : (int?)null;
            nightKill[1] = int.TryParse(load.SelectSingleNode("/NewProtocol/Night_Kill/SecondNight")?.InnerText, out tryParseVal) ? tryParseVal : (int?)null;
            nightKill[2] = int.TryParse(load.SelectSingleNode("/NewProtocol/Night_Kill/ThirdNight")?.InnerText, out tryParseVal) ? tryParseVal : (int?)null;
            var ugadaykaOn = load.SelectSingleNode("/NewProtocol/Ugadayka");
            if (ugadaykaOn?.Attributes != null) outList.Add("UgadaykaOn", ugadaykaOn.Attributes["Ugadayka_On"].Value);
            var ugadaykaOneNode = load.SelectSingleNode("/NewProtocol/Ugadayka/UgadaykaOne");
            ugadaykaContainer[0] = ugadaykaOneNode != null && int.TryParse(ugadaykaOneNode.InnerText, out tryParseVal) ? tryParseVal : (int?)null;
            ugadaykaContainer[1] = ugadaykaOneNode != null && int.TryParse(load.SelectSingleNode("/NewProtocol/Ugadayka/UgadaykaTwo")?.InnerText, out tryParseVal) ? tryParseVal : (int?)null;
            ugadaykaContainer[2] = ugadaykaOneNode != null && int.TryParse(load.SelectSingleNode("/NewProtocol/Ugadayka/UgadaykaThree")?.InnerText, out tryParseVal) ? tryParseVal : (int?)null;
            var result = $"{GetLocalized("BestPlayer")}: " +
                load.SelectSingleNode("/NewProtocol/BestPlayers/BestPlayerOne")?.InnerText + " " +
                load.SelectSingleNode("/NewProtocol/BestPlayers/BestPlayerTwo")?.InnerText + " " +
                load.SelectSingleNode("/NewProtocol/BestPlayers/BestPlayerThree")?.InnerText;
            outList.Add("BestPlayer", result);
            var winningTeamNode = load.SelectSingleNode("/NewProtocol/WinningTeam");
            if (winningTeamNode != null) outList.Add("GameResult", winningTeamNode.InnerText);
            var firstNightSheriffNode = load.SelectSingleNode("/NewProtocol/FirstNightSheriff");
            if (firstNightSheriffNode != null) outList.Add("FirstNightSheriff", firstNightSheriffNode.InnerText);
            var threeZvNode = load.SelectSingleNode("/NewProtocol/ThreeZv");
            if (threeZvNode != null) outList.Add("ThreeZv", threeZvNode.InnerText);
            var falseComNode = load.SelectSingleNode("/NewProtocol/FalseCom");
            if (falseComNode != null) outList.Add("FalseCom", falseComNode.InnerText);
            return outList;
        }
    }
}
