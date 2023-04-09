using System;
using System.ComponentModel;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Windows.Threading;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Microsoft.Xaml.Behaviors.Media;
using ntfy;
using Serilog;
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
        private readonly FileSystemWatcher _penaltiesFileSystemWatcher;
        public static RoutedCommand cmdSlotZero = new RoutedCommand();
        public static RoutedCommand cmdSlotOne = new RoutedCommand();
        public static RoutedCommand cmdSlotTwo = new RoutedCommand();
        public static RoutedCommand cmdSlotThree = new RoutedCommand();
        public static RoutedCommand cmdSlotFour = new RoutedCommand();
        public static RoutedCommand cmdSlotFive = new RoutedCommand();
        public static RoutedCommand cmdSlotSix = new RoutedCommand();
        public static RoutedCommand cmdSlotSeven = new RoutedCommand();
        public static RoutedCommand cmdSlotEight = new RoutedCommand();
        public static RoutedCommand cmdSlotNine = new RoutedCommand();
        public static RoutedCommand cmdRunServer = new RoutedCommand();

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

            cmdSlotZero.InputGestures.Add(new KeyGesture(Key.D0, ModifierKeys.Control));
            cmdSlotOne.InputGestures.Add(new KeyGesture(Key.D1, ModifierKeys.Control));
            cmdSlotTwo.InputGestures.Add(new KeyGesture(Key.D2, ModifierKeys.Control));
            cmdSlotThree.InputGestures.Add(new KeyGesture(Key.D3, ModifierKeys.Control));
            cmdSlotFour.InputGestures.Add(new KeyGesture(Key.D4, ModifierKeys.Control));
            cmdSlotFive.InputGestures.Add(new KeyGesture(Key.D5, ModifierKeys.Control));
            cmdSlotSix.InputGestures.Add(new KeyGesture(Key.D6, ModifierKeys.Control));
            cmdSlotSeven.InputGestures.Add(new KeyGesture(Key.D7, ModifierKeys.Control));
            cmdSlotEight.InputGestures.Add(new KeyGesture(Key.D8, ModifierKeys.Control));
            cmdSlotNine.InputGestures.Add(new KeyGesture(Key.D9, ModifierKeys.Control));
            cmdRunServer.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control));

            // Initialize file system watschers
            _cfgFileSystemWatcher = new FileSystemWatcher(settingsModel.Configfiledirectory);
            _cfgFileSystemWatcher.Filter = $"{System.IO.Path.GetFileName(settingsModel.Configfilepath)}";
            _cfgFileSystemWatcher.NotifyFilter = NotifyFilters.LastAccess | 
                NotifyFilters.LastWrite | 
                NotifyFilters.FileName;
            _cfgFileSystemWatcher.Changed += CfgHandleChanged;
            _cfgFileSystemWatcher.EnableRaisingEvents = true;
            _penaltiesFileSystemWatcher = new FileSystemWatcher(settingsModel.Snapsdirectory);
            _penaltiesFileSystemWatcher.Filter = "*.svg";
            _penaltiesFileSystemWatcher.NotifyFilter = NotifyFilters.LastAccess | 
                NotifyFilters.LastWrite | 
                NotifyFilters.FileName;
            _penaltiesFileSystemWatcher.Created += PenaltyHandleChanged;
            _penaltiesFileSystemWatcher.EnableRaisingEvents=true;
            checkServerRunningTimer.Interval = TimeSpan.FromSeconds(5);
            checkServerRunningTimer.Tick += checkServerRunningTimer_Tick;

            // Set bindings
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
            TbPenalties.DataContext = settingsModel;
            TbNtfyRaceTopic.DataContext = settingsModel;
            TbNtfyPenaltyTopic.DataContext = settingsModel;
        }

        // New penalty?
        private async void PenaltyHandleChanged(object sender, FileSystemEventArgs e)
        {
            Log.Information("New penalty detected");
            string? _filename = e.FullPath;
            if (_filename != null)
            {
                Thread.Sleep(100);
                try
                {
                    //string _username = _filename.Split("_on_")[0].Trim();
                    //string _boatname = _filename.Split("_on_")[1].Split("_")[0].Trim();
                    //string _offence = _filename.Split("_-_")[1].Split('_')[1].Trim();
                    string _timestamp = $"[{DateTime.Now.ToString("HH:mm:ss")}]";
                    string _filecontent=File.ReadAllText(_filename);
                    string _username =string.Empty;
                    string _offence = string.Empty;
                    foreach (string line in _filecontent.Split("\n"))
                    {
                        if (line.Contains("font-size='40'"))
                        {
                            _username = line.Split('>')[1].Split('|')[0].Trim();
                            _offence = line.Split('|')[1].Split('<')[0].Trim();
                        }
                    }
                    settingsModel.Penalties += $"{_timestamp} {_username}: {_offence}\n";

                    SoundPlayer player = new(Properties.Resources.beep_sound);
                    player.Play();
                    if (settingsModel.Ntfyracectopic != string.Empty)
                    {
                        await SendNtfyPenaltyAnnouncement($"New penalty ({_offence}) by {_username} in race {settingsModel.Servername}.");
                    }
                }
                catch 
                {
                    Log.Warning($"Failed parsing penalty file name: {_filename}");
                }
            }
        }

        // Check if server is running
        private void checkServerRunningTimer_Tick(object? sender, EventArgs e)
        {
            settingsModel.Serverprocessrunning = IsServerRunning();
            Log.Debug($"ServerRunning: {settingsModel.Serverprocessrunning}");
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

        private async void RunServerProcess()
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
            settingsModel.Btnservercontent = "_Stop [crtl+s]";

#if !DEBUG
            if (settingsModel.Ntfyracectopic != string.Empty)
            {
                Log.Information("Sending message to Ntfy channel 📫");
                await SendNtfyRaceAnnouncement();
            }
#endif
        }


        private void BtnStartServer_Click(object sender, RoutedEventArgs e)
        {
            if (!settingsModel.Serverprocessrunning)
            {
                RunServerProcess();
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
                        settingsModel.Btnservercontent = "_Start [Crtl+s]";
                        Log.Information("HG server stopped");
                    }
                    catch (Exception ex)
                    {
                        Log.Warning($"Failed to stop server: {ex.Message}");
                    }
                    //TimeSpan dauer = r.EndTime - r.StartTime;
                    //string d = dauer.ToString(@"hh\:mm\:ss", null);
                    //SendMsg(r.TargetId, $"Die Remote-Spiegelsitzung von {r.SourceUserName} wurde beendet.");
                    //Log.Information($"{_localUsername}::Session (process id {r.ProcessId}) terminated by exiting the application. Duration: {d}");
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
            settingsModel.Btnservercontent = "_Start [Crtl+s]";
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

        private async Task SendNtfyRaceAnnouncement()
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
            var topic = settingsModel.Ntfyracectopic;
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

        private async Task SendNtfyPenaltyAnnouncement(string text)
        {
            // Create a new ntfy client
            var topic = settingsModel.Ntfypenaltytopic;
            var client = new Client("https://ntfy.sh");
            var message = new SendingMessage
            {
                Title = "A penalty occurred!",
                Message = $"Server name: {settingsModel.Servername}\n" + text,
                Tags = new[]
                {
                    "loudspeaker", "rotating_light"
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
            e.Handled = true;
        }

        private void MnEdit_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("notepad.exe", $"{settingsModel.Configfilepath}");
            e.Handled = true;
        }

        private void CfgHandleChanged(object sender, FileSystemEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show($"The configuration file {settingsModel.Configfilepath} has been modified outside the application.\n\nLoad changes?",
                    "Warning", MessageBoxButton.OKCancel);
            if (result == MessageBoxResult.OK)
            {
                SettingsFile.ReadConfigfile(settingsModel);
            }
        }

        private void NtfyHelp_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            _= Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
        }

        private void MnOpenSnaps_Click(object sender, RoutedEventArgs e)
        {
            _ = Process.Start("explorer.exe", settingsModel.Snapsdirectory);
        }

        private void SlotZeroCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LoadSlot(0);
        }

        private void SlotOneCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LoadSlot(1);
        }

        private void SlotTwoCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LoadSlot(2);
        }
        private void SlotThreeCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LoadSlot(3);
        }
        private void SlotFourCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LoadSlot(4);
        }
        private void SlotFiveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LoadSlot(5);
        }
        private void SlotSixCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LoadSlot(6);
        }
        private void SlotSevenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LoadSlot(7);
        }
        private void SlotEightCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LoadSlot(8);
        }
        private void SlotNineCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            LoadSlot(9);
        }
        private void RunServerCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (!settingsModel.Serverprocessrunning)
            {
                RunServerProcess();
            }
            else
            {
                KillServerProcess();
            }
        }
        private void MnSlot0_Click(object sender, RoutedEventArgs e)
        {
            LoadSlot(0);
        }

        private void MnSlot1_Click(object sender, RoutedEventArgs e)
        {
            LoadSlot(1);
        }
        private void MnSlot2_Click(object sender, RoutedEventArgs e)
        {
            LoadSlot(2);
        }
        private void MnSlot3_Click(object sender, RoutedEventArgs e)
        {
            LoadSlot(3);
        }
        private void MnSlot4_Click(object sender, RoutedEventArgs e)
        {
            LoadSlot(4);
        }
        private void MnSlot5_Click(object sender, RoutedEventArgs e)
        {
            LoadSlot(5);
        }
        private void MnSlot6_Click(object sender, RoutedEventArgs e)
        {
            LoadSlot(6);
        }
        private void MnSlot7_Click(object sender, RoutedEventArgs e)
        {
            LoadSlot(7);
        }
        private void MnSlot8_Click(object sender, RoutedEventArgs e)
        {
            LoadSlot(8);
        }
        private void MnSlot9_Click(object sender, RoutedEventArgs e)
        {
            LoadSlot(9);
        }

        private async void LoadSlot(int slotnumber)
        {
            string _filename = settingsModel.Configfiledirectory + $@"\slot{slotnumber.ToString()}.kl";
            if(File.Exists(_filename))
            {
                if (settingsModel.Serverprocessrunning)
                {
                    KillServerProcess();
                }
                //settingsModel = new();
                SettingsFile.ReadConfigfile(settingsModel, $@"{settingsModel.Configfiledirectory}\slot{slotnumber.ToString()}.kl");
//#if !DEBUG
                RunServerProcess();
//#endif
                Log.Information($"Slot {slotnumber} loaded, server (re)started");
                await this.ShowMessageAsync("Hot slot loaded", $"Hot slot #{slotnumber.ToString()} loaded, " +
                    $"server (re)started.");
            }
            else
            {
                try
                {
                    File.Copy(settingsModel.Configfilepath, settingsModel.Configfiledirectory + $@"\slot{slotnumber.ToString()}.kl");
                    SettingsFile.ReadConfigfile(settingsModel, $@"{settingsModel.Configfiledirectory}\slot{slotnumber.ToString()}.kl");
                    await this.ShowMessageAsync("New hot slot created", $"Hot slot #{slotnumber.ToString()} did not exist, " +
                        $"a new configuration based on the 'server_cfg.kl' has been created and loaded.\n" +
                        $"Adjust the settings to your needs and save it as 'slot{slotnumber.ToString()}.kl'.\n");
                }
                catch
                {
                    Log.Warning($"Failed to copy {Path.GetFileName(settingsModel.Configfilepath)} to slot{slotnumber.ToString()}.kl");
                }
            }
        }

        private async void MnDeleteSnaps_Click(object sender, RoutedEventArgs e)
        {
            MessageDialogResult result = await this.ShowMessageAsync("Delete?", $"Delete all svg files in {settingsModel.Snapsdirectory}?");
            if (result == MessageDialogResult.Affirmative)
            {
                Log.Information("Cleaning up snaps directory");
                foreach (var _filename in Directory.GetFiles(settingsModel.Snapsdirectory, "*.svg"))
                {
                    try
                    {
                        File.Delete(_filename);
                    }
                    catch (Exception ex)
                    {
                        Log.Warning($"Failed to delete {_filename}");
                    }
                }
            }
        }
    }
}
