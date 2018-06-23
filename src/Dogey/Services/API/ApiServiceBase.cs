using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dogey
{
    public abstract class ApiServiceBase
    {
        public const int DefaultRequestLimit = 30;
        public const int DefaultResetInterval = 30; // Minutes
        public int RequestLimit => _requestLimit;
        public int RequestsRemaining => _requestsRemaining;
        public TimeSpan ResetInterval => TimeSpan.FromMinutes(_resetInterval);
        public DateTime ResetAt => _resetAt;

        private int _requestLimit;
        private int _resetInterval;
        private int _requestsRemaining;
        private DateTime _resetAt;
        
        private readonly HttpClient _http;
        
        protected ApiServiceBase(HttpClient http, int? requestLimit = null, int? resetInterval = null)
        {
            _http = http;

            _requestLimit = requestLimit ?? DefaultRequestLimit;
            _resetInterval = resetInterval ?? DefaultResetInterval;
            _resetAt = DateTime.MinValue;
        }

        public void SetRequestLimit(int? requestLimit = null)
        {
            _requestLimit = requestLimit ?? DefaultRequestLimit;
        }
        public void SetRequestInterval(int? resetInterval = null)
        {
            _resetInterval = resetInterval ?? DefaultResetInterval;
        }

        protected bool IsRatelimited()
        {
            if (_resetAt <= DateTime.UtcNow)
            {
                _resetAt = DateTime.UtcNow.AddMinutes(_resetInterval);
                _requestsRemaining = _requestLimit;
            }

            if (_requestsRemaining == 0) return true;
            _requestsRemaining--;
            return false;
        }

        protected async Task<HttpResponseMessage> SendAsync(HttpRequestMessage message)
        {
            var response = await _http.SendAsync(message).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            return response;
        }
        protected async Task<Stream> SendAsync(HttpMethod method, string query)
        {
            using (var request = new HttpRequestMessage(method, query))
            {
                var response = await SendAsync(request);
                return await response.Content.ReadAsStreamAsync();
            }
        }
        protected async Task<T> SendAsync<T>(HttpMethod method, string query)
        {
            using (var request = new HttpRequestMessage(method, query))
            {
                var response = await SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(json);
            }
        }
    }
}
