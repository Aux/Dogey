using Discord.WebSocket;
using System;
using System.Threading.Tasks;

namespace Dogey
{
    public class ServiceManager
    {
        private DiscordSocketClient _client;

        public ServiceManager(DiscordSocketClient client)
        {
            _client = client;
        }

        public async Task InitializeSQLiteAsync()
        {
            await Task.Delay(0);
        }

        public Task InitializeMySQLAsync()
        {
            throw new NotImplementedException();
        }

        public Task InitializeRedisAsync()
        {
            throw new NotImplementedException();
        }
    }
}
