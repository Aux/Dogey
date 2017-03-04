using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Dogey
{
    public class ServiceManager
    {
        private DiscordSocketClient _client;
        private CommandHandler _handler;

        //sqlite
        private SQLite.LoggingService _litelog;

        public ServiceManager(DiscordSocketClient client)
        {
            _client = client;
        }

        public async Task InitializeAsync()
        {
            _handler = new CommandHandler();
            await _handler.InitializeAsync(_client);

            switch (Configuration.Load().Database)
            {
                case DbMode.SQLite:
                    await InitializeSQLiteAsync(); break;
                case DbMode.MySQL:
                    await InitializeSQLiteAsync(); break;
                case DbMode.Redis:
                    await InitializeSQLiteAsync(); break;
                default:
                    throw new NotImplementedException();
            }
        }

        private Task InitializeSQLiteAsync()
        {
            _litelog = new SQLite.LoggingService(_client);
            return Task.CompletedTask;
        }

        private Task InitializeMySQLAsync()
        {
            throw new NotImplementedException();
        }

        private Task InitializeRedisAsync()
        {
            throw new NotImplementedException();
        }
    }
}
