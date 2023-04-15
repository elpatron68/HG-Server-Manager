using Discord.WebSocket;
using Discord;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using System.Net.Http;

namespace HG_ServerUI
{
    /// <summary>
    /// Interaktionslogik für ResultsView.xaml
    /// </summary>
    public partial class ResultsView : MetroWindow
    {
        private Discordbot _discordRacebot = new();
        public ResultsView(SettingsModel settingsModel)
        {
            InitializeComponent();
            CbResultfiles.ItemsSource = GetResultFiles(settingsModel.Resultsdirectory);
        }

        private List<string> GetResultFiles(string _path)
        {
            string[] _files= Directory.GetFiles(_path, "*.json");
            List<string> _list = new(_files);
            return _list;
        }

        private void CbResultfiles_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string? _filename = CbResultfiles.SelectedItem.ToString();
            if (File.Exists(_filename))
            {
                DgResults.ItemsSource = (IEnumerable)GetResultsfromfile(_filename);
            }
        }


        private List<RaceEntry> GetResultsfromfile(string _filename)
        {
            Root? myDeserializedClass = JsonConvert.DeserializeObject<Root>(File.ReadAllText(_filename));
            return myDeserializedClass.entries;
        }


        public void Dg2Bitmap(int width, int heigth)
        {
            RenderTargetBitmap renderTargetBitmap =
                new RenderTargetBitmap(width, heigth, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(DgResults);
            PngBitmapEncoder pngImage = new PngBitmapEncoder();
            pngImage.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            using (Stream fileStream = File.Create("results.png"))
            {
                pngImage.Save(fileStream);
            }
        }

        private void BtSavePng_Click(object sender, RoutedEventArgs e)
        {
            int _width = Convert.ToInt32(DgResults.ActualWidth);
            int _heigth = Convert.ToInt32(DgResults.ActualHeight) + 20;
            Dg2Bitmap(_width, _heigth);
            SendResult2Discord();
        }

        private async void SendResult2Discord()
        {
            var client = new DiscordSocketClient();
            await client.LoginAsync(TokenType.Bot, _discordRacebot.DiscordbotToken);
            await client.StartAsync();

            const string fileName = "results.png";
            var embed = new EmbedBuilder()
                .WithImageUrl($"attachment://{fileName}")
                .WithDescription("Race results")
                .Build();

            var channel = await client.GetChannelAsync(_discordRacebot.Discordchannelid) as IMessageChannel;

            MemoryStream memoryStream = new MemoryStream();
            using (FileStream file = new FileStream("results.png", FileMode.Open, FileAccess.Read))
                file.CopyTo(memoryStream);

            var result = await client.GetChannelAsync(_discordRacebot.Discordchannelid) as IMessageChannel;
            await result!.SendFileAsync(stream: memoryStream, filename: fileName, embed: embed);
            try { File.Delete("results.png"); } catch (Exception) { }
        }

        public class RaceEntry
        {
            public string? name { get; set; }
            public string? boat_model { get; set; }
            public object id { get; set; }
            public double race_time { get; set; }
            public double flight_time { get; set; }
        }

        public class Regatta
        {
            public string? regatta_id { get; set; }
            public string? race_id { get; set; }
        }

        public class Root
        {
            public Regatta http { get; set; }
            public List<RaceEntry> entries { get; set; }
        }
    }
}
