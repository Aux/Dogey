using Discord;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dogey
{
    public class WeatherApiService : ApiServiceBase
    {
        public const string ApiUrl = "https://api.openweathermap.org/data/2.5/weather";
        public const string IconUrl = "http://openweathermap.org/img/w/{0}.png";

        private readonly IConfiguration _config;
        private readonly LoggingService _logger;
        private readonly RootController _root;

        private string _apiKey;

        public WeatherApiService(IConfiguration config, LoggingService logger, RootController root, HttpClient http)
            : base (http, 60)
        {
            _config = config;
            _logger = logger;
            _root = root;

            _apiKey = _config["tokens:openweather"];
        }

        public static string GetIconUrl(string id)
            => string.Format(IconUrl, id);
        
        public async Task<Forecast> GetForecastAsync(string city, WeatherUnit unit = WeatherUnit.Metric)
        {
            if (IsRatelimited()) return null;
            
            var builder = new StringBuilder();
            builder.Append("?q=" + city);
            builder.Append("&unit=" + unit.ToString().ToLower());
            builder.Append("&appid=" + _apiKey);

            try
            {
                return await SendAsync<Forecast>(HttpMethod.Get, ApiUrl + ApiUrl + builder.ToString());
            }
            catch (Exception ex)
            {
                await _logger.LogAsync(LogSeverity.Error, "Weather", ex.ToString());
            }
            return null;
        }
    }
}
