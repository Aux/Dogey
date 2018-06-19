using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly LoggingService _logger;
        private readonly RootController _root;
        private readonly IServiceProvider _provider;

        private const string _tempPrefix = "!";

        public CommandHandler(
            DiscordSocketClient discord,
            CommandService commands,
            LoggingService logger,
            RootController root,
            IServiceProvider provider)
        {
            _discord = discord;
            _commands = commands;
            _logger = logger;
            _root = root;
            _provider = provider;

            _discord.MessageReceived += OnMessageReceivedAsync;
        }

        private async Task OnMessageReceivedAsync(SocketMessage s)
        {
            if (!(s is SocketUserMessage msg)) return;
            if (msg.Author.Id == _discord.CurrentUser.Id) return;

            var context = new DogeyCommandContext(_discord, msg);
            string prefix = await _root.GetPrefixAsync(context.Guild?.Id ?? 0);

            int argPos = 0;
            bool hasStringPrefix = prefix == null ? false : msg.HasStringPrefix(prefix, ref argPos);

            if (hasStringPrefix || msg.HasMentionPrefix(_discord.CurrentUser, ref argPos))
                await ExecuteAsync(context, _provider, context.Message.Content.Substring(argPos));
        }

        public async Task ExecuteAsync(DogeyCommandContext context, IServiceProvider provider, string input)
        {
            var result = await _commands.ExecuteAsync(context, input, provider);

            if (result.IsSuccess || result.Error == CommandError.UnknownCommand)
                return;
            if (result is ExecuteResult execute)
                await _logger.LogAsync(LogSeverity.Error, "Commands", execute.Exception?.ToString());
            if (result is ParseResult parse && parse.Error == CommandError.BadArgCount)
            {
                var command = _commands.Search(context, input).Commands
                    .OrderByDescending(x => x.Command.Parameters.Count())
                    .FirstOrDefault().Command;

                var builder = new StringBuilder(_tempPrefix + command.Name);
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
