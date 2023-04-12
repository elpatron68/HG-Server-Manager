// Rename this file to "Discordbot.cs" before building the project!
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HG_ServerUI
{
    internal class Discordbot
    {
        private string _discordbottoken;
        public string DiscordbotToken
        {
            get { return _discordbottoken; }
            set { _discordbottoken = value; }
        }

        private ulong _discordchannelid;
        public ulong Discordchannelid
        {
            get { return _discordchannelid; }
            set { _discordchannelid = value; }
        }

        public Discordbot()
        {
            // Get token from Discord application portal: https://discord.com/developers/applications/
            DiscordbotToken = @"";
            // Get channel Id: Righ-click on channel => Copy channel Id
            Discordchannelid = 0;
        }
    }
}
