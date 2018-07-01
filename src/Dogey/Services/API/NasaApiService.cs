using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestEase;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dogey
{
    [Header("User-Agent", "Dogey (https://github.com/Aux/Dogey)")]
    public interface INasaApi
    {
        [Get("planetary/apod")]
        Task<Apod> GetApodAsync([Query("api_key")]string token, [Query("date", Format = "yyyy-MM-dd")]DateTime? date = null);
    }

    public class NasaApiService
    {
        public const string ApiUrl = "https://api.nasa.gov/";

        public static HttpClient GetClient()
            => new HttpClient { BaseAddress = new Uri(ApiUrl) };

        private readonly ILogger<NasaApiService> _logger;
        private readonly RatelimitService _ratelimiter;
        private readonly IConfiguration _config;
        private readonly INasaApi _api;

        private string _apiKey;

        public NasaApiService(ILogger<NasaApiService> logger, IConfiguration config, RatelimitService ratelimiter, INasaApi api)
        {
            _logger = logger;
            _config = config;
            _ratelimiter = ratelimiter;
            _api = api;

            _apiKey = _config["tokens:nasa"];
            _ratelimiter.CreateInfo(nameof(NasaApiService), 1000, 60);
        }

        public async Task<Apod> GetApodAsync(DateTime? date = null)
        {
            if (_ratelimiter.IsRatelimited(nameof(NasaApiService))) return null;

            try
            {
                return await _api.GetApodAsync(_apiKey, date);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return null;
        }
    }
}