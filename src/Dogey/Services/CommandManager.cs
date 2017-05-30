using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Dogey
{
    public class CommandManager
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IServiceProvider _provider;

        public CommandManager(IServiceProvider provider)
        {
            _provider = provider;
            _discord = _provider.GetService<DiscordSocketClient>();
            _commands = _provider.GetService<CommandService>();
        }

        public async Task StartAsync()
        {
            _commands.AddTypeReader<Uri>(new UriTypeReader());

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
            
            _discord.MessageReceived += OnMessageReceivedAsync;
            PrettyConsole.Log(LogSeverity.Info, "Commands", $"Loaded {_commands.Modules.Count()} modules with {_commands.Commands.Count()} commands");
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
            {
                PrettyConsole.Log(LogSeverity.Error, "Commands", r.Exception?.ToString());
                return;
            }

            await context.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
}
