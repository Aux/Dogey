using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Dogey
{
    public class PointEarningService : BackgroundService
    {
        public ConcurrentDictionary<string, Func<PointLog, Task>> Actions { get; }

        private readonly ILogger<PointEarningService> _logger;
        private readonly CommandHandlingService _commands;
        private readonly DiscordSocketClient _discord;
        private readonly PointsController _points;
        private readonly RootController _root;

        private CancellationToken _cancellationToken;

        public PointEarningService(ILogger<PointEarningService> logger, CommandHandlingService commands, DiscordSocketClient discord, PointsController points, RootController root)
        {
            Actions = new ConcurrentDictionary<string, Func<PointLog, Task>>();
            _logger = logger;
            _commands = commands;
            _discord = discord;
            _points = points;
            _root = root;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _discord.MessageReceived += OnMessageReceivedAsync;
            _discord.MessageDeleted += OnMessageDeletedAsync;
            _logger.LogInformation("Started");
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            _discord.MessageReceived -= OnMessageReceivedAsync;
            _discord.MessageDeleted -= OnMessageDeletedAsync;
            _logger.LogInformation("Stopped");
        }

        public bool TryAddAction(string id, Func<PointLog, Task> func)
        {
            return Actions.TryAdd(id, func);
        }

        public bool TryRemoveAction(string id, out Func<PointLog, Task> value)
        {
            value = null;
            if (Actions.TryRemove(id, out Func<PointLog, Task> response))
                value = response;
            return value != null;
        }
        
        private Task OnMessageReceivedAsync(SocketMessage s)
        {
            _ = Task.Run(async () =>
            {
                //bool plonked = await _root.IsBannedAsync(msg.Author);
                //if (plonked || msg.Author.IsBot) return;

                if (!(s is SocketUserMessage msg)) return;
                if (msg.Author.IsBot) return;

                var context = new DogeyCommandContext(_discord, msg);
                string prefix = await _root.GetPrefixAsync(context.Guild?.Id ?? 0);

                if (_commands.IsCommand(context, prefix, out int argPos)) return;

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
                        var multiplied = earning * wallet.Multiplier.Value;
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

                        foreach (var action in Actions.Values)
                            await action(log);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Unable to add points: {ex}", ex);
                }
            }, _cancellationToken);
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
                    _logger.LogError("Unable to remove points: {ex}", ex);
                }
            }, _cancellationToken);
            return Task.CompletedTask;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
            => throw new NotImplementedException();
    }
}
