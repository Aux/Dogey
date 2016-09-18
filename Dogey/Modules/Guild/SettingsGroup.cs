using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Attributes;
using Dogey.Enums;
using Dogey.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.GuildModule
{
    [Module, Name("Guild")]
    [MinPermissions(AccessLevel.ServerAdmin)]
    [RequireContext(ContextType.Guild)]
    public class SettingsGroup
    {
        private DiscordSocketClient _client;

        public SettingsGroup(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("setprefix")]
        [Description("Change the prefix for this guild.")]
        public async Task SetPrefix(IUserMessage msg, string prefix)
        {
            if (prefix.Count() > 10 && !string.IsNullOrWhiteSpace(prefix))
            {
                await msg.Channel.SendMessageAsync($"The command prefix cannot be greater than 10 characters or empty.");
                return;
            }

            var channel = (msg.Channel as IGuildChannel);
            using (var db = new DataContext())
            {
                var settings = db.Settings.Where(x => x.GuildId == channel.Guild.Id).FirstOrDefault();
                if (settings != null)
                {
                    settings.Prefix = prefix;
                    await db.SaveChangesAsync();
                }
            }
            await msg.Channel.SendMessageAsync($"The command prefix for this guild is now `{prefix}`.");
        }

        [Command("setmodlog")]
        [Description("Set the channel moderator actions are logged to.")]
        public async Task SetLogChannel(IUserMessage msg, ITextChannel channel)
        {
            using (var db = new DataContext())
            {
                var settings = db.Settings.Where(x => x.GuildId == channel.Guild.Id).FirstOrDefault();
                if (settings != null)
                {
                    settings.LogChannelId = channel.Id;
                    await db.SaveChangesAsync();
                }
            }
            await msg.Channel.SendMessageAsync($"The modlog for this guild is now {channel.Mention}.");
        }

        [Command("setstar")]
        [Description("Set the starred messages are logged to.")]
        public async Task SetStarChannel(IUserMessage msg, ITextChannel channel)
        {
            using (var db = new DataContext())
            {
                var settings = db.Settings.Where(x => x.GuildId == channel.Guild.Id).FirstOrDefault();
                if (settings != null)
                {
                    settings.StarChannelId = channel.Id;
                    await db.SaveChangesAsync();
                }
            }
            await msg.Channel.SendMessageAsync($"The modlog for this guild is now {channel.Mention}.");
        }
    }
}
