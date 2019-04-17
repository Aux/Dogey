using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Modules;
using Dogey.Scripting;
using Microsoft.Extensions.Logging;
using Scriban;
using Scriban.Functions;
using Scriban.Runtime;

namespace Dogey.Services
{
    public class CommandHandlingService : BackgroundService
    {
        private readonly ILogger<CommandHandlingService> _logger;
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _discord;
        private readonly ScriptingService _scripting;
        private readonly CommandService _commands;
        private readonly PrefixService _prefix;

        public CommandHandlingService(
            ILogger<CommandHandlingService> logger,
            IServiceProvider provider,
            DiscordSocketClient discord,
            ScriptingService scripting,
            CommandService commands,
            PrefixService prefix)
        {
            _logger = logger;
            _provider = provider;
            _discord = discord;
            _scripting = scripting;
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
                    using (context.Channel.EnterTypingState())
                    {
                        await ExecuteAsync(context, _provider, context.Message.Content.Substring(argPos));
                    }
                }
            });
            return Task.CompletedTask;
        }

        public async Task ExecuteAsync(DogeyCommandContext context, IServiceProvider provider, string input)
        {
            var result = await _commands.ExecuteAsync(context, input, provider);
            if (result.IsSuccess) return;

            if (result.Error == CommandError.UnknownCommand)
            {
                var parameters = input.Split(' ');
                if (_scripting.TryExecuteScript(parameters.FirstOrDefault().ToLower(), context, out string scriptReply))
                {
                    if (!string.IsNullOrWhiteSpace(scriptReply))
                        await context.Channel.SendMessageAsync(scriptReply);
                    return;
                }
            }
            if (result is ExecuteResult execute)
                _logger.LogError(execute.Exception?.ToString());

            if (result is ParseResult parse && parse.Error == CommandError.BadArgCount)
                await SendHelpTextAsync(context, parse, input);
            else
            if (!string.IsNullOrWhiteSpace(result.ErrorReason))
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }

        private async Task SendHelpTextAsync(DogeyCommandContext context, ParseResult result, string input)
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

            await context.Channel.SendMessageAsync($"{result.ErrorReason} {builder}");
        }
    }
}