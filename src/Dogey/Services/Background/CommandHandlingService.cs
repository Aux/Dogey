using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dogey
{
    public class CommandHandlingService : BackgroundService
    {
        private readonly ILogger<CommandHandlingService> _logger;
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly RootController _root;
        private readonly IServiceProvider _provider;

        private CancellationToken _cancellationToken;

        public CommandHandlingService(
            ILogger<CommandHandlingService> logger,
            DiscordSocketClient discord,
            CommandService commands,
            RootController root,
            IServiceProvider provider)
        {
            _logger = logger;
            _discord = discord;
            _commands = commands;
            _root = root;
            _provider = provider;
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
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
            => throw new NotImplementedException();
        
        public bool IsCommand(DogeyCommandContext context, string prefix, out int argPos)
        {
            argPos = 0;
            if (context.User.Id == _discord.CurrentUser.Id) return false;
            bool hasStringPrefix = prefix == null ? false : context.Message.HasStringPrefix(prefix, ref argPos);
            return (hasStringPrefix || context.Message.HasMentionPrefix(_discord.CurrentUser, ref argPos));
        }

        private Task OnMessageReceivedAsync(SocketMessage s)
        {
            _ = Task.Run(async () =>
            {
                if (!(s is SocketUserMessage msg)) return;

                var context = new DogeyCommandContext(_discord, msg);
                string prefix = await _root.GetPrefixAsync(context.Guild?.Id ?? 0);

                if (IsCommand(context, prefix, out int argPos))
                {
                    using (context.Channel.EnterTypingState())
                        await ExecuteAsync(context, _provider, context.Message.Content.Substring(argPos));
                }
            }, _cancellationToken);
            return Task.CompletedTask;
        }

        public async Task ExecuteAsync(DogeyCommandContext context, IServiceProvider provider, string input)
        {
            var result = await _commands.ExecuteAsync(context, input, provider);

            if (result.IsSuccess || result.Error == CommandError.UnknownCommand)
                return;
            if (result is ExecuteResult execute)
                _logger.LogError(execute.Exception?.ToString());
            if (result is ParseResult parse && parse.Error == CommandError.BadArgCount)
            {
                var command = _commands.Search(context, input).Commands
                    .OrderByDescending(x => x.Command.Parameters.Count())
                    .FirstOrDefault().Command;

                var builder = new StringBuilder("!" + command.Name);
                if (command.Parameters.Count > 0)
                {
                    // !name <required> [optional=1]
                    foreach (var arg in command.Parameters)
                    {
                        string argText = arg.Name;

                        if (arg.IsRemainder)
                            argText += "...";
                        if (arg.IsMultiple)
                            argText += "+";

                        if (arg.IsOptional)
                        {
                            argText = '[' + argText;
                            if (arg.DefaultValue != null)
                                argText += ($"={arg.DefaultValue}");
                            argText += ']';
                        }
                        else
                        {
                            argText = '<' + argText;
                            argText += '>';
                        }

                        builder.Append($" {argText}");
                    }
                }

                await context.Channel.SendMessageAsync($"{parse.ErrorReason} {builder}");
                return;
            }

            if (!string.IsNullOrWhiteSpace(result.ErrorReason))
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
}
