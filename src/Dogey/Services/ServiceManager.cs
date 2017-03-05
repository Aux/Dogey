using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Dogey
{
    public class ServiceManager
    {
        private DiscordSocketClient _client;

        //sqlite
        private SQLite.LoggingService _litelog;
        private SQLite.CommandHandler _litecommands;

        public ServiceManager(DiscordSocketClient client)
        {
            _client = client;
        }

        public async Task InitializeAsync()
        {
            var commands = new CommandService(new CommandServiceConfig()
            {
                CaseSensitiveCommands = false,
#if DEBUG
                DefaultRunMode = RunMode.Sync
#elif RELEASE
                DefaultRunMode = RunMode.Async
#endif
            });

            commands.AddTypeReader(typeof(Uri), new UriTypeReader());
            await commands.AddModulesAsync(Assembly.GetEntryAssembly());

            switch (Configuration.Load().Database)
            {
                case DbMode.SQLite:
                    await InitializeSQLiteAsync(commands); break;
                case DbMode.MySQL:
                    await InitializeMySQLAsync(commands); break;
                case DbMode.Redis:
                    await InitializeRedisAsync(commands); break;
                default:
                    throw new NotImplementedException();
            }
        }

        private async Task InitializeSQLiteAsync(CommandService commands)
        {
            using (var db = new SQLite.ConfigDatabase())
                db.Database.EnsureCreated();
            using (var db = new SQLite.LogDatabase())
                db.Database.EnsureCreated();
            using (var db = new SQLite.TagDatabase())
                db.Database.EnsureCreated();
            
            _litelog = new SQLite.LoggingService(_client);
            _litecommands = new SQLite.CommandHandler();

            await _litecommands.InitializeAsync(_client, commands);
        }

        private Task InitializeMySQLAsync(CommandService commands)
        {
            throw new NotImplementedException();
        }

        private Task InitializeRedisAsync(CommandService commands)
        {
            throw new NotImplementedException();
        }
    }
}
