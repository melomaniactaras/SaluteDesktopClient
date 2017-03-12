using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace WpfApplication1.Models
{
    public static class XmlPlaylist
    {
        public static ObservableCollection<SongEntry> GetLastPlaylist()
        {
            var xml = new XmlDocument();
            var output = new ObservableCollection<SongEntry>();
            if (File.Exists($@"{Directory.GetCurrentDirectory()}\Playlist.xml"))
            {
                xml.Load($@"{Directory.GetCurrentDirectory()}\Playlist.xml");
            }
            var root = xml.SelectSingleNode("Root");
            var songs = root?.SelectNodes("//*");
            if (songs == null) return output;
            foreach (XmlNode item in songs)
            {
                if (item.Attributes == null || item.Attributes.Count == 0) continue;
                var song = new SongEntry
                {
                    SongName = item.Attributes["SongName"].Value ?? string.Empty,
                    ArtistName = item.Attributes["ArtistName"].Value ?? string.Empty,
                    Duration = TimeSpan.Parse(item.Attributes["Duration"].Value),
                    Uri = new Uri(item.Attributes["Uri"].Value),
                    IsSelected = false,
                };
                output.Add(song);
            }
            foreach (var item in output)
            {
                item.Index = output.IndexOf(item) + 1;
            }
            CheckIfSongExists(output);
            return output;
        }

        private static void CheckIfSongExists(ICollection<SongEntry> source)
        {
            foreach (var x in source.ToList().Where(x => !File.Exists(x.Uri.LocalPath)))
            {
                source.Remove(x);
            }
        }

        public static void SavePlaylist(ObservableCollection<SongEntry> source)
        {
            var playlist = new XmlDocument();
            var rootelement = playlist.CreateElement("Root");
            foreach (var item in source)
            {
                XmlElement songEntry;
                if (item.ArtistName != string.Empty)
                {
                    songEntry =
                        playlist.CreateElement(Regex.Replace(item.ArtistName, @"[^a-zA-Z\._]", string.Empty) + "a" +
                                               Regex.Replace(item.SongName, @"[^a-zA-Z\._]", string.Empty));
                }
                else if (!string.IsNullOrEmpty(item.SongName))
                {
                    songEntry = playlist.CreateElement(Regex.Replace(item.SongName, @"[^a-zA-Z\._]", string.Empty));
                }
                else songEntry = playlist.CreateElement("noname");
                songEntry.SetAttribute("SongName", item.SongName);
                songEntry.SetAttribute("ArtistName", item.ArtistName);
                songEntry.SetAttribute("Duration", item.Duration.ToString());
                songEntry.SetAttribute("Uri", item.Uri.ToString());
                rootelement.AppendChild(songEntry);
            }
            playlist.AppendChild(rootelement);
            playlist.Save($@"{Directory.GetCurrentDirectory()}\Playlist.xml");
        }
    }
}
