using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HG_ServerUI
{
    internal class DiscoverPath
    {
        /// <summary>
        /// Hydrofoil server directory
        /// </summary>
        /// <returns></returns>
        public static string FindGameDirectoryFromSteam()
        {
            // Computer\HKEY_CURRENT_USER\Software\Valve\Steam\Apps\1448820
            RegistryKey? key;
            key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam");
            try
            {
                string? _steamDirectory = (String?)key?.GetValue("SteamPath");
                string? _library = _steamDirectory?.Replace("/", @"\") + @"\steamapps\libraryfolders.vdf";

                string readText = File.ReadAllText(_library);
                string _sectionpath = "";
                foreach(string line in readText.Split("\n"))
                {
                    if (line.Contains("\"path\""))
                    {
                        var _splitted=line.Split('\"');
                        _sectionpath = _splitted[3].Replace(@"\\", @"\");
                    }
                    if (line.Contains("1448820"))
                    {
                        return _sectionpath + @"\steamapps\common\Hydrofoil Generation\server\hg_server.exe";
                    }
                }

            }
            catch
            {
                // Registry key not found
            }
            return "";
        }
    }
}
