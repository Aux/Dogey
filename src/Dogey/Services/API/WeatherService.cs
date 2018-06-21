using Discord;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dogey
{
    public class WeatherService
    {
        public const string ApiUrl = "https://api.openweathermap.org/data/2.5/weather";
        public const string IconUrl = "http://openweathermap.org/img/w/{0}.png";
        public const int RequestsPerMinute = 60;
        public int RequestsRemaining => _requestsRemaining;

        private readonly IConfiguration _config;
        private readonly LoggingService _logger;
        private readonly RootController _root;
        private readonly HttpClient _http;

        private string _apiKey;
        private int _requestsRemaining = RequestsPerMinute;
        private DateTime? _resetAt = null;

        public static string GetIconUrl(string id)
            => string.Format(IconUrl, id);

        public WeatherService(IConfiguration config, LoggingService logger, RootController root, HttpClient http)
        {
            _config = config;
            _logger = logger;
            _root = root;
            _http = http;

            _apiKey = _config["tokens:openweather"];
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

        public async Task<Forecast> GetForecastAsync(string city, WeatherUnit unit = WeatherUnit.Metric)
        {
            if (IsRatelimited()) return null;
            
            var builder = new StringBuilder();
            builder.Append("?q=" + city);
            builder.Append("&unit=" + unit.ToString().ToLower());
            builder.Append("&appid=" + _apiKey);

            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, ApiUrl + builder.ToString()))
                {
                    var response = await _http.SendAsync(request);
                    if (!response.IsSuccessStatusCode)
                        return null;

                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<Forecast>(content);
                }
            } catch (Exception ex)
            {
                await _logger.LogAsync(LogSeverity.Error, "Weather", ex.ToString());
            }
            return null;
        }
    }
}
