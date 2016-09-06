using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Dogey.Modules.InfoModule
{
    [Module, Name("Info")]
    [RequireContext(ContextType.Guild)]
    public class BotGroup
    {
        private DiscordSocketClient _client;

        public BotGroup(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("info")]
        [Description("Get information about Dogey.")]
        public async Task Info(IUserMessage msg)
        {
            var self = await _client.GetCurrentUserAsync();
            var channel = msg.Channel as ITextChannel;

            string prefix;
            if (channel?.Guild != null)
            {
                using (var db = new DataContext())
                {
                    var settings = db.Settings.Where(x => x.GuildId == channel.Guild.Id).FirstOrDefault();
                    prefix = settings.Prefix;
                }
            }
            else
            {
                prefix = Globals.Config.DefaultPrefix;
            }
            
            string message = $"Hello! I'm {self}.\nYou can get information about my commands by using `{prefix}help`. " +
                "To report bugs or request features you can join my guild here: https://discord.gg/0sjlWZiGRvRNZAqx";

            await msg.Channel.SendMessageAsync(message);
        }

        [Command("botinfo")]
        [Description("Get information about Dogey.")]
        public async Task Botinfo(IUserMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;
            
            var infomsg = new List<string>
            {
                "```xl",
                $" Owner(s): Auxesis#8522",
                $"  Library: Discord.Net ({DiscordConfig.Version})",
                $"  Runtime: {RuntimeInformation.FrameworkDescription} {RuntimeInformation.OSArchitecture}",
                $"   Uptime: {(DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"d'd' hh'h' m'm' s's'")}",
                $"Heap Size: {Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2)}",
                $"   Guilds: {(await _client.GetGuildSummariesAsync()).Count()}",
                $" Channels: {(await _client.GetGuildsAsync()).Select(async g => await g.GetChannelsAsync()).Count()}",
                $"    Users: {(await _client.GetGuildsAsync()).Select(async g => await g.GetUsersAsync()).Count()}",
                "```"
            };

            if (Globals.Config.IsSelfbot)
                await msg.ModifyAsync((e) => e.Content = string.Join("\n", infomsg));
            else
                await msg.Channel.SendMessageAsync(string.Join("\n", infomsg));
        }

        [Module("botinfo"), Name("Info")]
        public class SubCommands
        {

        }
    }
}
