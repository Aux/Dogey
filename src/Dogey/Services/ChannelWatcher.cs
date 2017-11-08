using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey
{
    public class ChannelWatcher
    {
        private readonly DiscordSocketClient _discord;
        private readonly LoggingService _logger;
        private readonly DogManager _manager;

        private const ulong _guildId = 158057120493862912;
        private ulong[] _channelIds = new[] { 206606983250313216ul, 318586966788669450ul };
        
        public ChannelWatcher(DiscordSocketClient discord, LoggingService logger, DogManager manager)
        {
            _discord = discord;
            _logger = logger;
            _manager = manager;
            
            _discord.MessageReceived += OnMessageReceivedAsync;
            _discord.MessageDeleted += OnMessageDeletedAsync;
            _discord.Ready += OnReadyAsync;
        }

        private async Task OnMessageReceivedAsync(SocketMessage msg)
        {
            if (_channelIds.Any(x => x == msg.Channel.Id) && msg is SocketUserMessage usermsg)
            {
                await _manager.AddDogImageAsync(usermsg);
                await _logger.LogAsync("Info", "ChannelWatcher", $"Added {msg.Channel.Id}/{usermsg.Id}");
            }
        }

        private async Task OnMessageDeletedAsync(Cacheable<IMessage, ulong> msg, ISocketMessageChannel channel)
        {
            if (_channelIds.Any(x => x == channel.Id))
            {
                await _manager.RemoveDogImageAsync(msg.Id);
                await _logger.LogAsync("Info", "ChannelWatcher", $"Removed {channel.Id}/{msg.Id}");
            }
        }

        private async Task OnReadyAsync()
        {
            try
            {
                var guild = _discord.GetGuild(_guildId);
                foreach (var channelId in _channelIds)
                {
                    var channel = guild.GetTextChannel(channelId);

                    IEnumerable<IMessage> messages;

                    var latestImage = await _manager.GetLastestImageAsync(channelId);
                    if (latestImage == null)
                        messages = await channel.GetMessagesAsync(100000).Flatten();
                    else
                        messages = await channel.GetMessagesAsync(latestImage.MessageId, Direction.After).Flatten();
                    
                    foreach (var msg in messages)
                    {
                        if (msg is IUserMessage usermsg)
                        {
                            var isDupe = await _manager.IsDupeImageAsync(usermsg);
                            if (isDupe) continue;

                            await _manager.AddDogImageAsync(usermsg);
                            await _logger.LogAsync("Info", "ChannelWatcher", $"Added {channelId}/{usermsg.Id}");
                        }
                    }
                }
            } catch (Exception ex)
            {
                await _logger.LogAsync("Error", "ChannelWatcher", ex.ToString() + "\n\n");
            }

            _discord.Ready -= OnReadyAsync;
        }
    }
}
