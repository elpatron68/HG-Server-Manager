using Discord.WebSocket;
using Discord;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HG_ServerUI
{
    /// <summary>
    /// Interaktionslogik für ResultsView.xaml
    /// </summary>
    public partial class ResultsView : MetroWindow
    {
        private Discordbot _discordRacebot = new();
        private SettingsModel _settingsModel = new SettingsModel();
        private List<string> _resultFiles = new();

        public ResultsView(SettingsModel settingsModel)
        {
            InitializeComponent();
            _settingsModel = settingsModel;
            _resultFiles = GetResultFiles(_settingsModel.Resultsdirectory);

            if (_resultFiles.Count > 0)
            {
                _resultFiles.Add("Cumulated series results");
                CalculateRaceSeries(_resultFiles);
                CbResultfiles.ItemsSource = _resultFiles;
            }
            else
            {
                CbResultfiles.Items.Add("No regatta results found.");
                CbResultfiles.SelectedIndex = 0;
            }
        }

        private void CalculateRaceSeries(List<string> _resultfiles)
        {
            List<Boatpoints> bp = new List<Boatpoints>();

            foreach (string resultfile in _resultfiles)
            {
                List<RaceEntry> results = GetResultsfromfile(resultfile);
                foreach (RaceEntry r in results)
                {
                    bool _match = false;
                    foreach (Boatpoints _bp in bp)
                    {
                        // Existing boat name
                        if (r.name == _bp.boatname)
                        {
                            _bp.points += r.points;
                            _bp.races += 1;
                            _match= true;
                            break;
                        }
                    }
                    if(!_match)
                    {
                        // New boat name?
                        var b = new Boatpoints();
                        b.boatname = r.name;
                        b.points += r.points;
                        b.races += 1;
                        bp.Add(b);
                    }
                }
            }
            DgResults.ItemsSource=bp;
            LbDgname.Content = "Cumulated regatta series points";
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
            if (_filename == "Cumulated series results")
            {
                CalculateRaceSeries(_resultFiles);
            }
            else
            {
                if (File.Exists(_filename))
                {
                    DgResults.ItemsSource = (IEnumerable)GetResultsfromfile(_filename);
                    DateTime fileCreatedDate = File.GetCreationTimeUtc(_filename);
                    string _timestamp = fileCreatedDate.ToString("yyyy.MM.dd HH:mm:ss");
                    string _lbtext = $"Results from '{Path.GetFileName(_filename)}' ({_timestamp})";
                    LbDgname.Content = _lbtext;
                }
            }
        }


        private List<RaceEntry> GetResultsfromfile(string _filename)
        {
            if (File.Exists(_filename))
            {
                Root? RaceResults = JsonConvert.DeserializeObject<Root>(File.ReadAllText(_filename));
                int _rank = 1;
                int _numboats = 0;
                foreach (var entry in RaceResults.entries)
                {
                    if (entry.race_time > 0)
                    {
                        entry.rank = _rank;
                        entry.race_time = Math.Round(entry.race_time, 2, MidpointRounding.AwayFromZero);
                        _numboats++;
                        _rank++;
                    }
                    else
                    {
                        entry.points = 0;
                        entry.rank = 99;
                    }
                }
                foreach (var entry in RaceResults.entries)
                {
                    if (entry.race_time > 0)
                    {
                        entry.points = _numboats;
                        _numboats--;
                    }
                }
                return RaceResults.entries;
            }
            return new List<RaceEntry>();
        }


        public async void Dg2Bitmap(int width, int heigth)
        {
            string _filename = Path.GetDirectoryName(CbResultfiles.Text) + @"\" +
                    Path.GetFileNameWithoutExtension(CbResultfiles.Text) + ".png";
            bool _isCumulative = false;

            if (CbResultfiles.SelectedItem == null)
            {
                _filename = Path.Combine(_settingsModel.Resultsdirectory, "series_results.png");
                _isCumulative = true;
            }
            else
            {
                if(CbResultfiles.SelectedItem.ToString() == "Cumulated series results")
                {
                    _filename = Path.Combine(_settingsModel.Resultsdirectory, "series_results.png");
                    _isCumulative = true;
                }
            }

            RenderTargetBitmap renderTargetBitmap =
                new RenderTargetBitmap(width, heigth, 96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(DgResults);
            PngBitmapEncoder pngImage = new PngBitmapEncoder();
            pngImage.Frames.Add(BitmapFrame.Create(renderTargetBitmap));
            using (Stream fileStream = File.Create(_filename))
            {
                pngImage.Save(fileStream);
            }

            var client = new DiscordSocketClient();
            await client.LoginAsync(TokenType.Bot, _discordRacebot.DiscordbotToken);
            await client.StartAsync();

            DateTime fileCreatedDate = File.GetCreationTimeUtc(_filename);
            string _timestamp = fileCreatedDate.ToString("yyyy.MM.dd HH:mm:ss");
            string _text = string.Empty;
            if (_isCumulative)
            {
                _text = $"**Cumulative race series results for {_settingsModel.Servername}** :checkered_flag:\n";
            }
            else
            {
                _text = $"**Race results for {_settingsModel.Servername}** :checkered_flag:\n" +
                $"Race ended at {_timestamp} UTC";
            }
            
            var embed = new EmbedBuilder()
                .WithImageUrl($"attachment://{_filename}")
                .WithDescription(_text)
                .Build();

            var channel = await client.GetChannelAsync(_discordRacebot.Discordchannelid) as IMessageChannel;

            MemoryStream memoryStream = new MemoryStream();
            using (FileStream file = new FileStream(_filename, FileMode.Open, FileAccess.Read))
                file.CopyTo(memoryStream);

            var result = await client.GetChannelAsync(_discordRacebot.Discordchannelid) as IMessageChannel;
            await result!.SendFileAsync(stream: memoryStream, filename: _filename, embed: embed);
            //try { File.Delete("results.png"); } catch (Exception) { }
        }

        private void BtSavePng_Click(object sender, RoutedEventArgs e)
        {
            if(DgResults.Items.Count>0)
            {
                int _width = Convert.ToInt32(DgResults.ActualWidth);
                int _heigth = Convert.ToInt32(DgResults.ActualHeight) + 40;
                Dg2Bitmap(_width, _heigth);
            }
        }

        public class RaceEntry
        {
            public int rank { get; set; }
            public string? name { get; set; }
            public string? boat_model { get; set; }
            public double race_time { get; set; }
            public double flight_time { get; set; }
            public int points { get; set; }
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
