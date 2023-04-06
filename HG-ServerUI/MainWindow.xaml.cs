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
using ntfy;
using Serilog;
using Serilog.Sinks.RichTextBox;
using Serilog.Sinks.RichTextBox.Themes;

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

            Log.Logger = new LoggerConfiguration()
                .WriteTo.RichTextBox(RtbLogMessages,
                theme: RichTextBoxConsoleTheme.Grayscale,
                outputTemplate: "[{Timestamp:HH:mm:ss}] {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();
            Log.Information("HG Server Manager started");

            settingsModel = SettingsModel.AddPaths(settingsModel);
            settingsModel = SettingsFile.ReadConfigfile(settingsModel);
            Log.Information("Settings loaded");

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

        private async void RunProcess()
        {
            Log.Information("Starting HG server");
            SettingsFile.WriteConfigfile(settingsModel);
            Process server = new Process();
            server.StartInfo.UseShellExecute = false;
            server.StartInfo.CreateNoWindow = true;
            server.StartInfo.FileName = settingsModel.Exepath;
            server.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(settingsModel.Exepath);
            server.EnableRaisingEvents = true;
            server.Exited += new EventHandler(ProcessExited);
            server.Start();
            settingsModel.Processid = server.Id;
            settingsModel.Serverprocessrunning = true;
            settingsModel.Btnservercontent = "_Stop server";
            Log.Information("Sending message to Ntfy channel");
            await SendNtfyAsync();
        }

        private void BtnStartServer_Click(object sender, RoutedEventArgs e)
        {
            if (!settingsModel.Serverprocessrunning)
            {
                RunProcess();
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
                            Log.Information("HG server stopped");
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
            _ = Process.Start("https://store.steampowered.com/app/1448820/Hydrofoil_Generation/");
        }

        private void MnHgDiscord_Click(object sender, RoutedEventArgs e)
        {
            _ = Process.Start("https://discord.gg/paQbBgWM");
        }

        private void MnProjectOnGithub_Click(object sender, RoutedEventArgs e)
        {
            _ = Process.Start("https://github.com/elpatron68/HG-ServerUI");
        }

        private void MnOpenLogfile_Click(object sender, RoutedEventArgs e)
        {
            _ = Process.Start("notepad.exe", settingsModel.Logfilepath);
        }

        private async Task SendNtfyAsync()
        {
            // Create a new ntfy client
            var topic = "Hydrofoil_Generation_Servermonitor";
            var client = new Client("https://ntfy.sh");
            var message = new SendingMessage
            {
                Title = "A new Hydrofoil Generation server started!",
                Message = $"Server name: {settingsModel.Servername}"
            };
            try
            {
                await client.Publish(topic, message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void MnOpenNtfy_Click(object sender, RoutedEventArgs e)
        {
            _ = Process.Start("https://ntfy.sh/Hydrofoil_Generation_Servermonitor");
        }

        private void MnSave_Click(object sender, RoutedEventArgs e)
        {
            SettingsFile.WriteConfigfile(settingsModel);
        }

        private void MnSaveAs_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new()
            {
                InitialDirectory = settingsModel.Configfiledirectory,
                DefaultExt = "kl",
                Filter = "HG configuration files (*.kl)|*.kl"
            };
            if (ofd.ShowDialog() == true)
            {
                SettingsFile.WriteConfigfile(settingsModel, ofd.FileName);
            }
        }

        private void MnLoad_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new()
            {
                InitialDirectory = settingsModel.Configfiledirectory,
                DefaultExt = "kl",
                Filter = "HG configuration files (*.kl)|*.kl"
            };
            if (ofd.ShowDialog() == true)
            {
                settingsModel = SettingsFile.ReadConfigfile(settingsModel, ofd.FileName);
            }
        }
    }
}
