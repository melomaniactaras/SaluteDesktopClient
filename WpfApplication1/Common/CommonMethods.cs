using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using WpfApplication1.Models;

namespace WpfApplication1.Common
{
    public static class CommonMethods
    {
        static CommonMethods()
        {
            Culture = GetCulture();
        }

        private static CultureInfo Culture { get; }

        public static string GetLocalized(string value)
        {
            return Properties.Resources.ResourceManager.GetString(value, Culture);
        }

        public static async Task<MessageDialogResult> ShowMetroDialog(string title, string message,
            MessageDialogStyle style)
        {
            var window = Application.Current.Windows.OfType<MetroWindow>().SingleOrDefault(x => x.IsActive);
            window?.Focus();
            var settings = new MetroDialogSettings
            {
                ColorScheme = MetroDialogColorScheme.Accented,
                AnimateShow = true,
                AnimateHide = true
            };
            return await window.ShowMessageAsync(title, message, style, settings);
        }

        public static Gap GetGap()
        {
            try
            {
                int seasonGap;
                int seriesGap;
                int allTimeGap;
                var xml = new XmlDocument();
                xml.Load($@"{Directory.GetCurrentDirectory()}\Settings.xml");
                var seasonBalNode = xml.SelectSingleNode("/Settings/SeasonBal");
                var seriesBalNode = xml.SelectSingleNode("/Settings/SeriesBal");
                var allTimeBalNode = xml.SelectSingleNode("/Settings/AllTimeBal");
                int.TryParse(seasonBalNode?.InnerText, out seasonGap);
                int.TryParse(seriesBalNode?.InnerText, out seriesGap);
                int.TryParse(allTimeBalNode?.InnerText, out allTimeGap);
                return new Gap
                {
                    SeasonMinGames = seasonGap,
                    SeriesMinGames = seriesGap,
                    AllTimeMinGames = allTimeGap
                };

            }
            catch (FileNotFoundException)
            {
                ShowErrorMessage("ERROR", GetLocalized("SettingsNotFoundError"));
            }
            return null;
        }

        public static bool SaluteIsMainRatingSystem()
        {
            try
            {
                var xml = new XmlDocument();
                xml.Load($@"{Directory.GetCurrentDirectory()}\Settings.xml");
                var ratingNode = xml.SelectSingleNode("/Settings/MainRatingSystem");
                if (ratingNode == null) return true;
                return ratingNode.InnerText == "salute";
            }
            catch (FileNotFoundException)
            {
                ShowErrorMessage("ERROR", GetLocalized("SettingsNotFoundError"));
                return true;
            }
        }
        public static CultureInfo GetCulture()
        {
            var xml = new XmlDocument();
            xml.Load($@"{Directory.GetCurrentDirectory()}\Settings.xml");
            var cultureNode = xml.SelectSingleNode("/Settings/Language");
            return cultureNode == null
                ? CultureInfo.GetCultureInfo("en-US")
                : CultureInfo.GetCultureInfo(cultureNode.InnerText);
        }

        public static bool IsApplyRatio()
        {
            var xml = new XmlDocument();
            xml.Load($@"{Directory.GetCurrentDirectory()}\Settings.xml");
            var ratioNode = xml.SelectSingleNode("/Settings/UseFiimRatio");
            if (ratioNode == null)
                return true;
            return ratioNode.InnerText == "True";
        }

        public static void ShowErrorMessage(string header, string message)
        {
            var window = Application.Current.Windows.OfType<MetroWindow>().SingleOrDefault(x => x.IsActive);
            window?.Focus();
            var settings = new MetroDialogSettings
            {
                ColorScheme = MetroDialogColorScheme.Accented,
                AnimateShow = true,
                AnimateHide = true
            };
            window?.ShowMessageAsync(header, message,
                MessageDialogStyle.Affirmative, settings);
        }

        public static void Export(string title)
        {
            var settings = new MetroDialogSettings
            {
                ColorScheme = MetroDialogColorScheme.Accented,
                AnimateShow = true,
                AnimateHide = true
            };
            var dlg = new Microsoft.Win32.SaveFileDialog
            {
                FileName = $"{title}__{DateTime.Now.ToShortDateString().Replace("/", "")}",
                DefaultExt = ".png",
                Filter = "Png files (*.png)|*.png"
            };
            var result = dlg.ShowDialog();
            if (result != true) return;
            var screen = Application.Current.Windows.OfType<MetroWindow>().SingleOrDefault(x => x.IsActive);
            using (var stream = File.Open(dlg.FileName, FileMode.OpenOrCreate))
            {
                if (screen == null) return;
                var w = (int)screen.Width * 2;
                var h = (int)screen.Height * 2;
                var visual = new DrawingVisual();
                using (var context = visual.RenderOpen())
                {
                    context.DrawRectangle(Brushes.Black, null, new Rect(new Point(), new Size(screen.Width, screen.Height)));
                    context.DrawRectangle(new VisualBrush(screen), null,
                        new Rect(new Point(), new Size(screen.Width, screen.Height)));
                }
                visual.Transform = new ScaleTransform(w / screen.ActualWidth, h / screen.ActualHeight);
                var rtb = new RenderTargetBitmap(w, h, 96, 96, PixelFormats.Pbgra32);
                rtb.Render(visual);
                var crop = new CroppedBitmap(rtb, new Int32Rect(0, 60, w, h - 60));
                var enc = new PngBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(crop));
                enc.Save(stream);
                screen.ShowMessageAsync("SUCCESS", GetLocalized("ExportComplete"), MessageDialogStyle.Affirmative, settings);
            }
        }
    }
}
