using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Extensions;
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
            var guild = (msg.Channel as IGuildChannel)?.Guild;

            string prefix;
            if (guild != null)
                prefix = await guild.GetCustomPrefixAsync();
            else
                prefix = Globals.Config.DefaultPrefix;
            
            string message = $"Hello! I'm {self} <:auxHappy:213686501089738752>. You can get information about my commands by using `{prefix}help`. " +
                "To report bugs or request features you can join my guild here: https://discord.gg/0sjlWZiGRvRNZAqx";

            await msg.Channel.SendMessageAsync(message);
        }

        [Command("botinfo")]
        [Description("Get information about Dogey.")]
        public async Task Botinfo(IUserMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild;
            
            var infomsg = new List<string>
            {
                "```xl",
                $" Owner(s): Auxesis#8522 (158056840402436096)",
                $"  Library: Discord.Net ({DiscordConfig.Version})",
                $"  Runtime: {RuntimeInformation.FrameworkDescription} {RuntimeInformation.OSArchitecture}",
                $"   Uptime: {(DateTime.Now - Process.GetCurrentProcess().StartTime)}",
                $"Heap Size: {Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2)} MB",
                $"  Latency: {_client.Latency} MS",
                $"   Guilds: {(await _client.GetGuildSummariesAsync()).Count()}",
                $" Channels: {(await _client.GetGuildsAsync()).Sum(g => g.GetChannels().Count())}",
                $"    Users: {(await _client.GetGuildsAsync()).Sum(g => g.GetUsers().Count())}"
            };
            
            using (var db = new DataContext())
            {
                infomsg.Add($" Messages: {db.MessageLogs.Count(x => x.GuildId == guild.Id)} of {db.MessageLogs.Count()}");
            }

            infomsg.Add("```");
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
            public async Task Owner(IUserMessage msg)
            {
                await Utility.SendMessage(msg, "Auxesis#8522 (158056840402436096)");
            }

            [Command("library")]
            [Alias("runtime", "lib")]
            public async Task Library(IUserMessage msg)
            {
                await Utility.SendMessage(msg, $"Discord.Net ({DiscordConfig.Version}) on {RuntimeInformation.FrameworkDescription} {RuntimeInformation.OSArchitecture}");
            }

            [Command("uptime")]
            [Alias("online", "up")]
            public async Task Uptime(IUserMessage msg)
            {
                await Utility.SendMessage(msg, (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString());
            }

            [Command("heap")]
            [Alias("memory", "mem")]
            public async Task Heap(IUserMessage msg)
            {
                await Utility.SendMessage(msg, $"{Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2)} MB");
            }

            [Command("latency")]
            [Alias("ping", "lag")]
            public async Task Latency(IUserMessage msg)
            {
                await Utility.SendMessage(msg, $"{_client.Latency} MS");
            }

            [Command("guilds")]
            [Alias("servers")]
            public async Task Guilds(IUserMessage msg)
            {
                await Task.Delay(1);
            }

            [Command("messages")]
            [Alias("msgs")]
            public async Task Messages(IUserMessage msg)
            {
                await Task.Delay(1);
            }

            [Command("commands")]
            [Alias("cmds")]
            public async Task Commands(IUserMessage msg)
            {
                await Task.Delay(1);
            }
        }
    }
}
