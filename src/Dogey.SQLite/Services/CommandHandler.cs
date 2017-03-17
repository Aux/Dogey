using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Dogey.SQLite
{
    public class CommandHandler
    {
        private DiscordSocketClient _client;
        private CommandService _service;

        public async Task InitializeAsync(DiscordSocketClient c, CommandService service)
        {
            PrettyConsole.Log(LogSeverity.Info, "Commands", $"Loading SQLite commands");
            _client = c;
            _service = service;

            await _service.AddModulesAsync(Assembly.GetEntryAssembly());
            await _service.AddModulesAsync(typeof(LiteEntity<>).GetTypeInfo().Assembly);

            _client.MessageReceived += HandleCommandAsync;
            PrettyConsole.Log(LogSeverity.Info, "Commands", $"Ready, loaded {_service.Commands.Count()} commands");
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            var timer = new Stopwatch();
            timer.Start();

            var msg = s as SocketUserMessage;
            if (msg == null)
                return;

            var context = new SocketCommandContext(_client, msg);
            string prefix = await context.Guild.GetPrefixAsync();

            int argPos = 0;
            bool hasStringPrefix = prefix == null ? false : msg.HasStringPrefix(prefix, ref argPos);

            if (hasStringPrefix || msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var result = await _service.ExecuteAsync(context, argPos);

                timer.Stop();
                if (!result.IsSuccess)
                {
                    if (result is ExecuteResult r)
                        Console.WriteLine(r.Exception.ToString());
                    else if (result.Error == CommandError.UnknownCommand)
                        await context.Channel.SendMessageAsync("Command not recognized");
                    else
                        await context.Channel.SendMessageAsync(result.ToString());
                } else
                {
                    await context.LogAsync(timer.ElapsedMilliseconds);
                }
            }
        }
    }
}
