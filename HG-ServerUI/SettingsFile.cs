using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace HG_ServerUI
{
    public class SettingsFile
    {
        public static SettingsModel ReadConfigfile(SettingsModel model, string filename = "") 
        {
            if (filename == "")
            {
                filename = model.Configfilepath;
            }

            if(File.Exists(filename))
            {
                string readText = File.ReadAllText(filename);
                
                // Set default Ntfy topics
                if (!readText.Contains("ntfyracetopic="))
                {
                    readText= "# ntfyracetopic=Hydrofoil_Generation_Servermonitor\n" + readText;
                }
                if (!readText.Contains("ntfypenaltytopic="))
                {
                    readText = "# ntfypenaltytopic=Hydrofoil_Generation_Penaltymonitor\n" + readText;
                }

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
                        model.Windminspeed = float.Parse(line.Split("=")[1].Trim(),
                            CultureInfo.InvariantCulture.NumberFormat);
                    }
                    if (line.Contains("max_speed="))
                    {
                        model.Windmaxspeed = float.Parse(line.Split("=")[1].Trim(),
                            CultureInfo.InvariantCulture.NumberFormat);
                    }
                    if (line.Contains("hdg_variation="))
                    {
                        model.Windheading = int.Parse(line.Split("=")[1].Trim().Split(",")[0]);
                        model.Windvariation = int.Parse(line.Split("=")[1].Trim().Split(",")[1]);
                    }
                    if (line.Contains("evolution_gain="))
                    {
                        model.Windevolutiongain = float.Parse(line.Split("=")[1].Trim(),
                            CultureInfo.InvariantCulture.NumberFormat);
                    }
                    if (line.Contains("ocs_drag_gain="))
                    {
                        model.Ocsdraggain = float.Parse(line.Split("=")[1].Trim(),
                            CultureInfo.InvariantCulture.NumberFormat);
                    }
                    if (line.Contains("boundary_drag="))
                    {
                        model.Boundarydrag = float.Parse(line.Split("=")[1].Trim(),
                            CultureInfo.InvariantCulture.NumberFormat);
                    }
                    if (line.Contains("penalty_drag_gain="))
                    {
                        model.Penaltydraggain = float.Parse(line.Split("=")[1].Trim(),
                            CultureInfo.InvariantCulture.NumberFormat);
                    }
                    if (line.Contains("use_collisions="))
                    {
                        if (line.Contains("false"))
                        {
                            model.Usecollisions = false;
                        }
                        else
                        {
                            model.Usecollisions = true;
                        }
                    }
                    if (line.Contains("wind_shadows_scale="))
                    {
                        model.Windshadowscale = float.Parse(line.Split("=")[1].Trim(),
                            CultureInfo.InvariantCulture.NumberFormat);
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
                    if (line.Contains("ntfyracetopic="))
                    {
                        model.Ntfyracectopic = line.Split("=")[1].Trim();
                    }
                    //if (model.Ntfyracectopic == "")
                    //{
                    //    model.Ntfyracectopic = "Hydrofoil_Generation_Servermonitor";
                    //}
                    if (line.Contains("ntfypenaltytopic="))
                    {
                        model.Ntfypenaltytopic = line.Split("=")[1].Trim();
                    }
                    //if (model.Ntfypenaltytopic == "")
                    //{
                    //    model.Ntfypenaltytopic = "Hydrofoil_Generation_Penaltymonitor";
                    //}
                }
            }
            return model; 
        }
        public static void WriteConfigfile(SettingsModel model, string filename = "")
        {
            if (filename == "")
            {
                filename = model.Configfilepath;
            }
            string content = """
# Meta information from HG Server-Manager
# Should be harmless, may be deleted if any problems occur
# See https://github.com/elpatron68/HG-Server-Manager/tree/master#ntfy-notifications if you want to learn more about Ntfy
# ntfyracetopic=<ntfyracetopic>
# ntfypenaltytopic=<ntfypenaltytopic>

name=<servername>
ports
    tcp=<tcpport>
    udp=<udpport>
    steam=<steamport>

boat=<boat>
location=<location>
course=<course>
max_clients=<maxclients>
max_spectators=<maxspectators>
minimum_player_count=<minplayers>
sleep_time_ms=0
password=<password>
admin_password=<adminpassword>
max_race_time_minutes=<maxracetime>

session_times
    setup=<sessiontimesetup>
    pre_start=<sessiontimeprestart>
    post_race=<sessiontimepostrace>

wind
    min_speed=<windminspeed>
    max_speed=<windmaxspeed>
    hdg_variation=<windheading>,<windvariation>
    evolution_gain=<windevolutiongain>

rules
    ocs_drag_gain=<ocsdraggain>
    boundary_drag=<boundarydrag>
    penalty_drag_gain=<penaltydraggain>
    use_collisions=<usecollisions>
    wind_shadows_scale=<windshadowscale>

penalties
    start

    collisions
        gap_to_clear=<gaptoclear>
        client_slowdown=<clientslowdown>
        penalty_duration=<penaltyduration>
        black_flag_logic
            time=<blackflagduration>
            legs=<blackflaglegs>

""";
            content = content.Replace("<tcpport>", model.Tcpport)
                .Replace("<udpport>", model.Udpport)
                .Replace("<steamport>", model.Steamport)
                .Replace("<boat>", model.Boat)
                .Replace("<servername>", model.Servername)
                .Replace("<location>", model.Location)
                .Replace("<course>", model.Course)
                .Replace("<maxclients>", model.Maxclients.ToString())
                .Replace("<maxspectators>", model.Maxspectators.ToString())
                .Replace("<minplayers>", model.Minplayers.ToString())
                .Replace("<password>", model.Password)
                .Replace("<adminpassword>", model.Adminpassword)
                .Replace("<maxracetime>", model.Maxracetime.ToString())
                .Replace("<sessiontimesetup>", model.Sessiontimesetup.ToString())
                .Replace("<sessiontimeprestart>", model.Sessiontimeprestart.ToString())
                .Replace("<sessiontimepostrace>", model.Sessiontimepostrace.ToString())
                .Replace("<windminspeed>", model.Windminspeed.ToString("0.#", CultureInfo.InvariantCulture))
                .Replace("<windmaxspeed>", model.Windmaxspeed.ToString("0.#", CultureInfo.InvariantCulture))
                .Replace("<windheading>", model.Windheading.ToString())
                .Replace("<windvariation>", model.Windvariation.ToString())
                .Replace("<windevolutiongain>", model.Windevolutiongain.ToString("0.#", CultureInfo.InvariantCulture))
                .Replace("<ocsdraggain>", model.Ocsdraggain.ToString("0.##", CultureInfo.InvariantCulture))
                .Replace("<boundarydrag>", model.Boundarydrag.ToString("0.#", CultureInfo.InvariantCulture))
                .Replace("<penaltydraggain>", model.Penaltydraggain.ToString("0.##", CultureInfo.InvariantCulture))
                .Replace("<usecollisions>", model.Usecollisions.ToString().ToLower())
                .Replace("<windshadowscale>", model.Windshadowscale.ToString("0.#", CultureInfo.InvariantCulture))
                .Replace("<gaptoclear>", model.Gaptoclear.ToString())
                .Replace("<clientslowdown>", model.Clientslowdown.ToString())
                .Replace("<penaltyduration>", model.Penaltyduration.ToString())
                .Replace("<blackflagduration>", model.Blackflagduration.ToString())
                .Replace("<blackflaglegs>", model.Blackflaglegs.ToString())
                .Replace("<ntfyracetopic>", model.Ntfyracectopic)
                .Replace("<ntfypenaltytopic>", model.Ntfypenaltytopic)
                ;
            File.WriteAllText(filename, content);
        }
    }
}
