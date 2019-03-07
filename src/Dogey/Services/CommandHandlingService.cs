using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Modules;
using Microsoft.Extensions.Logging;

namespace Dogey.Services
{
    public class CommandHandlingService : Service
    {
        private readonly ILogger<CommandHandlingService> _logger;
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly PrefixService _prefix;

        public CommandHandlingService(
            ILogger<CommandHandlingService> logger,
            IServiceProvider provider,
            DiscordSocketClient discord,
            CommandService commands,
            PrefixService prefix)
        {
            _logger = logger;
            _provider = provider;
            _discord = discord;
            _commands = commands;
            _prefix = prefix;
        }
        
        public override void Start()
        {
            _discord.MessageReceived += OnMessageReceivedAsync;
            _logger.LogInformation("Started");
        }

        public override void Stop()
        {
            _discord.MessageReceived -= OnMessageReceivedAsync;
            _logger.LogInformation("Stopped");
        }
        
        private Task OnMessageReceivedAsync(SocketMessage s)
        {
            _ = Task.Run(async () =>
            {
                if (!(s is SocketUserMessage msg)) return;

                var context = new DogeyCommandContext(_discord, msg);
                if (_prefix.TryGetPosition(context, out int argPos))
                {
                    var typing = context.Channel.EnterTypingState();
                    await ExecuteAsync(context, _provider, context.Message.Content.Substring(argPos));
                    typing.Dispose();
                }
            });
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

                var builder = new StringBuilder(context.Client.CurrentUser.Mention + " " + command.Name);
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
                            argText = $"[{argText + (arg.DefaultValue != null ? "=" + arg.DefaultValue : "")}]";
                        else
                            argText = $"<{argText}>";

                        builder.Append(" " + argText);
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