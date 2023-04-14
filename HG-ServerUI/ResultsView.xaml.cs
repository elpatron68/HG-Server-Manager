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
using System.Windows.Shapes;

namespace HG_ServerUI
{
    /// <summary>
    /// Interaktionslogik für ResultsView.xaml
    /// </summary>
    public partial class ResultsView : MetroWindow
    {
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

        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
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
