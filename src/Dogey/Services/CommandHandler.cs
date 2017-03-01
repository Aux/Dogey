using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.SQLite;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Dogey.Services
{
    public class CommandHandler
    {
        private DiscordSocketClient _client;
        private CommandService _service;

        public async Task InitializeAsync(DiscordSocketClient c)
        {
            _client = c;                                            
            _service = new CommandService(new CommandServiceConfig()
            {
                CaseSensitiveCommands = false,
#if DEBUG
                DefaultRunMode = RunMode.Sync
#elif RELEASE
                DefaultRunMode = RunMode.Async
#endif
            });

            _service.AddTypeReader(typeof(Uri), new UriTypeReader());
            await _service.AddModulesAsync(Assembly.GetEntryAssembly());

            var config = Configuration.Load();
            PrettyConsole.Log(LogSeverity.Info, "Commands", $"Loading {config.Database} commands");
            switch (config.Database)
            {
                case DbMode.SQLite:
                    await _service.LoadSqliteModulesAsync(); break;
                case DbMode.MySQL:
                    throw new NotImplementedException();
                case DbMode.Redis:
                    throw new NotImplementedException();
                case DbMode.MongoDB:
                    throw new NotImplementedException();
                case DbMode.PostgreSQL:
                    throw new NotImplementedException();
                default:
                    break;
            }

            _client.MessageReceived += HandleCommandAsync;
            PrettyConsole.Log(LogSeverity.Info, "Commands", $"Ready, loaded {_service.Commands.Count()} commands");
        }

        private async Task HandleCommandAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null)
                return;

            var context = new SocketCommandContext(_client, msg);
            string prefix = Configuration.Load().Prefix;

            int argPos = 0;
            if (msg.HasStringPrefix(prefix, ref argPos) ||
                msg.HasMentionPrefix(_client.CurrentUser, ref argPos))
            {
                var result = await _service.ExecuteAsync(context, argPos);

                if (!result.IsSuccess)
                {
                    if (result is ExecuteResult r)
                        Console.WriteLine(r.Exception.ToString());
                    else
                        await context.Channel.SendMessageAsync(result.ToString());
                }
            }
        }
    }
}
