using Discord;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey
{
    public class ChannelWatcher
    {
        private readonly DiscordSocketClient _discord;
        private readonly DogManager _manager;

        private const ulong _guildId = 158057120493862912;
        private ulong[] _channelIds = new[] { 206606983250313216ul, 318586966788669450ul };
        
        public ChannelWatcher(DiscordSocketClient discord, DogManager manager)
        {
            _discord = discord;
            _manager = manager;
            
            _discord.MessageReceived += OnMessageReceivedAsync;
            _discord.MessageDeleted += OnMessageDeletedAsync;
            //_discord.Ready += OnReadyAsync;
        }

        private async Task OnMessageReceivedAsync(SocketMessage msg)
        {
            await Task.Delay(0);
            if (_channelIds.Any(x => x == msg.Channel.Id))
                return;
        }

        private async Task OnMessageDeletedAsync(Cacheable<IMessage, ulong> msg, ISocketMessageChannel channel)
        {
            await Task.Delay(0);
            if (_channelIds.Any(x => x == channel.Id))
                return;
        }

        private async Task OnReadyAsync()
        {
            try
            {
                var guild = _discord.GetGuild(_guildId);
                foreach (var channelId in _channelIds)
                {
                    var channel = guild.GetTextChannel(channelId);
                    var messages = await channel.GetMessagesAsync(50).Flatten();

                    foreach (var msg in messages)
                    {
                        if (msg is IUserMessage usermsg)
                        {
                            await _manager.AddDogImageAsync(usermsg);
                            await PrettyConsole.LogAsync("Info", "ChannelWatcher", $"Added {channelId}/{usermsg.Id}");
                        }
                    }
                }
            } catch (Exception ex)
            {
                await PrettyConsole.LogAsync("Error", "ChannelWatcher", ex.ToString() + "\n\n");
            }

            _discord.Ready -= OnReadyAsync;
        }
    }
}
