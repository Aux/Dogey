using Discord;
using Microsoft.Extensions.Configuration;
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
        public string Name => nameof(WeatherApiService);

        public static HttpClient GetClient()
            => new HttpClient { BaseAddress = new Uri(ApiUrl) };

        private readonly RatelimitService _ratelimiter;
        private readonly IConfiguration _config;
        private readonly LoggingService _logger;
        private readonly IWeatherApi _api;

        private string _apiKey;

        public WeatherApiService(RatelimitService ratelimiter, IConfiguration config, LoggingService logger, HttpClient http)
        {
            _ratelimiter = ratelimiter;
            _config = config;
            _logger = logger;
            _api = RestClient.For<IWeatherApi>(ApiUrl);

            _apiKey = _config["tokens:openweather"];
        }

        public static string GetIconUrl(string id)
            => string.Format(IconUrl, id);
        
        public async Task<Forecast> GetForecastAsync(string city, WeatherUnit unit = WeatherUnit.Metric)
        {
            if (_ratelimiter.IsRatelimited(Name)) return null;
            
            try
            {
                return await _api.GetForecastAsync(city, unit.ToString().ToLower(), _apiKey);
            }
            catch (Exception ex)
            {
                await _logger.LogAsync(LogSeverity.Error, Name, ex.ToString());
            }
            return null;
        }
    }
}
