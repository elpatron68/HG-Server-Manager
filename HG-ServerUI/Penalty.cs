using ControlzEx.Standard;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace HG_ServerUI
{
    public class Penalty
    {
        private string? _filename;
        public string? Filename { get { return _filename; } set { _filename = value; } }
        private string? _fullpath;
        public string? Fullpath { get { return _fullpath; } set { _fullpath = value; } }
        private DateTime _timestamp;
        public DateTime Timestamp { get { return _timestamp; } set { _timestamp = value; } }
        private string? _username;
        public string? Username { get { return _username; } set { _username = value; } }
        private string? _competitor;
        public string? Competitor { get { return _competitor; } set { _competitor = value; } }
        private string? _rule;
        public string? Rule { get { return _rule; } set { _rule = value; } }

        public Penalty(string _filename)
        {
            Fullpath = _filename;
            Filename = Path.GetFileName(Fullpath);
            try
            {
                Username = Filename.Split(' ')[0].Trim();
            }
            catch { }
            Timestamp = File.GetCreationTime(_filename);

            if(Filename.Contains(" on "))
            {
                try
                {
                    Competitor = Filename.Split(" on ")[1].Split(' ')[0].Trim();
                }
                catch { Competitor = ""; };
            }
            else
            {
                Competitor = "";
            }

            int _split = Regex.Matches(Filename, Regex.Escape("-")).Count;
            Rule = Filename.Split('-')[_split].Split('_')[0].Trim();
            if (Rule == "port")
            {
                Rule = "10 stbd-port";
            }
        }
    }
}
