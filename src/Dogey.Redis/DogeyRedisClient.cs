using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace Dogey.Redis
{
    public class DogeyRedisClient
    {
        internal RedisClient Client
            => _client;

        private RedisClient _client;

        public Task ConnectAsync(string url, int port)
        {
            return Task.CompletedTask;
        }

        public Task LoginAsync(string password)
        {
            return Task.CompletedTask;
        }

        public Task DisconnectAsync()
        {
            return Task.CompletedTask;
        }

        public Task SubscribeAsync(string topic, Action<RedisChannel, RedisValue> action)
            => _client.SubscribeAsync(topic, action);

        public Task PublishAsync(string topic, object content)
            => _client.PublishAsync(topic, content);
    }
}