using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using Discord.WebSocket;
using Discord;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Serilog;
using Serilog.Sinks.RichTextBox.Themes;
using System.IO.Compression;

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
        private readonly FileSystemWatcher _resultsFileSystemWatcher;
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
        private Discordbot _discordRacebot=new();

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
            Log.Information($"App version: {settingsModel.Appversion}");
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
            _cfgFileSystemWatcher = new FileSystemWatcher(settingsModel.Configfiledirectory)
            {
                Filter = $"{System.IO.Path.GetFileName(settingsModel.Configfilepath)}",
                NotifyFilter = NotifyFilters.LastAccess |
                NotifyFilters.LastWrite |
                NotifyFilters.FileName
            };
            _cfgFileSystemWatcher.Changed += CfgHandleChanged;
            _cfgFileSystemWatcher.EnableRaisingEvents = true;

            _penaltiesFileSystemWatcher = new FileSystemWatcher(settingsModel.Snapsdirectory)
            {
                Filter = "*.svg",
                NotifyFilter = NotifyFilters.LastAccess |
                NotifyFilters.LastWrite |
                NotifyFilters.FileName
            };
            _penaltiesFileSystemWatcher.Created += PenaltyHandleChanged;
            _penaltiesFileSystemWatcher.EnableRaisingEvents=true;

            _resultsFileSystemWatcher = new FileSystemWatcher(settingsModel.Resultsdirectory)
            {
                Filter = "*.json",
                NotifyFilter = NotifyFilters.LastAccess |
                NotifyFilters.LastWrite |
                NotifyFilters.FileName
            };
            _resultsFileSystemWatcher.Created += ResultsHandleChanged;
            _resultsFileSystemWatcher.EnableRaisingEvents = true;

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
            WinHGSM.DataContext = settingsModel;
            TiBoats.DataContext = settingsModel;
            TiActiveCourse.DataContext = settingsModel;
            TbChat.DataContext = settingsModel;
            CbDiscordRace.DataContext = settingsModel;
        }

        // New regatta results
        private void ResultsHandleChanged(object sender, FileSystemEventArgs e)
        {
            Log.Information($"New race results: {e.Name}");
        }

        // New penalty
        private void PenaltyHandleChanged(object sender, FileSystemEventArgs e)
        {
            Log.Information("New penalty detected");
            string? _filename = e.FullPath;
            if (_filename != null)
            {
                Thread.Sleep(100);
                try
                {
                    string _timestamp = $"[{DateTime.Now.ToString("HH:mm:ss")}]";
                    string _filecontent = File.ReadAllText(_filename);
                    string _username = string.Empty;
                    string _offence = string.Empty;
                    foreach (string line in _filecontent.Split("\n"))
                    {
                        if (line.Contains("font-size='40'"))
                        {
                            _username = line.Split('>')[1].Split('|')[0].Trim();
                            _offence = line.Split('|')[1].Split('<')[0].Trim();
                        }
                    }
                    settingsModel.Penalties = $"{_timestamp} {_username}: {_offence}\n" + settingsModel.Penalties;

                    SoundPlayer player = new(Properties.Resources.beep_sound);
                    player.Play();
                }
                catch
                {
                    Log.Warning($"Failed parsing penalty, file name: {Path.GetFileName(_filename)}");
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
                BtnStartServer.IsEnabled = false;
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
            //if(Network.Testport("127.0.0.1", int.Parse(settingsModel.Tcpport), TimeSpan.FromMilliseconds(100)))
            //{
            //    Log.Warning($"Port {settingsModel.Tcpport} is open, server already running? ⚠️");
            //    BtnStartServer.IsEnabled = false;
            //}
            //else
            //{
            //    Log.Information($"Server port {settingsModel.Tcpport} is free  ✅");
            //}
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
            SendTab();
            SettingsFile.WriteConfigfile(settingsModel);
            _cfgFileSystemWatcher.EnableRaisingEvents = true;
            Process server = new Process();
            server.StartInfo.UseShellExecute = false;
            server.StartInfo.CreateNoWindow = true;
            server.StartInfo.FileName = settingsModel.Exepath;
            server.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(settingsModel.Exepath);
            server.EnableRaisingEvents = true;
            server.StartInfo.RedirectStandardOutput = true;
            server.OutputDataReceived += ConsoleOutputHandler;
            server.Exited += new EventHandler(ProcessExited);
            server.Start();
            server.BeginOutputReadLine();
            settingsModel.Processid = server.Id;
            settingsModel.Serverprocessrunning = true;
            settingsModel.Btnservercontent = "_Stop [crtl+s]";
            //ToggleControls(false);
            TestPortAsync();
            Thread.Sleep(1000);
            if (settingsModel.Serverreachable && settingsModel.DiscordracenotificationEnabled)
            {
                MessageDialogResult _post2Discord = await this.ShowMessageAsync("Discord notification",
                    "Discord regatta notification is activated. Do you really want to announce this race publicly on Discord?",
                    MessageDialogStyle.AffirmativeAndNegative);
                if (_post2Discord == MessageDialogResult.Affirmative)
                {
                    string _passwordprotected = string.Empty;
                    if (settingsModel.Password.Length > 0)
                    {
                        _passwordprotected = "Private race";
                    }
                    else
                    {
                        _passwordprotected = "Open race";
                    }
                    string _message = $"**A New HG Regatta Has Started :rocket:**\n" +
                        $":trophy: Race name: *{settingsModel.Servername}*\n" +
                        $":world_map: Course: *{settingsModel.Course}*\n" +
                        $":earth_africa: Location: *{settingsModel.Location}*\n" +
                        $":sailboat: Boat: *{settingsModel.Boat}*\n" +
                        $":wind_blowing_face: Max wind: *{settingsModel.Windmaxspeed} kt*\n" +
                        $":white_sun_small_cloud: Min wind: *{settingsModel.Windminspeed} kt*\n"; // +
                    if (settingsModel.Password.Length > 0)
                    {
                        MessageDialogResult _postPrivaterace = await this.ShowMessageAsync("Private regatta",
                            "You have set a password. Do you want to announce the password on Discord?",
                            MessageDialogStyle.AffirmativeAndNegative);
                        if (_postPrivaterace == MessageDialogResult.Affirmative)
                        {
                            _message += $"_Password:_ {settingsModel.Password}";
                        }
                        else
                        {
                            _message += $"_Password protection:_ {_passwordprotected}\n";
                        }
                    }
                    await AnnounceRaceToDiscord(_message);
                }
                else
                {
                    Log.Information("Discord announcement canceled");
                }
                if (!settingsModel.Serverreachable && settingsModel.DiscordracenotificationEnabled)
                {
                    Log.Information("No Discord message sent - server is LAN-only");
                }
            }
        }

        private void ConsoleOutputHandler(object sendingProcess,
            DataReceivedEventArgs outLine)
        {
            Debug.WriteLine(outLine.Data);
            if(outLine.Data != null)
            {
                string _timestamp = $"[{DateTime.Now.ToString("HH:mm:ss")}]";
                if (outLine.Data.Contains("Boat count"))
                {
                    try
                    {
                        settingsModel.Boatsinrace = outLine.Data.Split(':')[2].Trim();
                    }
                    catch { }
                }
                if (outLine.Data.Contains("We now have"))
                {
                    try
                    {
                        // "[INFO] We now have 1 conns and 1 boats"
                        settingsModel.Boatsinrace = outLine.Data.Split(' ')[7].Trim();
                    }
                    catch { }
                }
                if (outLine.Data.Contains("RaceState"))
                {
                    try
                    {
                        settingsModel.Racestate = $"{outLine.Data.Split(':')[1].Split('(')[0].Trim()}";
                    }
                    catch { }
                }
                if (outLine.Data.Contains("New connection from") && outLine.Data.Contains("is_connected: true"))
                {
                    try
                    {
                        string _peer = outLine.Data.Split("peer")[1].Split(' ')[1].Split(':')[0].Trim();
                        Log.Information($"New connection from {_peer}");
                    }
                    catch { }
                }
                if (outLine.Data.Contains("Course changed to"))
                {
                    try
                    {
                        settingsModel.Activecourse = outLine.Data.Split(' ')[4].Trim();
                    }
                    catch { }
                }
                // [INFO] Spectator SGMConnectSpectator { protocol_version: 32, password: None, token: [20, 0, 0, 0, 104, 47, 8, 47, 216, 197, 223, 30, 17, 197, 223, 3, 1, 0, 16, 1, 151, 244, 52, 100, 24, 0, 0, 0, 1, 0, 0, 0, 2, 0, 0, 0, 193, 41, 251, 7, 234, 41, 36, 141, 159, 188, 235, 7, 13, 0, 0, 0, 178, 0, 0, 0, 50, 0, 0, 0, 4, 0, 0, 0, 17, 197, 223, 3, 1, 0, 16, 1, 116, 27, 22, 0, 238, 151, 151, 87, 57, 177, 168, 192, 0, 0, 0, 0, 220, 222, 51, 100, 92, 142, 79, 100, 1, 0, 242, 198, 7, 0, 0, 0, 0, 0, 222, 113, 114, 196, 249, 77, 50, 81, 177, 136, 18, 71, 56, 52, 5, 78, 65, 245, 50, 75, 37, 240, 207, 118, 175, 232, 106, 15, 34, 129, 189, 12, 167, 136, 251, 41, 192, 213, 46, 47, 143, 47, 234, 246, 138, 237, 38, 159, 171, 224, 160, 71, 154, 153, 36, 47, 124, 236, 255, 10, 188, 71, 254, 102, 17, 116, 80, 119, 100, 35, 72, 128, 163, 205, 140, 221, 193, 184, 174, 176, 173, 206, 29, 206, 176, 160, 146, 21, 84, 195, 211, 28, 249, 198, 106, 134, 179, 248, 153, 4, 253, 168, 1, 104, 50, 211, 211, 17, 177, 209, 89, 114, 235, 2, 157, 236, 163, 119, 229, 112, 5, 65, 47, 254, 112, 11, 130, 138], hash: 76561198025262353 }            connected
                if (outLine.Data.Contains("SGMConnectSpectator") && outLine.Data.Contains("connected"))
                {
                    try
                    {
                        Log.Information($"New spectator connected");
                    }
                    catch { }
                }
                // 16:11:23 [INFO] ServerLogic: ServerGameMessage::Connect received: SGMConnect { protocol_version: 32, password: None, boat_name: "elpatron", boat_model: "jx50", nation: "GER", skin: Skin("red white")
                if (outLine.Data.Contains("ServerGameMessage") && outLine.Data.Contains("Connect received"))
                {
                    try
                    {
                        string _boatname = outLine.Data.Split("boat_name:")[1].Split(',')[0].Replace("\"", "").Trim();
                        string _nation = outLine.Data.Split("nation:")[1].Split(',')[0].Replace("\"", "").Trim();
                        Log.Information($"{_boatname} from {_nation} joined the race");
                    }
                    catch { }
                }
                // [WARN] Umpire::on_timeline_crossed
                if (outLine.Data.Contains("Umpire::"))
                {
                    try
                    {
                        Log.Information($"Umpire: {outLine.Data.Split("::")[1].Trim()}");
                    }
                    catch { }
                }
                // 16:36:50 [INFO] Removing SrvBoat: BoatDef { boat_name: "elpatron", boat_model: "jx50", skin: Skin("red white"
                if (outLine.Data.Contains("Removing SrvBoat:"))
                {
                    try
                    {
                        Log.Information($"Boat left server: {outLine.Data.Split("boat_name:")[1]
                            .Split(',')[0]
                            .Replace("\"", "").Trim()}");
                    }
                    catch { }
                }
                // 09:11:16[INFO] CHAT: ChatMessage { origin: Boat(0), message: "Hallo Welt" }
                if (outLine.Data.Contains("CHAT:"))
                {
                    try
                    {
                        string _message = outLine.Data.Split("message:")[1].Split("\"")[1].Trim();
                        settingsModel.Chat = $"{_timestamp} {_message}\n{settingsModel.Chat}"; // _timestamp + _message + settingsModel.Chat;
                    }
                    catch { }
                }

            }
        }

        private void BtnStartServer_Click(object sender, RoutedEventArgs e)
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
                        settingsModel.Boatsinrace = "0";
                        settingsModel.Activecourse = "n/a";
                        settingsModel.Racestate = "Inactive";
                        Log.Information("HG server stopped");
                        
                        //ToggleControls(true);
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

        private void MnSave_Click(object sender, RoutedEventArgs e)
        {
            _cfgFileSystemWatcher.EnableRaisingEvents = false;
            SendTab();
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
                SendTab();
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
            Thread.Sleep(2000);
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

            if (resultTcpClient.Connected)
            {
                settingsModel.Serverreachable = true;
                Log.Information("Server is accessible to the public");
            }
            else
            {
                settingsModel.Serverreachable = false;
                Log.Information("External port check failed: LAN-only server");
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
                SettingsFile.ReadConfigfile(settingsModel, $@"{settingsModel.Configfiledirectory}\slot{slotnumber.ToString()}.kl");
                RunServerProcess();
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
            MessageDialogResult result = await this.ShowMessageAsync("Are you sure?", 
                $"Delete all svg files in \"{settingsModel.Snapsdirectory}\"?", MessageDialogStyle.AffirmativeAndNegative);
            if (result == MessageDialogResult.Affirmative)
            {
                Log.Information("Cleaning up snaps directory");
                int _count = 0;
                foreach (string _filename in Directory.GetFiles(settingsModel.Snapsdirectory, "*.svg"))
                {
                    try
                    {
                        File.Delete(_filename);
                        _count += 1;
                    }
                    catch (Exception ex)
                    {
                        Log.Warning($"Failed to delete {_filename}: {ex.Message}");
                    }
                }
                Log.Information($"{_count} files deleted.");
            }
        }

        private void SendTab()
        {
            KeyEventArgs _tab = new KeyEventArgs(Keyboard.PrimaryDevice,
                Keyboard.PrimaryDevice.ActiveSource, 0, Key.Tab);
            _tab.RoutedEvent = Keyboard.KeyDownEvent;
            InputManager.Current.ProcessInput(_tab);
        }

        private void ToggleControls(bool enabled)
        {
            List<Control> _controls = new();
            _controls.AddRange(new Control[] { TxServerName,
                TxPortTcp,
                TxPortUdp,
                TxPortSteam,
                CbBoat,
                CbLocation,
                NmMaxClients,
                NmMinPlayers,
                TxPassword,
                TxAdminPassword,
                NmMaxRaceTime,
                NmMaxSpectators,
                NmSessiontimePrestart,
                NmSessiontimePostrace,
                NmSessiontimeSetup,
                NmWindMinSpeed,
                NmWindMaxSpeed,
                NmWindHeading,
                NmWindEvolutionGain,
                NmOcsDragGain,
                NmBoundaryDrag,
                NmPenaltyDragGain,
                CheckUseCollisions,
                NmWindShadowScale,
                NmGapToClear,
                NmClientSlowdown,
                NmPenaltyDuration,
                NmBlackFlagDuration,
                NmBlackFlagLegs,
            });

            foreach (Control c in _controls)
            {

                c.IsEnabled = enabled;
            }
        }

        //private void LbCourse_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    Clipboard.SetText($"/set_course {CbCourse.Text}");
        //    Log.Information($"Cmd '/set_course {CbCourse.Text}' copied to clipboard");
        //}

        //private void LbWindMax_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    Clipboard.SetText($"/set_wind {NmWindMaxSpeed.Value}");
        //    Log.Information($"Cmd '/set_wind {NmWindMaxSpeed.Value}' copied to clipboard");
        //}

        //private void LbAdminPassword_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        //{
        //    Clipboard.SetText($"/admin {settingsModel.Adminpassword}");
        //    Log.Information($"Cmd '/admin {settingsModel.Adminpassword}' copied to clipboard");
        //}

        private void RtbLogMessages_TextChanged(object sender, TextChangedEventArgs e)
        {
            RtbLogMessages.ScrollToEnd();
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _ = Process.Start(new ProcessStartInfo("https://github.com/elpatron68/HG-Server-Manager") { UseShellExecute = true });
        }

        public async Task AnnounceRaceToDiscord(string _message)
        {
                Log.Information("Sending discord announcement");
                var client = new DiscordSocketClient();
                await client.LoginAsync(TokenType.Bot, _discordRacebot.DiscordbotToken);
                await client.StartAsync();
                var channel = await client.GetChannelAsync(_discordRacebot.Discordchannelid) as IMessageChannel;
                await channel!.SendMessageAsync(_message);
        }

        private void MnResults_Click(object sender, RoutedEventArgs e)
        {
            ResultsView rv = new(settingsModel);
            rv.Show();
        }

        private async void MnArchive_Click(object sender, RoutedEventArgs e)
        {
            // Copy all results (*.kl and *.json) to a Zip archive
            string _startPath = Directory.GetParent(settingsModel.Resultsdirectory).ToString();
            string _zipPath = Path.GetTempPath();
            string _zipFile = _zipPath + $"results_archive_{DateTime.Now:yy-MM-dd HH-mm-ss}.zip";
            string _archiveDirectory = Path.GetDirectoryName(settingsModel.Exepath) + @"\archive\";
            int _filescount=0 ;

            if (Directory.GetFiles(_startPath).Length > 0)
            {
                MessageDialogResult _result = await this.ShowMessageAsync("Are you sure?",
                    $"Do you want to archive and delete all regatta results in '{_startPath}' and start a new series?\n\n" +
                    $"All files will be archived into a zip file in '{_archiveDirectory}' and can " +
                    $"be restored by extracting the archive to it´s original location.", MessageDialogStyle.AffirmativeAndNegative);
                if (_result == MessageDialogResult.Affirmative)
                {
                    ZipFile.CreateFromDirectory(_startPath, _zipFile);
                    // Move Zip file to archive directory in AppContext.BaseDirectory
                    try
                    {
                        if (!Directory.Exists(_archiveDirectory))
                        {
                            Directory.CreateDirectory(_archiveDirectory);
                        }
                        File.Move(_zipFile, Path.Combine(_archiveDirectory, Path.GetFileName(_zipFile)));                            
                    }
                    catch(Exception ex)
                    { 
                        Log.Warning($"Failed to move {_zipFile}: {ex.Message}"); 
                    }

                    // Cleanup results directory
                    // Delete *.kl files
                    string[] _klfiles = Directory.GetFiles(_startPath);
                    foreach (string _klfile in _klfiles)
                    {
                        try
                        {
                            File.Delete(_klfile);
                            _filescount++;
                        }
                        catch (Exception ex)
                        {
                            Log.Warning($"Failed to delete {_klfile}: {ex.Message}");
                        }
                    }
                    // Delete *.json files
                    string[] _jsonfiles = Directory.GetFiles(settingsModel.Resultsdirectory);
                    foreach (string _jsonfile in _jsonfiles)
                    {
                        try
                        {
                            File.Delete(_jsonfile);
                            _filescount++;
                        }
                        catch (Exception ex)
                        {
                            Log.Warning($"Failed to delete {_jsonfile}: {ex.Message}"); }
                        }
                    Log.Information($"{_filescount} files moved to archive.");
                }
                else
                {
                    Log.Information($"Deleting result files cancelled by user.");
                }
            }
            else
            {
                Log.Information($"No result files found.");
            }
        }
    }
}
