using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Models;
using Dogey.Tools;
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
            ulong? guildId = (msg.Channel as IGuildChannel)?.Guild.Id;

            string prefix;
            if (guildId != null)
            {
                using (var db = new DataContext())
                {
                    var settings = db.Settings.Where(x => x.GuildId == guildId).FirstOrDefault();
                    prefix = settings.Prefix;
                }
            }
            else
            {
                prefix = Globals.Config.DefaultPrefix;
            }
            
            string message = $"Hello! I'm {self} :auxHappy:. You can get information about my commands by using `{prefix}help`. " +
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
                $" Owner(s): Auxesis#8522 (158056840402436096)",
                $"  Library: Discord.Net ({DiscordConfig.Version})",
                $"  Runtime: {RuntimeInformation.FrameworkDescription} {RuntimeInformation.OSArchitecture}",
                $"   Uptime: {(DateTime.Now - Process.GetCurrentProcess().StartTime)}",
                $"Heap Size: {Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString()}",
                $"  Latency: {_client.Latency}ms",
                $"   Guilds: {(await _client.GetGuildSummariesAsync()).Count()}",
                $" Channels: {(await _client.GetGuildsAsync()).Select(async g => await g.GetChannelsAsync()).Count()}",
                $"    Users: {(await _client.GetGuildsAsync()).Select(async g => await g.GetUsersAsync()).Count()}",
                "```"
            };

            await Utility.SendMessage(msg, string.Join("\n", infomsg));
        }

        [Module("botinfo"), Name("Info")]
        public class SubCommands
        {
            private DiscordSocketClient _client;

            public SubCommands(DiscordSocketClient client)
            {
                _client = client;
            }

            [Command("owner")]
            [Alias("own", "o")]
            [Description("Get Dogey's owner.")]
            public async Task Owner(IUserMessage msg)
            {
                await Utility.SendMessage(msg, $"My owner is **Auxesis#8522** (158056840402436096).");
            }

            [Command("library")]
            [Alias("lib", "l")]
            [Description("Get the library Dogey is currently running on.")]
            public async Task Library(IUserMessage msg)
            {
                await Utility.SendMessage(msg, $"I am currently running Discord.Net ({DiscordConfig.Version})");
            }

            [Command("runtime")]
            [Alias("run", "r")]
            [Description("Get the runtime Dogey is currently using.")]
            public async Task Runtime(IUserMessage msg)
            {
                await Utility.SendMessage(msg, $"I am currently running {RuntimeInformation.FrameworkDescription} {RuntimeInformation.OSArchitecture}.");
            }

            [Command("uptime")]
            [Alias("online", "up", "u")]
            [Description("Get how long Dogey has been online.")]
            public async Task Uptime(IUserMessage msg)
            {
                await Utility.SendMessage(msg, (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString());
            }

            [Command("heap")]
            [Alias("memory", "mem", "m")]
            [Description("Get how much memory Dogey is currently using.")]
            public async Task Heap(IUserMessage msg)
            {
                await Utility.SendMessage(msg, Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString());
            }

            [Command("latency")]
            [Alias("lag", "ping", "p")]
            [Description("Get Dogey's current ping.")]
            public async Task Latency(IUserMessage msg)
            {
                await Utility.SendMessage(msg, $"{_client.Latency}ms");
            }

            [Command("guilds")]
            [Alias("channels", "users")]
            [Description("Get info about the guilds Dogey is connected to.")]
            public async Task Guilds(IUserMessage msg)
            {
                await Task.Delay(1);
            }
        }
    }
}
