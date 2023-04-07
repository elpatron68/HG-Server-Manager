using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Controls;

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
            try
            {
                process.Start();
            }
            catch 
            {
                return "n/a";
            }
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();
            return output;
        }

        public static bool Testport(string host, int port, TimeSpan timeout) 
        {
            try
            {
                using (var client = new TcpClient())
                {
                    var result = client.BeginConnect(host, port, null, null);
                    var success = result.AsyncWaitHandle.WaitOne(timeout);
                    client.EndConnect(result);
                    return success;
                }
            }
            catch
            {
                return false;
            }
        }

    }
}
