using Discord;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Models
{
    [Table("guildsettings")]
    public class GuildSettings
    {
        [Key, Required, Column("GuildId")]
        public ulong GuildId { get; set; }

        [Column("Prefix")]
        public string Prefix { get; set; }

        [Column("LogChannelId")]
        public ulong LogChannelId { get; set; }

        [Column("StarChannelId")]
        public ulong StarChannelId { get; set; }

        public GuildSettings() { }
        public GuildSettings(IMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild;
            GuildId = guild.Id;
            Prefix = "~";
        }
        
        public async Task<ITextChannel> LogChannel(IMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild;
            if (guild != null)
                return await guild.GetTextChannelAsync(LogChannelId);
            else
                return null;
        }

        public async Task<ITextChannel> StarChannel(IMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild;
            if (guild != null)
                return await guild.GetTextChannelAsync(StarChannelId);
            else
                return null;
        }
    }
}
