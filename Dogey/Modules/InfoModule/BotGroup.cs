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
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;
            
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
                $"    Users: {(await _client.GetGuildsAsync()).Sum(g => g.GetUsers().Count())}",
                "```"
            };

            await Utility.SendMessage(msg, string.Join("\n", infomsg));
        }
    }
}
