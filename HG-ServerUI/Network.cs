using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HG_ServerUI
{
    internal class Network
    {
        public static string GetExternalIpaddress()
        {
            string _result = string.Empty;
            Process process = new();
            process.StartInfo.FileName = "curl.exe";
            process.StartInfo.Arguments = "http://ipinfo.io/ip";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            return output;
        }


    }
}
