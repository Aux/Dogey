using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Dogey
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly LoggingService _logger;
        private readonly IServiceProvider _provider;

        public CommandHandler(
            DiscordSocketClient discord,
            CommandService commands,
            LoggingService logger,
            IServiceProvider provider)
        {
            _discord = discord;
            _commands = commands;
            _logger = logger;
            _provider = provider;

            _discord.MessageReceived += OnMessageReceivedAsync;
        }
        
        private async Task OnMessageReceivedAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null)
                return;

            var context = new DogeyCommandContext(_discord, msg);
            string prefix = await context.Guild.GetPrefixAsync();

            int argPos = 0;
            bool hasStringPrefix = prefix == null ? false : msg.HasStringPrefix(prefix, ref argPos);

            if (hasStringPrefix || msg.HasMentionPrefix(_discord.CurrentUser, ref argPos))
                using (context.Channel.EnterTypingState())
                    await ExecuteAsync(context, _provider, argPos);
        }

        public async Task ExecuteAsync(DogeyCommandContext context, IServiceProvider provider, int argPos)
        {
            var result = await _commands.ExecuteAsync(context, argPos, provider);
            await ResultAsync(context, result);
        }

        public async Task ExecuteAsync(DogeyCommandContext context, IServiceProvider provider, string input)
        {
            var result = await _commands.ExecuteAsync(context, input, provider);
            await ResultAsync(context, result);
        }

        private async Task ResultAsync(DogeyCommandContext context, IResult result)
        {
            if (result.IsSuccess)
                return;
            
            if (result is ExecuteResult r)
                await _logger.LogAsync(LogSeverity.Error, "Commands", r.Exception?.ToString() ?? r.ErrorReason);
            else
                await context.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
}
