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
            _service = new CommandService();                         

            await _service.AddModulesAsync(Assembly.GetEntryAssembly());

            var config = Configuration.Load();
            switch (config.Database)
            {
                case DbMode.SQLite:
                    PrettyConsole.Log(LogSeverity.Info, "Commands", $"Loading SQLite commands");
                    await _service.LoadSqliteModulesAsync(); break;
                case DbMode.MySQL:
                    PrettyConsole.Log(LogSeverity.Info, "Commands", $"Loading MySQL commands");
                    throw new NotImplementedException();
                case DbMode.Redis:
                    PrettyConsole.Log(LogSeverity.Info, "Commands", $"Loading Redis commands");
                    throw new NotImplementedException();
                case DbMode.MongoDB:
                    PrettyConsole.Log(LogSeverity.Info, "Commands", $"Loading MongoDB commands");
                    throw new NotImplementedException();
                case DbMode.PostgreSQL:
                    PrettyConsole.Log(LogSeverity.Info, "Commands", $"Loading PostgreSQL commands");
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
                    await context.Channel.SendMessageAsync(result.ToString());
            }
        }
    }
}
