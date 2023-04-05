using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        private float _windminspeed;
        public float Windminspeed
        {
            get { return _windminspeed; }
            set { _windminspeed = value; OnPropertyChanged(); }
        }

        private float _windmaxspeed;
        public float Windmaxspeed
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

        private float _windevolutiongain;
        public float Windevolutiongain
        {
            get { return _windevolutiongain; }
            set { _windevolutiongain = value; OnPropertyChanged(); }
        }

        private float _ocsdraggain;
        public float Ocsdraggain
        {
            get { return _ocsdraggain; }
            set { _ocsdraggain = value; OnPropertyChanged(); }
        }

        private float _boundarydrag;
        public float Boundarydrag
        {
            get { return _boundarydrag; }
            set { _boundarydrag = value; OnPropertyChanged(); }
        }

        private float _penaltydraggain;
        public float Penaltydraggain
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

        private float _windshadowscale;
        public float Windshadowscale
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


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
