using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dogey
{
    public class PointEarningService : BackgroundService
    {
        public ConcurrentDictionary<string, Func<IEnumerable<PointLog>, Task>> Actions { get; }
        private ConcurrentDictionary<ulong, Wallet> ActiveWallets { get; }

        private readonly ILogger<PointEarningService> _logger;
        private readonly DiscordSocketClient _discord;
        private readonly PointsController _points;
        private readonly RootController _root;

        private CancellationToken _cancellationToken;
        private Timer _timer;

        public PointEarningService(ILogger<PointEarningService> logger, DiscordSocketClient discord, PointsController points, RootController root)
        {
            Actions = new ConcurrentDictionary<string, Func<IEnumerable<PointLog>, Task>>();
            ActiveWallets = new ConcurrentDictionary<ulong, Wallet>();

            _timer = new Timer(OnTimer, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));

            _logger = logger;
            _discord = discord;
            _points = points;
            _root = root;
        }

        private void OnTimer(object state)
        {
            if (ActiveWallets.IsEmpty) return;

            var instanceId = Guid.NewGuid().ToString("N");
            var logs = new List<PointLog>();

            foreach (var wallet in ActiveWallets.Values)
            {
                var log = _points.CreateAsync(new PointLog
                {
                    Timestamp = DateTime.UtcNow,
                    UserId = wallet.Id,
                    SenderId = instanceId,
                    EarningType = EarningType.Activity,
                    Amount = 5
                }).GetAwaiter().GetResult();

                wallet.Balance += log.Amount;
                _points.ModifyAsync(wallet).GetAwaiter().GetResult();

                logs.Add(log);
            }

            ActiveWallets.Clear();
            foreach (var action in Actions.Values)
                action(logs);
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            _discord.MessageReceived += OnMessageReceivedAsync;
            _logger.LogInformation("Started");
            return Task.CompletedTask;
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            _discord.MessageReceived -= OnMessageReceivedAsync;
            _logger.LogInformation("Stopped");
        }

        public bool TryAddAction(string id, Func<IEnumerable<PointLog>, Task> func)
        {
            return Actions.TryAdd(id, func);
        }

        public bool TryRemoveAction(string id, out Func<IEnumerable<PointLog>, Task> value)
        {
            value = null;
            if (Actions.TryRemove(id, out Func<IEnumerable<PointLog>, Task> response))
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
                
                var wallet = await _points.GetOrCreateWalletAsync(msg.Author);
                ActiveWallets.TryAdd(wallet.Id, wallet);
            }, _cancellationToken);
            return Task.CompletedTask;
        }
        
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
            => throw new NotImplementedException();
    }
}
