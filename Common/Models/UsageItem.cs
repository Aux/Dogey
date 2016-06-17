using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Common.Models
{
    public class UsageItem
    {
        /// <summary>The date and time this command was executed.</summary>
        public DateTime Timestamp { get; set; }
        /// <summary>The guild/server id this command was executed in.</summary>
        public ulong Guild { get; set; }
        /// <summary>The channel id this command was executed in.</summary>
        public ulong Channel { get; set; }
        /// <summary>The id of the user that executed this command.</summary>
        public ulong User { get; set; }
        /// <summary>The command text.</summary>
        public string Command { get; set; }

        public UsageItem(CommandEventArgs e)
        {
            Timestamp = DateTime.Now;
            Guild = e.Server.Id;
            Channel = e.Channel.Id;
            User = e.User.Id;
            Command = e.Command.Text;
        }
    }
}
