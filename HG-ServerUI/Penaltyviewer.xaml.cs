using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using SkiaSharp;
using Svg.Skia;


namespace HG_ServerUI
{
    /// <summary>
    /// Interaktionslogik für Penaltyviewer.xaml
    /// </summary>
    public partial class Penaltyviewer : MetroWindow
    {
        private List<Penalty> penaltyList = new();
        private SettingsModel model;
        public Penaltyviewer(SettingsModel settingsModel)
        {
            model = settingsModel;
            InitializeComponent();
            penaltyList = ConvertSvg2Png();
            DgPenalties.ItemsSource = penaltyList;
        }

        private List<Penalty> ConvertSvg2Png()
        {
            foreach(string svgFile in Directory.GetFiles(model.Snapsdirectory, "*.svg"))
            {
                string pngfile = Path.Combine(Path.GetDirectoryName(svgFile), Path.GetFileNameWithoutExtension(svgFile) + ".png");
                Penalty penalty = new(svgFile);
                if (!File.Exists(pngfile))
                {
                    using var svg = new SKSvg();
                    if (svg.Load(svgFile) is { })
                    {
                        try
                        {
                            using (var stream = File.OpenWrite(pngfile))
                            {
                                SKColor _white = SKColor.Parse("ffffff");
                                _ = svg.Save(stream, _white, SKEncodedImageFormat.Png, 100);
                            }
                        }
                        catch { }
                    }
                }
                penaltyList.Add(penalty);
            }
            return penaltyList;
        }

        /// <summary>
        /// Remove columns from DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DgPenalties_AutoGeneratingColumn(object sender, System.Windows.Controls.DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "PngFilename" || 
                e.PropertyName== "PngFullpath" ||
                e.PropertyName == "SvgFullpath")
            {
                e.Column = null;
            }
        }

        private void DgPenalties_SelectedCellsChanged(object sender, System.Windows.Controls.SelectedCellsChangedEventArgs e)
        {
            int _index = DgPenalties.SelectedIndex;
            ImgPenalty.Source = new BitmapImage(new Uri(penaltyList[_index].PngFullpath));
        }

        private async Task MnDelete_ClickAsync(object sender, System.Windows.RoutedEventArgs e)
        {
            _ = await this.ShowMessageAsync("Not implemented",
                    "This funtion is not yet implemented.?",
                    MessageDialogStyle.Affirmative);
            //ImgPenalty.Source = null;
            //IList rows = DgPenalties.SelectedItems;
            //foreach (object? dgrow in rows)
            //{
            //    try
            //    {
            //        string? _f1 = ((Penalty)dgrow).PngFullpath;
            //        string? _f2 = ((Penalty)dgrow).SvgFullpath;
            //        File.Delete(_f1);
            //        File.Delete(_f2);
            //        //foreach(var penrow in penaltyList)
            //        //{
            //        //    if(penrow.PngFullpath == _f1)
            //        //    {
            //        //        penaltyList.Remove(penrow);
            //        //        return;
            //        //    }
            //        //}
            //    }
            //    catch(Exception ex)
            //    {
            //    }
            //    penaltyList = new();
            //    penaltyList = ConvertSvg2Png();
        }
        }
    }
}
