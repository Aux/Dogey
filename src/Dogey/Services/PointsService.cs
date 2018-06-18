using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dogey
{
    public class PointsService
    {
        public Dictionary<string, Func<PointLog, Task>> Actions => _actions;

        private readonly DiscordSocketClient _discord;
        private readonly PointsController _points;
        private readonly RootController _root;
        private readonly LoggingService _logger;

        private Dictionary<string, Func<PointLog, Task>> _actions;

        public PointsService(DiscordSocketClient discord, PointsController points, RootController root, LoggingService logger)
        {
            _actions = new Dictionary<string, Func<PointLog, Task>>();
            _discord = discord;
            _points = points;
            _root = root;
            _logger = logger;

            _discord.MessageReceived += OnMessageReceivedAsync;
            _discord.MessageDeleted += OnMessageDeletedAsync;
        }
        
        public void AddAction(string id, Func<PointLog, Task> func)
        {
            _actions.Add(id, func);
        }

        public void RemoveAction(string id)
        {
            _actions.Remove(id);
        }
        
        private Task OnMessageReceivedAsync(SocketMessage msg)
        {
            _ = Task.Run(async () =>
            {
                bool plonked = await _root.IsBannedAsync(msg.Author);
                if (plonked) return;

                var wallet = await _points.GetOrCreateWalletAsync(msg.Author);

                try
                {
                    int earning = 0;

                    // Bonus points for a prime id
                    if (MathHelper.IsPrime(msg.Id))
                        earning += 25;

                    // Add 3-5x points for concecutive numbers
                    for (int mult = 3; mult <= 5; mult++)
                    {
                        int repeats = StringHelper.RepeatingChars(msg.Id.ToString(), mult);
                        if (repeats > 0)
                            earning += repeats * (mult - 1);
                    }

                    if (wallet.Multiplier != null)
                    {
                        var multiplied = (double)earning * wallet.Multiplier.Value;
                        earning += (int)Math.Round(multiplied, MidpointRounding.AwayFromZero);
                    }

                    if (earning > 0)
                    {
                        var log = await _points.CreateAsync(new PointLog
                        {
                            Timestamp = msg.Timestamp.DateTime,
                            UserId = msg.Author.Id,
                            SenderId = msg.Id,
                            Amount = earning
                        });

                        wallet.Balance += log.Amount;
                        await _points.ModifyAsync(wallet);

                        foreach (var action in _actions.Values)
                            await action(log);
                    }
                }
                catch (Exception ex)
                {
                    await _logger.LogAsync(LogSeverity.Error, "Points", $"Unable to add points: {ex}");
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
                    var log = await _points.GetLogAsync(msg.Id);
                    if (log == null) return;

                    var user = msg.HasValue ? msg.Value.Author : _discord.GetUser(log.UserId);
                    var wallet = await _points.GetWalletAsync(user);
                    wallet.Balance -= log.Amount;

                    await _points.ModifyAsync(wallet);
                    await _points.DeleteAsync(log);
                }
                catch (Exception ex)
                {
                    await _logger.LogAsync(LogSeverity.Error, "Points", $"Unable to remove points: {ex}");
                }
            });
            return Task.CompletedTask;
        }
    }
}
