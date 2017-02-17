using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Dogey.Redis
{
    internal class RedisClient : IDisposable
    {
        public ConnectionMultiplexer Multiplexer
            => _redis;

        private ConnectionMultiplexer _redis;
        private IDatabase _database;
        private ISubscriber _subscriber;
        private string _url;
        private int _port;
        private bool _disposed;

        public RedisClient(string url, int port = 17044)
        {
            _url = url;
            _port = port;
        }

        public async Task ConnectAsync(string password)
        {
            if (_database != null)
                throw new InvalidOperationException();

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                TypeNameHandling = TypeNameHandling.All
            };

            var options = ConfigurationOptions.Parse($"{_url}:{_port}");
            options.SyncTimeout = 30000;
            options.Password = password;

            _redis = await ConnectionMultiplexer.ConnectAsync(options);
            _database = _redis.GetDatabase();
        }

        public Task DisconnectAsync()
        {
            Dispose();
            return Task.CompletedTask;
        }

        public async Task<T> GetObjectAsync<T>(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentNullException("id");

            var result = await _database.StringGetAsync(id);

            if (!result.HasValue)
                throw new KeyNotFoundException();

            var value = JsonConvert.DeserializeObject<T>(result.ToString());
            return value;
        }

        public Task SubscribeAsync(string topic, Action<RedisChannel, RedisValue> action)
        {
            if (_subscriber == null)
                _subscriber = _redis.GetSubscriber();

            return _subscriber.SubscribeAsync(topic, action, CommandFlags.FireAndForget);
        }

        public Task PublishAsync(string topic, object content)
        {
            if (_subscriber == null)
                _subscriber = _redis.GetSubscriber();

            string value = JsonConvert.SerializeObject(content);
            return _subscriber.PublishAsync(topic, value, CommandFlags.FireAndForget);
        }

        public Task<List<T>> GetObjectsAsync<T>()
        {
            return Task.FromResult(new List<T>());
        }

        public Task SaveObjectAsync<T>(string id, T value)
        {
            return Task.CompletedTask;
        }

        public Task SaveObjectsAsync<T>(string id, List<T> values)
        {
            return Task.CompletedTask;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _redis.Dispose();
                    _database = null;
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}