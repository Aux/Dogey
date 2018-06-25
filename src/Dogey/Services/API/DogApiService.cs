using Discord;
using RestEase;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dogey
{
    [Header("User-Agent", "Dogey (https://github.com/Aux/Dogey)")]
    public interface IDogApi
    {
        [Get("dog.php")]
        Task<DogResponse> GetDogAsync(string id = null);
    }

    public class DogApiService
    {
        public const string ApiUrl = "https://api.thedogapi.co.uk/v2";

        public static HttpClient GetClient()
            => new HttpClient { BaseAddress = new Uri(ApiUrl) };

        private readonly RatelimitService _ratelimiter;
        private readonly LoggingService _logger;
        private readonly HttpClient _http;
        private readonly IDogApi _api;
        
        public DogApiService(RatelimitService ratelimiter, LoggingService logger, HttpClient http, IDogApi api)
        {
            _ratelimiter = ratelimiter;
            _logger = logger;
            _http = http;
            _api = api;
        }
        
        public async Task<DogData> GetDogAsync(string id = null)
        {
            if (_ratelimiter.IsRatelimited(nameof(DogApiService))) return null;
            
            try
            {
                var response = await _api.GetDogAsync(id);
                return response.Data.FirstOrDefault();
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
                using (var request = new HttpRequestMessage(HttpMethod.Get, data.ImageUrl))
                {
                    var response = await _http.SendAsync(request);
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
