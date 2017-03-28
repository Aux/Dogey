using Discord.Commands;
using Discord.WebSocket;
using NTwitch.Rest;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Dogey
{
    public class ServiceManager
    {
        private DependencyMap _map;
        private DiscordSocketClient _discord;
        private TwitchRestClient _twitch;

        //sqlite
        private SQLite.LoggingService _litelog;
        private SQLite.CommandHandler _litecommands;

        public ServiceManager(DiscordSocketClient discord, TwitchRestClient twitch)
        {
            _map = new DependencyMap();
            _discord = discord;
            _twitch = twitch;

            _map.Add(_discord);
            _map.Add(_twitch);
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
            string dataPath = Path.Combine(AppContext.BaseDirectory, "data");
            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);

            using (var db = new SQLite.ConfigDatabase())
                db.Database.EnsureCreated();
            using (var db = new SQLite.LogDatabase())
                db.Database.EnsureCreated();
            using (var db = new SQLite.TagDatabase())
                db.Database.EnsureCreated();
            using (var db = new SQLite.PatsDatabase())
                db.Database.EnsureCreated();

            _litelog = new SQLite.LoggingService(_discord);
            _litecommands = new SQLite.CommandHandler();

            await _litecommands.InitializeAsync(commands, _map);
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
