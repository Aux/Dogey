using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Dogey
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly IServiceProvider _provider;

        public CommandHandler(IServiceProvider provider)
        {
            _provider = provider;
            _client = _provider.GetService<DiscordSocketClient>();
            _commands = _provider.GetService<CommandService>();
        }

        public async Task StartAsync()
        {
            PrettyConsole.Log(LogSeverity.Info, "Commands", $"Loading commands");

            _commands.AddTypeReader<Uri>(new UriTypeReader());

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
            await _commands.AddModulesAsync(typeof(Entity<>).GetTypeInfo().Assembly);

            _commands.Log += OnLogAsync;

            _client.MessageReceived += HandleCommandAsync;
            PrettyConsole.Log(LogSeverity.Info, "Commands", $"Ready, loaded {_commands.Commands.Count()} commands");
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null)
                return;

            var context = new DogeyCommandContext(_client, msg);
            string prefix = await context.Guild.GetPrefixAsync();

            int argPos = 0;
            bool hasStringPrefix = prefix == null ? false : msg.HasStringPrefix(prefix, ref argPos);

            if (hasStringPrefix || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
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
            if (!result.IsSuccess)
            {
                if (result is ExecuteResult r)
                    Console.WriteLine(r.Exception.ToString());
                else if (result.Error == CommandError.UnknownCommand)
                    await context.Channel.SendMessageAsync("Command not recognized");
                else
                    await context.Channel.SendMessageAsync(result.ToString());
            }
        }

        private Task OnLogAsync(LogMessage msg)
            => PrettyConsole.LogAsync(msg.Severity, msg.Source, msg.Exception?.ToString() ?? msg.Message);
    }
}
