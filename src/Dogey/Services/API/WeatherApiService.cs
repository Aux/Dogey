using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestEase;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dogey
{
    [Header("User-Agent", "Dogey (https://github.com/Aux/Dogey)")]
    public interface IWeatherApi
    {
        [Get("weather")]
        Task<Forecast> GetForecastAsync(string q, string unit, string appid);
    }

    public class WeatherApiService
    {
        public const string ApiUrl = "https://api.openweathermap.org/data/2.5";
        public const string IconUrl = "http://openweathermap.org/img/w/{0}.png";

        public static HttpClient GetClient()
            => new HttpClient { BaseAddress = new Uri(ApiUrl) };

        private readonly ILogger<WeatherApiService> _logger;
        private readonly RatelimitService _ratelimiter;
        private readonly IConfiguration _config;
        private readonly IWeatherApi _api;

        private string _apiKey;

        public WeatherApiService(ILogger<WeatherApiService> logger, RatelimitService ratelimiter, IConfiguration config, HttpClient http)
        {
            _logger = logger;
            _ratelimiter = ratelimiter;
            _config = config;
            _api = RestClient.For<IWeatherApi>(ApiUrl);

            _apiKey = _config["tokens:openweather"];
        }

        public static string GetIconUrl(string id)
            => string.Format(IconUrl, id);
        
        public async Task<Forecast> GetForecastAsync(string city, WeatherUnit unit = WeatherUnit.Metric)
        {
            if (_ratelimiter.IsRatelimited(nameof(WeatherApiService))) return null;
            
            try
            {
                return await _api.GetForecastAsync(city, unit.ToString().ToLower(), _apiKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return null;
        }
    }
}
