using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using WpfApplication1.Common;
using WpfApplication1.Models;
using static WpfApplication1.Common.CommonMethods;

namespace WpfApplication1.DAL
{
    public static class SaluteRepository
    {
        public static void GeneralFiimDbUpdate(FiimPlayerEntry player, IFiimRating currentPlayer, bool redWin, bool blackWin)
        {
            currentPlayer.Games += 1;
            currentPlayer.ScoreMain += player.ScoreMain;
            currentPlayer.ScoreAdditional += player.ScoreAdditional;
            currentPlayer.Rating = (currentPlayer.ScoreMain + currentPlayer.ScoreAdditional)/currentPlayer.Games*100;
            if (player.IsRedTeam() && redWin)
            {
                currentPlayer.RedWin += 1;
            }
            if (player.IsBlackTeam() && blackWin)
            {
                currentPlayer.BlackWin += 1;
            }
        }

        public static void GeneralDbUpdate(PlayerEntry player, IRating currentPlayer,
            ObservableCollection<PlayerEntry> players, bool isBestWay, bool redWin,
            bool blackWin, bool techRedWin, bool techBlackWin,
            ObservableCollection<int?> ugadaykaContainer, bool ugadaykaOn, bool threeZv, int prohBal,
            bool writeAcchievements, bool falseCom)
        {
            var path = $@"{Directory.GetCurrentDirectory()}\Rating Acchievements.txt";
            currentPlayer.Games += 1;
            currentPlayer.Score += player.Result;
            if (player.Reflection != null && player.Reflection > 0)
            {
                currentPlayer.BestPlayer = currentPlayer.BestPlayer.Increment();
            }
            if (isBestWay && player.PositionInKillQueue == 1)
            {
                currentPlayer.BestChoice = currentPlayer.BestChoice.Increment();
            }
            if (falseCom)
            {
                currentPlayer.FalseCom = currentPlayer.FalseCom.Increment();
            }
            if (redWin && player.IsRedTeam() || blackWin && player.IsBlackTeam())
            {
                currentPlayer.Win = currentPlayer.Win.Increment();
                currentPlayer.WinRow = currentPlayer.WinRow.Increment();
                if (falseCom)
                {
                    currentPlayer.FalseComWin = currentPlayer.FalseComWin.Increment();
                }
            }
            if (techRedWin && player.IsRedTeam())
            {
                currentPlayer.TechRed = currentPlayer.TechRed.Increment();
            }
            if (techBlackWin && player.IsBlackTeam())
            {
                currentPlayer.TechBlack = currentPlayer.TechBlack.Increment();
            }
            if (redWin && player.IsBlackTeam() || blackWin && player.IsRedTeam())
            {
                currentPlayer.Lose = currentPlayer.Lose.Increment();
                currentPlayer.WinRow = 0;
                if (falseCom)
                {
                    currentPlayer.FalseComLose = currentPlayer.FalseComLose.Increment();
                }
            }
            if (ugadaykaContainer.Contains(players.IndexOf(player) + 1) && player.IsRedTeam() && redWin && ugadaykaOn)
            {
                currentPlayer.RedUgadayka = currentPlayer.RedUgadayka.Increment();
            }
            if (ugadaykaContainer.Contains(players.IndexOf(player) + 1) && player.IsBlackTeam() && blackWin && ugadaykaOn)
            {
                currentPlayer.BlackUgadayka = currentPlayer.BlackUgadayka.Increment();
            }
            if (blackWin && player.Role == GetLocalized("Don"))
            {
                currentPlayer.WinDon = currentPlayer.WinDon.Increment();
            }
            if (redWin && player.Role == GetLocalized("Sheriff"))
            {
                currentPlayer.WinSheriff = currentPlayer.WinSheriff.Increment();
            }
            if (redWin && player.Role == GetLocalized("Red"))
            {
                currentPlayer.WinRed = currentPlayer.WinRed.Increment();
            }
            if (blackWin && player.Role == GetLocalized("Black"))
            {
                currentPlayer.WinBlack = currentPlayer.WinBlack.Increment();
            }
            if (redWin && player.Role == GetLocalized("Don"))
            {
                currentPlayer.LoseDon = currentPlayer.LoseDon.Increment();
            }
            if (blackWin && player.Role == GetLocalized("Sheriff"))
            {
                currentPlayer.LoseSheriff = currentPlayer.LoseSheriff.Increment();
            }
            if (redWin && player.Role == GetLocalized("Black"))
            {
                currentPlayer.LoseBlack = currentPlayer.LoseBlack.Increment();
            }
            if (blackWin && player.Role == GetLocalized("Red"))
            {
                currentPlayer.LoseRed = currentPlayer.LoseRed.Increment();
            }
            if (player.Foul != null && player.Foul != 0)
            {
                currentPlayer.Fouls = currentPlayer.Fouls == null ? player.Foul : currentPlayer.Fouls += player.Foul;
                currentPlayer.WithoutFouls = 0;
            }
            else
            {
                currentPlayer.WithoutFouls = currentPlayer.WithoutFouls.Increment();
            }
            if (player.PositionInKillQueue == 1)
            {
                currentPlayer.KilledAtFirstDay = currentPlayer.KilledAtFirstDay.Increment();
            }
            if (player.CheckedAtNight == 1)
            {
                currentPlayer.CheckedFirst = currentPlayer.CheckedFirst.Increment();
            }
            if (player.CheckedAtNight == 2)
            {
                currentPlayer.CheckedSecond = currentPlayer.CheckedSecond.Increment();
            }
            if (threeZv && player.Role == GetLocalized("Sheriff"))
            {
                currentPlayer.ThreeZv = currentPlayer.ThreeZv.Increment();
            }
            if (player.Foul == 4)
            {
                currentPlayer.Ban = currentPlayer.Ban.Increment();
            }
            if (currentPlayer.WinRow == 5)
            {
                currentPlayer.Score += 1;
                if (writeAcchievements)
                {
                    using (var sw = new StreamWriter(path, true))
                    {
                        sw.WriteLine($"{DateTime.Now.ToShortDateString()}. {currentPlayer.Nick} - Combo Win");
                        sw.Close();
                    }
                }
            }
            if (currentPlayer.WinRow == 10)
            {
                if (currentPlayer.Score != null) currentPlayer.Score += 3;
                if (writeAcchievements)
                {
                    using (var sw = new StreamWriter(path, true))
                    {
                        sw.WriteLine($"{DateTime.Now.ToShortDateString()}. {currentPlayer.Nick} - Epic Win");
                        sw.Close();
                    }
                }
            }
            if (currentPlayer.WinRow > 10)
            {
                if (currentPlayer.Score != null) currentPlayer.Score += (double)currentPlayer.WinRow / 20;
                if (writeAcchievements)
                {
                    using (var sw = new StreamWriter(path, true))
                    {
                        sw.WriteLine($"{DateTime.Now.ToShortDateString()}. {currentPlayer.Nick} - Win series {currentPlayer.WinRow}");
                        sw.Close();
                    }
                }
            }
            if (currentPlayer.WithoutFouls == 10 && writeAcchievements)
            {
                using (var sw = new StreamWriter(path, true))
                {
                    sw.WriteLine($"{DateTime.Now.ToShortDateString()}. {currentPlayer.Nick} - Epic Without Fouls");
                    sw.Close();
                }
            }
            if (currentPlayer.Games >= prohBal)
            {
                currentPlayer.Rating = currentPlayer.Score / currentPlayer.Games * 100;
                if (currentPlayer.Rating != null)
                    currentPlayer.Rating = Math.Round((double)currentPlayer.Rating, 3);
            }
            else currentPlayer.Rating = null;
        }

        public static void FiimDbUpdate(ObservableCollection<FiimPlayerEntry> players, bool redWin, bool blackWin)
        {
            using (var context = new SaluteDbContext())
            {
                foreach (var player in players)
                {
                    var currentNick = player.Nick;
                    IFiimRating fiimInSeason =
                        context.SeasonRatingFiimDbSet.FirstOrDefault(t => t.Nick == currentNick);
                    IFiimRating fiimInSeries =
                        context.SeriesRatingFiimDbSet.FirstOrDefault(t => t.Nick == currentNick);
                    IFiimRating fiimInWeek = context.WeekRatingFiimDbSet.FirstOrDefault(t => t.Nick == currentNick);
                    IFiimRating fiimInAl = context.AllTimeRatingFiimDbSet.FirstOrDefault(t => t.Nick == currentNick);
                    if (fiimInSeason != null)
                    {
                        GeneralFiimDbUpdate(player, fiimInSeason, redWin, blackWin);
                    }
                    else
                    {
                        var newPlayer = new SeasonRatingFiim(player, redWin, blackWin);
                        context.SeasonRatingFiimDbSet.Add(newPlayer);
                    }
                    if (fiimInSeries != null)
                    {
                        GeneralFiimDbUpdate(player, fiimInSeries, redWin, blackWin);
                    }
                    else
                    {
                        var newPlayer = new SeriesRatingFiim(player, redWin, blackWin);
                        context.SeriesRatingFiimDbSet.Add(newPlayer);
                    }
                    if (fiimInWeek != null)
                    {
                        GeneralFiimDbUpdate(player, fiimInWeek, redWin, blackWin);
                    }
                    else
                    {
                        var newPlayer = new WeekRatingFiim(player, redWin, blackWin);
                        context.WeekRatingFiimDbSet.Add(newPlayer);
                    }
                    if (fiimInAl != null)
                    {
                        GeneralFiimDbUpdate(player, fiimInAl, redWin, blackWin);
                    }
                    else
                    {
                        var newPlayer = new AllTimeRatingFiim(player, redWin, blackWin);
                        context.AllTimeRatingFiimDbSet.Add(newPlayer);
                    }
                }
                context.SaveChanges();
            }
        }

        public static void DbUpdate(ObservableCollection<PlayerEntry> players, bool isBestWay,
            bool redWin, bool blackWin, bool techRedWin, bool techBlackWin,
            ObservableCollection<int?> ugadaykaContainer, bool ugadaykaOn, bool threeZv, int prohBal, int? falseCom)
        {
            var gap = GetGap();
            using (var context = new SaluteDbContext())
            {
                foreach (var player in players)
                {
                    var fc = falseCom != null && falseCom == players.IndexOf(player) + 1;
                    var currentNick = player.Nick;
                    IRating inSeason = context.SeasonRatingDbSet.FirstOrDefault(t => t.Nick == currentNick);
                    IRating inSeries = context.SeriesRatingDbSet.FirstOrDefault(t => t.Nick == currentNick);
                    IRating inWeek = context.WeekRatingDbSet.FirstOrDefault(t => t.Nick == currentNick);
                    IRating inAll = context.AllTimeRatingDbSet.FirstOrDefault(t => t.Nick == currentNick);
                    if (inSeason != null)
                    {
                        GeneralDbUpdate(player, inSeason, players, isBestWay,
                            redWin, blackWin, techRedWin, techBlackWin, ugadaykaContainer, ugadaykaOn, threeZv, gap.SeasonMinGames,
                            true, fc);
                    }
                    else
                    {
                        var newPlayer = new SeasonRating(player, players, isBestWay,
                            redWin, blackWin, techRedWin, techBlackWin, ugadaykaContainer, ugadaykaOn, threeZv, fc);
                        context.SeasonRatingDbSet.Add(newPlayer);
                    }
                    if (inSeries != null)
                    {
                        GeneralDbUpdate(player, inSeries, players, isBestWay,
                            redWin, blackWin, techRedWin, techBlackWin, ugadaykaContainer, ugadaykaOn, threeZv, gap.SeriesMinGames,
                            false, fc);
                    }
                    else
                    {
                        var newPlayer = new SeriesRating(player, players, isBestWay,
                            redWin, blackWin, techRedWin, techBlackWin, ugadaykaContainer, ugadaykaOn, threeZv, fc);
                        context.SeriesRatingDbSet.Add(newPlayer);
                    }
                    if (inWeek != null)
                    {
                        GeneralDbUpdate(player, inWeek, players, isBestWay,
                            redWin, blackWin, techRedWin, techBlackWin, ugadaykaContainer, ugadaykaOn, threeZv, 1,
                            false, fc);
                    }
                    else
                    {
                        var newPlayer = new WeekRating(player, players, isBestWay,
                            redWin, blackWin, techRedWin, techBlackWin, ugadaykaContainer, ugadaykaOn, threeZv, fc);
                        context.WeekRatingDbSet.Add(newPlayer);
                    }
                    if (inAll != null)
                    {
                        GeneralDbUpdate(player, inAll, players, isBestWay,
                            redWin, blackWin, techRedWin, techBlackWin, ugadaykaContainer, ugadaykaOn, threeZv, gap.AllTimeMinGames,
                            false, fc);
                    }
                    else
                    {
                        var newPlayer = new AllTimeRating(player, players, isBestWay,
                            redWin, blackWin, techRedWin, techBlackWin, ugadaykaContainer, ugadaykaOn, threeZv, fc);
                        context.AllTimeRatingDbSet.Add(newPlayer);
                    }
                }
                context.SaveChanges();
            }
        }

        public static void ApplyRatio()
        {
            using (var context = new SaluteDbContext())
            {
                var seasonRedSum = context.SeasonRatingFiimDbSet.Sum(t => t.RedWin) / 7;
                var seasonBlackSum = context.SeasonRatingFiimDbSet.Sum(t => t.BlackWin) / 3;
                var seasonRatio = CountRatio(seasonRedSum ?? 0, seasonBlackSum ?? 0);
                foreach (var player in context.SeasonRatingFiimDbSet)
                {
                    var pureRedResult = (player.RedWin ?? 0) * 2;
                    var pureBlackResult = (player.BlackWin ?? 0) * 2;
                    var additionalResult = player.ScoreAdditional;
                    if (seasonRedSum > seasonBlackSum)
                    {
                        player.Rating = Math.Round((pureRedResult + additionalResult + pureBlackResult * seasonRatio) / player.Games, 2) * 100;
                    }
                    else if (seasonBlackSum > seasonRedSum)
                    {
                        player.Rating = Math.Round((pureBlackResult + additionalResult + pureRedResult * seasonRatio) / player.Games, 2) * 100;
                    }
                    else
                    {
                        player.Rating = Math.Round((pureBlackResult + additionalResult + pureRedResult) / player.Games, 2) * 100;
                    }
                }
                var seriesRedSum = context.SeriesRatingFiimDbSet.Sum(t => t.RedWin) / 7;
                var seriesBlackSum = context.SeriesRatingFiimDbSet.Sum(t => t.BlackWin) / 3;
                var seriesRatio = CountRatio(seriesRedSum ?? 0, seriesBlackSum ?? 0);
                foreach (var player in context.SeriesRatingFiimDbSet)
                {
                    var pureRedResult = (player.RedWin ?? 0) * 2;
                    var pureBlackResult = (player.BlackWin ?? 0) * 2;
                    var additionalResult = player.ScoreAdditional;
                    if (seriesRedSum > seriesBlackSum)
                    {
                        player.Rating = Math.Round((pureRedResult + additionalResult + pureBlackResult * seriesRatio) / player.Games, 2) * 100;
                    }
                    else if (seriesBlackSum > seriesRedSum)
                    {
                        player.Rating = Math.Round((pureBlackResult + additionalResult + pureRedResult * seriesRatio) / player.Games, 2) * 100;
                    }
                    else
                    {
                        player.Rating = Math.Round((pureBlackResult + additionalResult + pureRedResult) / player.Games, 2) * 100;
                    }
                }
                var weekRedSum = context.WeekRatingFiimDbSet.Sum(t => t.RedWin) / 7;
                var weekBlackSum = context.WeekRatingFiimDbSet.Sum(t => t.BlackWin) / 3;
                var weekRatio = CountRatio(weekRedSum ?? 0, weekBlackSum ?? 0);
                foreach (var player in context.WeekRatingFiimDbSet)
                {
                    var pureRedResult = (player.RedWin ?? 0) * 2;
                    var pureBlackResult = (player.BlackWin ?? 0) * 2;
                    var additionalResult = player.ScoreAdditional;
                    if (weekRedSum > weekBlackSum)
                    {
                        player.Rating = Math.Round((pureRedResult + additionalResult + pureBlackResult * weekRatio) / player.Games, 2) * 100;
                    }
                    else if (weekBlackSum > weekRedSum)
                    {
                        player.Rating = Math.Round((pureBlackResult + additionalResult + pureRedResult * weekRatio) / player.Games, 2) * 100;
                    }
                    else
                    {
                        player.Rating = Math.Round((pureBlackResult + additionalResult + pureRedResult) / player.Games, 2) * 100;
                    }
                }
                var allRedSum = context.AllTimeRatingFiimDbSet.Sum(t => t.RedWin) / 7;
                var allBlackSum = context.AllTimeRatingFiimDbSet.Sum(t => t.BlackWin) / 3;
                var allRatio = CountRatio(allRedSum ?? 0, allBlackSum ?? 0);
                foreach (var player in context.AllTimeRatingFiimDbSet)
                {
                    var pureRedResult = (player.RedWin ?? 0) * 2;
                    var pureBlackResult = (player.BlackWin ?? 0) * 2;
                    var additionalResult = player.ScoreAdditional;
                    if (allRedSum > allBlackSum)
                    {
                        player.Rating = Math.Round((pureRedResult + additionalResult + pureBlackResult * allRatio) / player.Games, 2) * 100;
                    }
                    else if (allBlackSum > allRedSum)
                    {
                        player.Rating = Math.Round((pureBlackResult + additionalResult + pureRedResult * allRatio) / player.Games, 2) * 100;
                    }
                    else
                    {
                        player.Rating = Math.Round((pureBlackResult + additionalResult + pureRedResult) / player.Games, 2) * 100;
                    }
                }
                context.SaveChanges();
            }
        }

        private static double CountRatio(int redWins, int blackWins)
        {
            var halfTotalGames = (double)(redWins + blackWins) / 2;
            double ratio = 1;
            if (redWins > blackWins && blackWins != 0 && redWins != 0)
            {
                ratio = 1 + (halfTotalGames - blackWins) / halfTotalGames;
            }
            else if (blackWins > redWins && blackWins != 0 && redWins != 0)
            {
                ratio = 1 + (halfTotalGames - redWins) / halfTotalGames;
            }
            return ratio;
        }

        public static void SetPosition(ObservableCollection<PlayerEntry> players)
        {
            using (var context = new SaluteDbContext())
            {
                var inSeasonSorted = context.SeasonRatingDbSet.Where(x => x.Rating != 0 && x.Rating != null).OrderByDescending(x => x.Rating).ToList();
                var inSeriesSorted = context.SeriesRatingDbSet.Where(x => x.Rating != 0 && x.Rating != null).OrderByDescending(x => x.Rating).ToList();
                var inAllTimeSorted =
                    context.AllTimeRatingDbSet.Where(x => x.Rating != 0 && x.Rating != null)
                        .OrderByDescending(x => x.Rating)
                        .ToList();
                foreach (var sr in context.SeasonRatingDbSet)
                {
                    if (!string.IsNullOrEmpty(sr.Position) && sr.Position != "0")
                    {
                        sr.Position = sr.Position + "," + (inSeasonSorted.FindIndex(x => x.Nick == sr.Nick) + 1);
                    }
                    else sr.Position = (inSeasonSorted.FindIndex(x => x.Nick == sr.Nick) + 1).ToString();
                }
                foreach (var sr in context.SeriesRatingDbSet)
                {
                    if (!string.IsNullOrEmpty(sr.Position) && sr.Position != "0")
                    {
                        sr.Position = sr.Position + "," + (inSeriesSorted.FindIndex(x => x.Nick == sr.Nick) + 1);
                    }
                    else sr.Position = (inSeriesSorted.FindIndex(x => x.Nick == sr.Nick) + 1).ToString();
                }
                foreach (var alr in context.AllTimeRatingDbSet)
                {
                    if (!string.IsNullOrEmpty(alr.Position) && alr.Position != "0")
                    {
                        alr.Position = alr.Position + "," + (inAllTimeSorted.FindIndex(x => x.Nick == alr.Nick) + 1);
                    }
                    else alr.Position = (inAllTimeSorted.FindIndex(x => x.Nick == alr.Nick) + 1).ToString();
                }
                context.SaveChanges();
            }
        }

        public static List<int> GetPosition(string positions)
        {
            var stringsDivided = positions.Split(',').ToList();
            var returnValue = stringsDivided.Select(s => Convert.ToInt32(s)).ToList();
            return returnValue;
        }
    }
}
