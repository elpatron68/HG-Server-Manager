﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using MahApps.Metro.Controls;
using MahApps.Metro.Converters;
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
        public Penaltyviewer(SettingsModel settingsModel)
        {
            InitializeComponent();
            var PenList= ConvertSvg2Png(settingsModel.Snapsdirectory);
            DgPenalties.ItemsSource = PenList;
        }

        private List<Penalty> ConvertSvg2Png(string svgPath)
        {
            foreach(string svgFile in Directory.GetFiles(svgPath, "*.svg"))
            {
                string pngfile = Path.Combine(Path.GetDirectoryName(svgFile), Path.GetFileNameWithoutExtension(svgFile) + ".png");
                Penalty penalty = new(svgFile);
                if (!File.Exists(pngfile))
                {
                    using (var svg = new SKSvg())
                    {
                        if (svg.Load(svgFile) is { })
                        {
                            using (var stream = File.OpenWrite(pngfile))
                            {
                                SKColor _white = SKColor.Parse("ffffff");
                                _ = svg.Save(stream, _white, SKEncodedImageFormat.Png, 100);
                            }
                        }
                    }
                }
                penalty.Fullpath = pngfile;
                penalty.Filename = Path.GetFileNameWithoutExtension(penalty.Fullpath) + ".png";
                penaltyList.Add(penalty);
            }
            return penaltyList;
        }

        private void DgPenalties_AutoGeneratingColumn(object sender, System.Windows.Controls.DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "Fullpath" || e.PropertyName== "Filename")
            {
                e.Column = null;
            }
        }

        private void DgPenalties_SelectedCellsChanged(object sender, System.Windows.Controls.SelectedCellsChangedEventArgs e)
        {
            var _index = DgPenalties.SelectedIndex;
            ImgPenalty.Source = new BitmapImage(new Uri(penaltyList[_index].Fullpath));
        }
    }
}