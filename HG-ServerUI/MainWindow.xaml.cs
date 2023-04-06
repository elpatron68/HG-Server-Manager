using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using Microsoft.Win32;


namespace HG_ServerUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        SettingsModel settingsModel = new();
        public MainWindow()
        {
            InitializeComponent();

            settingsModel = SettingsModel.AddPaths(settingsModel);
            settingsModel = SettingsFile.ReadConfigfile(settingsModel);

            TxExePath.DataContext = settingsModel;
            TxServerName.DataContext = settingsModel;
            TxPortTcp.DataContext = settingsModel;
            TxPortUdp.DataContext = settingsModel;
            TxPortSteam.DataContext = settingsModel;
            CbBoat.DataContext = settingsModel;
            CbLocation.DataContext = settingsModel;
            CbCourse.DataContext = settingsModel;
            NmMaxClients.DataContext = settingsModel;
            NmMinPlayers.DataContext = settingsModel;
            TxPassword.DataContext = settingsModel;
            TxAdminPassword.DataContext = settingsModel;
            NmMaxRaceTime.DataContext = settingsModel;
            NmSessiontimeSetup.DataContext = settingsModel;
            NmSessiontimePrestart.DataContext = settingsModel;
            NmSessiontimePostrace.DataContext = settingsModel;
            NmWindMinSpeed.DataContext = settingsModel;
            NmWindMaxSpeed.DataContext = settingsModel;
            NmWindHeading.DataContext = settingsModel;
            NmWindVariation.DataContext = settingsModel;
            NmWindEvolutionGain.DataContext = settingsModel;
            NmOcsDragGain.DataContext = settingsModel;
            NmWindShadowScale.DataContext = settingsModel;
            NmBoundaryDrag.DataContext = settingsModel;
            NmPenaltyDragGain.DataContext = settingsModel;
            CheckUseCollisions.DataContext = settingsModel;
            NmGapToClear.DataContext = settingsModel;
            NmClientSlowdown.DataContext = settingsModel;
            NmPenaltyDuration.DataContext = settingsModel;
            NmBlackFlagDuration.DataContext = settingsModel;
            NmBlackFlagLegs.DataContext = settingsModel;
            NmMaxSpectators.DataContext = settingsModel;
            LbExternalIp.DataContext = settingsModel;
            LbServerStatus.DataContext = settingsModel;
            BtnStartServer.DataContext = settingsModel;
        }

        private void MnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            string f = ofd.FileName;
            settingsModel.Exepath = f;
        }

        private void BtnStartServer_Click(object sender, RoutedEventArgs e)
        {
            if (!settingsModel.Serverprocessrunning)
            {
                SettingsFile.Writefile(settingsModel);
                Process server = new Process();
                server.StartInfo.UseShellExecute = false;
                server.StartInfo.FileName = settingsModel.Exepath;
                server.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(settingsModel.Exepath);
                server.EnableRaisingEvents = true;
                server.Exited += new EventHandler(ProcessExited);
                server.Start();
                settingsModel.Processid = server.Id;
                settingsModel.Serverprocessrunning = true;
                settingsModel.Btnservercontent = "_Stop server";
            }
            else
            {
                Process[] process = Process.GetProcesses();
                foreach (Process p in process)
                {
                    if (p.Id == settingsModel.Processid)
                    {
                        try
                        {
                            p.Kill();
                            settingsModel.Serverprocessrunning = false;
                            settingsModel.Btnservercontent = "_Start server";
                        }
                        catch { }
                        //TimeSpan dauer = r.EndTime - r.StartTime;
                        //string d = dauer.ToString(@"hh\:mm\:ss", null);
                        //SendMsg(r.TargetId, $"Die Remote-Spiegelsitzung von {r.SourceUserName} wurde beendet.");
                        //Log.Information($"{_localUsername}::Session (process id {r.ProcessId}) terminated by exiting the application. Duration: {d}");
                        break;
                    }
                }
            }
        }

        private void ProcessExited(object sender, EventArgs e)
        {
            settingsModel.Btnservercontent = "_Start server";
            settingsModel.Serverprocessrunning = false;
        }

        private void MnHgSteam_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://store.steampowered.com/app/1448820/Hydrofoil_Generation/");
        }

        private void MnHgDiscord_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://discord.gg/paQbBgWM");
        }

        private void MnProjectOnGithub_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/elpatron68/HG-ServerUI");
        }

        private void MnOpenLogfile_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("notepad.exe", settingsModel.Logfilepath);
        }

    }
}
