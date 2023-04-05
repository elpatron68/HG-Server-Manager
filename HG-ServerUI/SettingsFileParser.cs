using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace HG_ServerUI
{
    public class SettingsFileParser
    {
        public static SettingsModel Readfile() 
        {
            SettingsModel model = new SettingsModel();
            string? _startdirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            string _filename = System.IO.Path.Combine(Directory.GetParent(_startdirectory).ToString(), @"server\cfg\server_cfg.kl");
#if DEBUG
            _filename = @"E:\Games\Steam\steamapps\common\Hydrofoil Generation\server\cfg\server_cfg.kl";
#endif
            string readText = File.ReadAllText(_filename);

            foreach (string line in readText.Split())
            {
                if (line.Contains("name="))
                {
                    model.Servername = line.Split("=")[1].Trim();
                }
                if (line.Contains("tcp="))
                {
                    model.Tcpport = line.Split("=")[1].Trim();
                }
                if (line.Contains("udp="))
                {
                    model.Udpport = line.Split("=")[1].Trim();
                }
                if (line.Contains("steam="))
                {
                    model.Steamport = line.Split("=")[1].Trim();
                }
                if (line.Contains("boat="))
                {
                    model.Boat = line.Split("=")[1].Trim();
                }
                if (line.Contains("location="))
                {
                    model.Location = line.Split("=")[1].Trim();
                }
                if (line.Contains("course="))
                {
                    model.Course = line.Split("=")[1].Trim();
                }
                if (line.Contains("max_clients="))
                {
                    model.Maxclients = int.Parse(line.Split("=")[1].Trim());
                }
                if (line.Contains("max_spectators="))
                {
                    model.Maxspectators = int.Parse(line.Split("=")[1].Trim());
                }
                if (line.Contains("minimum_player_count="))
                {
                    model.Minplayers = int.Parse(line.Split("=")[1].Trim());
                }
                if (line.Contains("password="))
                {
                    model.Password = line.Split("=")[1].Trim();
                }
                if (line.Contains("admin_password="))
                {
                    model.Adminpassword = line.Split("=")[1].Trim();
                }
                if (line.Contains("max_race_time_minutes="))
                {
                    model.Maxracetime = int.Parse(line.Split("=")[1].Trim());
                }
                if (line.Contains("setup="))
                {
                    model.Sessiontimesetup = int.Parse(line.Split("=")[1].Trim());
                }
                if (line.Contains("pre_start="))
                {
                    model.Sessiontimeprestart = int.Parse(line.Split("=")[1].Trim());
                }
                if (line.Contains("post_race="))
                {
                    model.Sessiontimepostrace = int.Parse(line.Split("=")[1].Trim());
                }
                if (line.Contains("min_speed="))
                {
                    model.Windminspeed = float.Parse(line.Split("=")[1].Trim(), CultureInfo.InvariantCulture.NumberFormat);
                }
                if (line.Contains("max_speed="))
                {
                    model.Windmaxspeed = float.Parse(line.Split("=")[1].Trim(), CultureInfo.InvariantCulture.NumberFormat);
                }
                if (line.Contains("hdg_variation="))
                {
                    model.Windminspeed = int.Parse(line.Split("=")[1].Trim().Split(",")[0]);
                    model.Windvariation = int.Parse(line.Split("=")[1].Trim().Split(",")[1]);
                }
                if (line.Contains("evolution_gain="))
                {
                    model.Windevolutiongain = float.Parse(line.Split("=")[1].Trim(), CultureInfo.InvariantCulture.NumberFormat);
                }
                if (line.Contains("ocs_drag_gain="))
                {
                    model.Ocsdraggain = float.Parse(line.Split("=")[1].Trim(), CultureInfo.InvariantCulture.NumberFormat);
                }
                if (line.Contains("boundary_drag="))
                {
                    model.Boundarydrag = float.Parse(line.Split("=")[1].Trim(), CultureInfo.InvariantCulture.NumberFormat);
                }
                if (line.Contains("penalty_drag_gain="))
                {
                    model.Penaltydraggain = float.Parse(line.Split("=")[1].Trim(), CultureInfo.InvariantCulture.NumberFormat);
                }
                if (line.Contains("use_collisions="))
                {
                    if (line.Contains("false")) 
                    { 
                        model.Usecollisions=false; 
                    }
                    else
                    {
                        model.Usecollisions = true;
                    }
                }
                if (line.Contains("wind_shadows_scale="))
                {
                    model.Windshadowscale = float.Parse(line.Split("=")[1].Trim(), CultureInfo.InvariantCulture.NumberFormat);
                }
                if (line.Contains("gap_to_clear="))
                {
                    model.Gaptoclear = int.Parse(line.Split("=")[1].Trim());
                }
                if (line.Contains("client_slowdown="))
                {
                    model.Clientslowdown = int.Parse(line.Split("=")[1].Trim());
                }
                if (line.Contains("penalty_duration="))
                {
                    model.Penaltyduration = int.Parse(line.Split("=")[1].Trim());
                }
                if (line.Contains("time="))
                {
                    model.Blackflagduration = int.Parse(line.Split("=")[1].Trim());
                }
                if (line.Contains("legs="))
                {
                    model.Blackflaglegs = int.Parse(line.Split("=")[1].Trim());
                }
            }
            return model; 
        }
    }
}
