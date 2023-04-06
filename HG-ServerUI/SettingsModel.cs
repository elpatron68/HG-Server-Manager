using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HG_ServerUI
{
    public class SettingsModel : INotifyPropertyChanged
    {
        private string _exepath;
        public string Exepath
        {
            get { return _exepath; }
            set { _exepath = value; OnPropertyChanged(); }
        }

        private bool _serverprocessrunning = false;
        public bool Serverprocessrunning
        {
            get { return _serverprocessrunning; }
            set { _serverprocessrunning = value; OnPropertyChanged(); }
        }

        private string _exepathtext;
        public string Exepathtext
        {
            get { return _exepathtext; }
            set { _exepathtext = value; OnPropertyChanged(); } 
        }

        private string _configfilepath;
        public string Configfilepath
        {
            get { return _configfilepath; }
            set { _configfilepath = value; OnPropertyChanged(); }
        }

        private string[] _boats;
        public string[] Boats
        {
            get { return _boats; }
            set { _boats = value; OnPropertyChanged(); }
        }

        private string[] _courses;
        public string[] Courses
        {
            get { return _courses; }
            set { _courses = value; OnPropertyChanged(); }
        }

        private string[] _locations;
        public string[] Locations
        {
            get { return _locations; }
            set { _locations = value; OnPropertyChanged(); }
        }

        private string _servername;
        public string Servername
        {
            get { return _servername; }
            set { _servername = value; OnPropertyChanged(); }
        }

        private string _tcpport;
        public string Tcpport
        {
            get { return _tcpport; }
            set { _tcpport = value; OnPropertyChanged(); }
        }

        private string _udpport;
        public string Udpport
        {
            get { return _udpport; }
            set { _udpport = value; OnPropertyChanged(); }
        }

        private string _steamport;
        public string Steamport
        {
            get { return _steamport; }
            set { _steamport = value; OnPropertyChanged(); }
        }

        private string _boat;
        public string Boat
        {
            get { return _boat; }
            set { _boat = value; OnPropertyChanged(); }
        }

        private string _location;
        public string Location
        {
            get { return _location; }
            set { _location = value; OnPropertyChanged(); }
        }

        private string _course;
        public string Course
        {
            get { return _course; }
            set { _course = value; OnPropertyChanged(); }
        }

        private int _maxclients;
        public int Maxclients
        {
            get { return _maxclients; }
            set { _maxclients = value; OnPropertyChanged(); }
        }

        private int _minplayers;
        public int Minplayers
        {
            get { return _minplayers; }
            set { _minplayers = value; OnPropertyChanged(); }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set { _password = value; OnPropertyChanged(); }
        }

        private string _adminpassword;
        public string Adminpassword
        {
            get { return _adminpassword; }
            set { _adminpassword = value; OnPropertyChanged(); }
        }

        private int _maxracetime;
        public int Maxracetime
        {
            get { return _maxracetime; }
            set { _maxracetime = value; OnPropertyChanged(); }
        }

        private int _sessiontimesetup;
        public int Sessiontimesetup
        {
            get { return _sessiontimesetup; }
            set { _sessiontimesetup = value; OnPropertyChanged(); }
        }

        private int _sessiontimeprestart;
        public int Sessiontimeprestart
        {
            get { return _sessiontimeprestart; }
            set { _sessiontimeprestart = value; OnPropertyChanged(); }
        }

        private int _sessiontimepostrace;
        public int Sessiontimepostrace
        {
            get { return _sessiontimepostrace; }
            set { _sessiontimepostrace = value; OnPropertyChanged(); }
        }

        private double _windminspeed;
        public double Windminspeed
        {
            get { return _windminspeed; }
            set { _windminspeed = value; OnPropertyChanged(); }
        }

        private double _windmaxspeed;
        public double Windmaxspeed
        {
            get { return _windmaxspeed; }
            set { _windmaxspeed = value; OnPropertyChanged(); }
        }

        private int _windheading;
        public int Windheading
        {
            get { return _windheading; }
            set { _windheading = value; OnPropertyChanged(); }
        }

        private int _windvariation;
        public int Windvariation
        {
            get { return _windvariation; }
            set { _windvariation = value; OnPropertyChanged(); }
        }

        private double _windevolutiongain;
        public double Windevolutiongain
        {
            get { return _windevolutiongain; }
            set { _windevolutiongain = value; OnPropertyChanged(); }
        }

        private double _ocsdraggain;
        public double Ocsdraggain
        {
            get { return _ocsdraggain; }
            set { _ocsdraggain = value; OnPropertyChanged(); }
        }

        private double _boundarydrag;
        public double Boundarydrag
        {
            get { return _boundarydrag; }
            set { _boundarydrag = value; OnPropertyChanged(); }
        }

        private double _penaltydraggain;
        public double Penaltydraggain
        {
            get { return _penaltydraggain; }
            set { _penaltydraggain = value; OnPropertyChanged(); }
        }

        private bool _usecollisions;
        public bool Usecollisions
        {
            get { return _usecollisions; }
            set { _usecollisions = value; OnPropertyChanged(); }
        }

        private double _windshadowscale;
        public double Windshadowscale
        {
            get { return _windshadowscale; }
            set { _windshadowscale = value; OnPropertyChanged(); }
        }

        private int _gaptoclear;
        public int Gaptoclear
        {
            get { return _gaptoclear; }
            set { _gaptoclear = value; OnPropertyChanged(); }
        }

        private int _clientslowdown;
        public int Clientslowdown
        {
            get { return _clientslowdown; }
            set { _clientslowdown = value; OnPropertyChanged(); }
        }

        private int _penaltyduration;
        public int Penaltyduration
        {
            get { return _penaltyduration; }
            set { _penaltyduration = value; OnPropertyChanged(); }
        }

        private int _blackflagduration;
        public int Blackflagduration
        {
            get { return _blackflagduration; }
            set { _blackflagduration = value; OnPropertyChanged(); }
        }

        private int _blackflaglegs;
        public int Blackflaglegs
        {
            get { return _blackflaglegs; }
            set { _blackflaglegs = value; OnPropertyChanged(); }
        }

        private int _maxspectators;
        public int Maxspectators
        {
            get { return _maxspectators; }
            set { _maxspectators = value; OnPropertyChanged(); }
        }

        private string _externalip;
        public string Externalip
        {
            get { return _externalip; }
            set { _externalip = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Constructor: Set default values
        /// </summary>
        public SettingsModel()
        {
            Servername = "FlyHighOrDie";
            Tcpport = "3333";
            Udpport = "3334";
            Steamport = "27015";
            Boat = "jx50";
            Location = "denhaag";
            Course = "reach_start_1400";
            Maxclients = 8;
            Maxspectators = 6;
            Minplayers = 2;
            Password = "";
            Adminpassword = "";
            Maxracetime = 20;
            Sessiontimesetup = 30;
            Sessiontimeprestart = 40;
            Sessiontimepostrace = 120;
            Windminspeed = 13.5;
            Windmaxspeed = 16;
            Windheading = 20;
            Windvariation = 5;
            Windevolutiongain = .1;
            Ocsdraggain = .15;
            Boundarydrag = 1;
            Penaltydraggain = .15;
            Usecollisions = false;
            Windshadowscale = 1;
            Gaptoclear = 30;
            Clientslowdown = 0;
            Penaltyduration = 60;
            Blackflagduration = 60;
            Blackflaglegs = 1;
            Externalip = "";
        }

        private static string GetCfgFilenameFromExepath(string _exepath)
        {
            return Path.GetDirectoryName(_exepath) + @"\cfg\server_cfg.kl";
        }

        public static SettingsModel AddPaths(SettingsModel model)
        {
            int _count = 0;
            model.Exepath = DiscoverPath.FindGameDirectoryFromSteam();
            model.Configfilepath = GetCfgFilenameFromExepath(model.Exepath);
            string _contentdirectory= Path.GetDirectoryName(model.Exepath) + @"\content";
            model.Boats = Directory.GetDirectories(_contentdirectory + @"\boats");
            _count = 0;
            foreach (var item in model.Boats)
            {
                model.Boats[_count] = item.Split(@"\").Last();
                _count++;
            }
            model.Courses = Directory.GetFiles(_contentdirectory + @"\courses","*.kl");

            _count = 0;
            foreach(var item in model.Courses)
            {
                model.Courses[_count] = item.Split(".")[0].Split(@"\").Last();
                _count++;
            }
            model.Locations= Directory.GetDirectories(_contentdirectory + @"\locations");
            _count = 0;
            foreach(var item in model.Locations)
            {
                model.Locations[_count] = item.Split(@"\").Last();
                _count++;
            }
            model.Exepathtext = model.Exepath.Replace(@"\steamapps\common\", @"\[...]\");
            model.Externalip = Network.GetExternalIpaddress();
            return model;
        }
    }
}
