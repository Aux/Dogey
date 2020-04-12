using Discord.Commands;
using Discord.WebSocket;
using Dogey.Commands;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Dogey.Services
{
    public class CommandHandlingService
    {
        private readonly ILogger<CommandHandlingService> _logger;
        private readonly CommandService _commandService;
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly IServiceProvider _serviceProvider;

        public CommandHandlingService(
            ILogger<CommandHandlingService> logger,
            CommandService commandService,
            DiscordSocketClient discordSocketClient,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _commandService = commandService;
            _discordSocketClient = discordSocketClient;
            _serviceProvider = serviceProvider;
        }

        public void Start()
        {
            _discordSocketClient.MessageReceived += OnMessageReceivedAsync;
            _logger.LogInformation("Started");
        }

        public void Stop()
        {
            _discordSocketClient.MessageReceived -= OnMessageReceivedAsync;
            _logger.LogInformation("Stopped");
        }

        private async Task OnMessageReceivedAsync(SocketMessage s)
        {
            if (!(s is SocketUserMessage msg)) return;
            if (!(s.Channel is SocketGuildChannel)) return;

            int argPos = 0;
            var context = new DogeyCommandContext(_discordSocketClient, msg);
            if (msg.HasMentionPrefix(_discordSocketClient.CurrentUser, ref argPos))
            {
                var result = await _commandService.ExecuteAsync(context, argPos, _serviceProvider);
                if (result.IsSuccess) return;

                switch (result)
                {
                    case ExecuteResult execute:
                        _logger.LogError(execute.Exception?.ToString());
                        return;
                    case ParseResult parse when parse.Error == CommandError.BadArgCount:
                        // Send Help Text
                        return;
                    default:
                        await context.Channel.SendMessageAsync(result.ErrorReason);
                        return;
                }
            }
        }
    }
}
