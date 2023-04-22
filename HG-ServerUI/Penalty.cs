using ControlzEx.Standard;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace HG_ServerUI
{
    public class Penalty
    {
        private string? _pngfilename;
        public string? PngFilename { get { return _pngfilename; } set { _pngfilename = value; } }
        private string? _pngfullpath;
        public string? PngFullpath { get { return _pngfullpath; } set { _pngfullpath = value; } }
        private string? _svgfullpath;
        public string? SvgFullpath { get { return _svgfullpath; } set { _svgfullpath = value; } }
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
            SvgFullpath=_filename;
            PngFullpath = Path.Combine(Path.GetDirectoryName(_filename), 
                Path.GetFileNameWithoutExtension(SvgFullpath) + ".png");
            PngFilename = Path.GetFileName(PngFullpath);
            try
            {
                Username = PngFilename.Split(' ')[0].Trim();
            }
            catch { }
            Timestamp = File.GetCreationTime(_filename);

            if(PngFilename.Contains(" on "))
            {
                try
                {
                    Competitor = PngFilename.Split(" on ")[1].Split(' ')[0].Trim();
                }
                catch { Competitor = ""; };
            }
            else
            {
                Competitor = "";
            }

            int _split = Regex.Matches(PngFilename, Regex.Escape("-")).Count;
            Rule = PngFilename.Split('-')[_split].Split('_')[0].Trim();
            if (Rule == "port")
            {
                Rule = "10 stbd-port";
            }
        }
    }
}
