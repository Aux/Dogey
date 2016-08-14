using Discord;
using Discord.Commands;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.CSharp.RuntimeBinder;
using Discord.WebSocket;

namespace Dogey.Modules.Public
{
    [Module]
    public class PublicModule
    {
        private DiscordSocketClient _client;

        public PublicModule(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("join")]
        [Description("Returns the Invite URL of the bot.")]
        public async Task Join(IMessage msg)
        {
            var application = await _client.GetApplicationInfoAsync();
            await
                msg.Channel.SendMessageAsync(
                    $"A user with `MANAGE SERVER` can invite me to your server here: <https://discordapp.com/oauth2/authorize?client_id={application.Id}&scope=bot>");
        }
        
        [Command("leave")]
        [Description("Instructs the bot to leave this server")]
        public async Task Leave(IMessage msg)
        {
            var guild = (msg.Channel as IGuildChannel)?.Guild ?? null;
            if (guild == null) { await msg.Channel.SendMessageAsync("This command can only be ran in a server."); return; }
            await msg.Channel.SendMessageAsync("Leaving~");
            await guild.LeaveAsync();
        }

        [Command("say")]
        [Description("Echos the provided input.")]
        public async Task Say(IMessage msg, string text)
        {
            await msg.Channel.SendMessageAsync(msg.Content);
        }

        [Command("info")]
        public async Task Info(IMessage msg)
        {
            var application = await _client.GetApplicationInfoAsync();
            await msg.Channel.SendMessageAsync(
                $"{Format.Bold("Info")}\n" +
                $"- Author: {application.Owner.Username} (ID {application.Owner.Id})\n" +
                $"- Library: Discord.Net ({DiscordConfig.Version})\n" +
                $"- Runtime: {RuntimeInformation.FrameworkDescription} {RuntimeInformation.OSArchitecture}\n" +
                $"- Uptime: {GetUptime()}\n\n" +

                $"{Format.Bold("Stats")}\n" +
                $"- Heap Size: {GetHeapSize()} MB\n" +
                $"- Guilds: {(await _client.GetGuildSummariesAsync()).Count}\n" +
                $"- Channels: {(await _client.GetGuildsAsync()).Select(async g => await g.GetChannelsAsync()).Count()}" +
                $"- Users: {(await _client.GetGuildsAsync()).Select(async g => await g.GetUsersAsync()).Count()}"
            );
        }

        private static string GetUptime()
            => (DateTime.Now - Process.GetCurrentProcess().StartTime).ToString(@"dd\.hh\:mm\:ss");
        private static string GetHeapSize() => Math.Round(GC.GetTotalMemory(true) / (1024.0 * 1024.0), 2).ToString();
    }
}