using Discord;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dogey
{
    public class DogService
    {
        public const string ApiUrl = "https://api.thedogapi.co.uk/v2/dog.php";
        public const string ImageUrl = "https://i.thedogapi.co.uk/{0}.{1}";
        public const int RequestsPerMinute = 10;
        public int RequestsRemaining => _requestsRemaining;

        private readonly IConfiguration _config;
        private readonly LoggingService _logger;
        private readonly HttpClient _http;

        private int _requestsRemaining = RequestsPerMinute;
        private DateTime? _resetAt = null;

        public string GetImageUrl(string id, string format = "jpg")
            => string.Format(ImageUrl, id, format);

        public DogService(IConfiguration config, LoggingService logger, HttpClient http)
        {
            _config = config;
            _logger = logger;
            _http = http;
        }

        private bool IsRatelimited()
        {
            if (_resetAt <= DateTime.UtcNow.AddMinutes(1))
            {
                _resetAt = DateTime.UtcNow;
                _requestsRemaining = RequestsPerMinute;
            }

            if (_requestsRemaining == 0) return true;
            _requestsRemaining--;
            return false;
        }

        public async Task<DogData> GetDogAsync(string id = null)
        {
            if (IsRatelimited()) return null;

            string query = null;
            if (!string.IsNullOrWhiteSpace(id)) query = "?id=" + id;

            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, Path.Combine(ApiUrl, query)))
                {
                    var response = await _http.SendAsync(request);
                    if (!response.IsSuccessStatusCode)
                        return null;

                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<DogResponse>(content).Data.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                await _logger.LogAsync(LogSeverity.Error, "Dog", ex.ToString());
            }
            return null;
        }

        public async Task<Stream> GetDogImageAsync(DogData data)
        {
            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, new Uri(data.ImageUrl)))
                {
                    var response = await _http.SendAsync(request);
                    if (!response.IsSuccessStatusCode)
                        return null;

                    return await response.Content.ReadAsStreamAsync();
                }
            }
            catch (Exception ex)
            {
                await _logger.LogAsync(LogSeverity.Error, "Dog", ex.ToString());
            }
            return null;
        }
    }
}
