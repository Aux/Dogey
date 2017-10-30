using Discord;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Dogey
{
    public class PointsService
    {
        private readonly DiscordSocketClient _discord;
        private readonly PointsManager _manager;
        private readonly LoggingService _logger;

        private const ulong _dapi = 81384788765712384;

        public PointsService(
            DiscordSocketClient discord,
            PointsManager manager,
            LoggingService logger)
        {
            _discord = discord;
            _manager = manager;
            _logger = logger;

            _discord.MessageReceived += OnMessageReceivedAsync;
            //_discord.MessageDeleted += OnMessageDeletedAsync;
        }

        private Task OnMessageReceivedAsync(SocketMessage msg)
        {
            _ = Task.Run(async () =>
            {
                if ((msg.Channel is IGuildChannel g) && g.Id == _dapi)
                    return;

                try
                {
                    if (msg.Author.IsBot) return;

                    var multiplier = GetMultiplier(msg.Id);
                    if (multiplier == 0)
                        return;

                    await _manager.CreateAsync(new MessagePoint
                    {
                        MessageId = msg.Id,
                        Amount = multiplier,
                        UserId = msg.Author.Id
                    });

                    await _manager.TryCreateProfileAsync(msg.Author.Id);
                    await _manager.UpdateTotalPointsAsync(msg.Author.Id, multiplier);
                } catch (Exception ex)
                {
                    await _logger.LogAsync("Error", "PointsService", ex.ToString() + "\n\n");
                }
            });
            return Task.CompletedTask;
        }

        private Task OnMessageDeletedAsync(Cacheable<IMessage, ulong> msg, ISocketMessageChannel channel)
        {
            _ = Task.Run(async () =>
            {
                try
                {
                    var point = await _manager.GetPointAsync(msg.Id);
                    if (point == null) return;

                    await _manager.DeletePointAsync(point);
                    await _manager.UpdateTotalPointsAsync(point.UserId, point.Amount * -1);
                }
                catch (Exception ex)
                {
                    await _logger.LogAsync("Error", "PointsService", ex.ToString() + "\n\n");
                }
            });
            return Task.CompletedTask;
        }

        public int GetMultiplier(ulong msgId)
        {
            int totalMult = 0;
            
            // Add 10x mult for prime
            if (MathHelper.IsPrime(msgId))
                totalMult += 10;

            // Add 3-5x mult for repeating chars
            for (int mult = 3; mult <= 5; mult++)
            {
                int repeats = StringHelper.RepeatingChars(msgId.ToString(), mult);
                if (repeats > 0)
                    totalMult += repeats * (mult - 1);
            }

            return totalMult;
        }
    }
}
