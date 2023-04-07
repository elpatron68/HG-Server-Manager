using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
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
        private readonly DispatcherTimer checkServerRunningTimer = new DispatcherTimer();
        private readonly FileSystemWatcher _cfgFileSystemWatcher;

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

            PreFlightCheck();
            _cfgFileSystemWatcher = new FileSystemWatcher(settingsModel.Configfiledirectory);
            _cfgFileSystemWatcher.Filter = $"{System.IO.Path.GetFileName(settingsModel.Configfilepath)}";
            _cfgFileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
            _cfgFileSystemWatcher.Changed += HandleChanged;
            _cfgFileSystemWatcher.EnableRaisingEvents = true;

            checkServerRunningTimer.Interval = TimeSpan.FromSeconds(5);
            checkServerRunningTimer.Tick += checkServerRunningTimer_Tick;

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
            LbServerReachable.DataContext = settingsModel;
            BtnStartServer.DataContext = settingsModel;            
        }

        private void checkServerRunningTimer_Tick(object? sender, EventArgs e)
        {
            settingsModel.Serverprocessrunning = IsServerRunning();
            Log.Debug($"ServerRunning: {settingsModel.Serverprocessrunning}");
            //TestPortAsync();
        }

        private void PreFlightCheck()
        {
            if (!File.Exists(settingsModel.Exepath))
            {
                Log.Error("Critical error! ⚠️");
                Log.Warning("HG server executable not found! ⚠️");
                settingsModel.Exepath= "HG server executable not found!";
                BtnStartServer.Content = "Error ⚠️";
                BtnStartServer.IsEnabled=false;
                return;
            }
            else
            {
                Log.Information("HG server executable found ✅");
            }
            if (!Directory.Exists(settingsModel.Configfiledirectory))
            {
                Log.Warning("HG server config dir not found ⚠️");
                return;
            }
            else
            {
                Log.Information("HG server config dir exists ✅");
            }
            Log.Information($"{settingsModel.Boats.Count()} boats found");
            Log.Information($"{settingsModel.Courses.Count()} courses found");
            Log.Information($"{settingsModel.Locations.Count()} locations found");
            Log.Information($"{Directory.GetFiles(settingsModel.Configfiledirectory).Count()} " +
                $"configuration files found");
            if(Network.Testport("127.0.0.1", int.Parse(settingsModel.Tcpport), TimeSpan.FromMilliseconds(100)))
            {
                Log.Warning($"Port {settingsModel.Tcpport} is open, server already running? ⚠️");
                BtnStartServer.IsEnabled = false;
            }
            else
            {
                Log.Information($"Server port {settingsModel.Tcpport} is free  ✅");
            }
            if (IsServerRunning())
            {
                Log.Warning("Another server process is running! ⚠️");
                BtnStartServer.IsEnabled = false;
            }
            else
            {
                Log.Information("No other server process is running ✅");
            }
        }

        private void MnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void RunProcess()
        {
            Log.Information("Starting HG server");
            _cfgFileSystemWatcher.EnableRaisingEvents = false;
            SettingsFile.WriteConfigfile(settingsModel);
            _cfgFileSystemWatcher.EnableRaisingEvents = true;
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
            Log.Information("Sending message to Ntfy channel 📫");
            await SendNtfyAsync();
        }

        private void BtnStartServer_Click(object sender, RoutedEventArgs e)
        {
            if (!settingsModel.Serverprocessrunning)
            {
                RunProcess();
                //Thread.Sleep(4000);
                TestPortAsync();
            }
            else
            {
                KillServerProcess();
            }
        }

        private void KillServerProcess()
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
                        settingsModel.Serverreachable = false;
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

        private bool IsServerRunning()
        {
            Process[] process = Process.GetProcesses();
            foreach (Process p in process)
            {
                if (p.ProcessName.Contains("hg_server"))
                {
                    return true;
                }
            }
            return false;
        }

        private void ProcessExited(object sender, EventArgs e)
        {
            settingsModel.Btnservercontent = "_Start server";
            settingsModel.Serverprocessrunning = false;
        }

        private void MnHgSteam_Click(object sender, RoutedEventArgs e)
        {
            _ = Process.Start(new ProcessStartInfo("https://store.steampowered.com/app/1448820/Hydrofoil_Generation/") { UseShellExecute = true });
        }

        private void MnHgDiscord_Click(object sender, RoutedEventArgs e)
        {
            _ = Process.Start(new ProcessStartInfo("https://discord.gg/paQbBgWM") { UseShellExecute = true });
        }

        private void MnProjectOnGithub_Click(object sender, RoutedEventArgs e)
        {
            _ = Process.Start(new ProcessStartInfo("https://github.com/elpatron68/HG-Server-Manager") { UseShellExecute = true });
        }

        private void MnOpenLogfile_Click(object sender, RoutedEventArgs e)
        {
            _ = Process.Start("notepad.exe", settingsModel.Logfilepath);
        }

        private async Task SendNtfyAsync()
        {
            string _passtext=string.Empty;
            if (settingsModel.Password != "")
            {
                _passtext = "Private server, password protected";
            }
            else
            {
                _passtext = "Open server, no password set";
            }
            // Create a new ntfy client
            var topic = "Hydrofoil_Generation_Servermonitor";
            var client = new Client("https://ntfy.sh");
            var message = new SendingMessage
            {
                Title = "A new Hydrofoil Generation server started!",
                Message = $"Server name: {settingsModel.Servername}\n" +
                $"Location: {settingsModel.Location}\n" +
                $"Course: {settingsModel.Course}\n" +
                $"Wind: Min {settingsModel.Windminspeed.ToString("0.#", CultureInfo.InvariantCulture)} kt, " +
                $"max {settingsModel.Windmaxspeed.ToString("0.#", CultureInfo.InvariantCulture)} kt\n" +
                $"{_passtext}",
                Tags = new[]
                {
                    "rocket", "boat"
                }
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
            _ = Process.Start(new ProcessStartInfo("https://ntfy.sh/Hydrofoil_Generation_Servermonitor") { UseShellExecute = true });
        }

        private void MnSave_Click(object sender, RoutedEventArgs e)
        {
            _cfgFileSystemWatcher.EnableRaisingEvents = false;
            SettingsFile.WriteConfigfile(settingsModel);
            _cfgFileSystemWatcher.EnableRaisingEvents = true;
            string filename = System.IO.Path.GetFileName(settingsModel.Configfilepath);
            Log.Information($"Saved configuration as {filename}");
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
                _cfgFileSystemWatcher.EnableRaisingEvents = false;
                SettingsFile.WriteConfigfile(settingsModel, ofd.FileName);
                _cfgFileSystemWatcher.EnableRaisingEvents = true;
                Log.Information($"Saved configuration as {ofd.FileName}");
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
                Log.Information($"Configuration loaded from {ofd.FileName}");
            }
        }


        public async void TestPortAsync()
        {
            Thread.Sleep(4000);
            string address = settingsModel.Externalip;
            int port = int.Parse(settingsModel.Tcpport);
            int connectTimeoutMilliseconds = 1000;

            var tcpClient = new TcpClient();
            var connectionTask = tcpClient
                .ConnectAsync(address, port).ContinueWith(task => {
                    return task.IsFaulted ? null : tcpClient;
                }, TaskContinuationOptions.ExecuteSynchronously);
            var timeoutTask = Task.Delay(connectTimeoutMilliseconds)
                .ContinueWith<TcpClient>(task => null, TaskContinuationOptions.ExecuteSynchronously);
            var resultTask = Task.WhenAny(connectionTask, timeoutTask).Unwrap();
            var resultTcpClient = await resultTask.ConfigureAwait(false);

            if (resultTcpClient != null)
            {
                settingsModel.Serverreachable = true;
                Log.Information("Server is accessible to the public");
            }
            else
            {
                settingsModel.Serverreachable = false;
                Log.Information("LAN-only server");
            }
        }

        /// <summary>
        /// Display message dialog
        /// </summary>
        /// <param name="title"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private async Task<MessageDialogResult> MetroMessage(string title, string message)
        {
            MetroDialogSettings dialogSettings = new MetroDialogSettings();
            //dialogSettings.AffirmativeButtonText = answers[rInt] + " [OK]";

            MessageDialogResult dialogResult = await this.ShowMessageAsync(title,
                message,
                MessageDialogStyle.Affirmative, dialogSettings);

            return dialogResult;
        }

        private void MetroWindow_Closing(object sender, CancelEventArgs e)
        {
            if(settingsModel.Serverprocessrunning)
            {
                Log.Information("Exiting application");
                MessageBoxResult result = MessageBox.Show("The server process is still runnning.\nKill server process and proceed?",
                    "Warning", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    KillServerProcess();
                }
                else
                {
                    e.Cancel = true;
                    Log.Information("Exiting cancelled");
                }
            }
        }

        private void Image_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _ = Process.Start(new ProcessStartInfo("https://github.com/elpatron68/HG-Server-Manager") { UseShellExecute = true });
        }

        private void MnEdit_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("notepad.exe", $"{settingsModel.Configfilepath}");
        }

        private void HandleChanged(object sender, FileSystemEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show($"The configuration file {settingsModel.Configfilepath} has been modified outside the application.\n\nLoad changes?",
                    "Warning", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                SettingsFile.ReadConfigfile(settingsModel);
            }
        }
    }
}
