using Microsoft.Extensions.Logging;
using RestEase;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Dogey
{
    [Header("User-Agent", "Dogey (https://github.com/Aux/Dogey)")]
    [Header("Content-Type", "text")]
    public interface INumbersApi
    {
        [Get("random/{type}")]
        Task<HttpResponseMessage> GetRandomAsync([Path("type")]string type);
        [Get("{date}/date")]
        Task<HttpResponseMessage> GetDateAsync([Path("date", Format = "dd/MM")]DateTime date);
        [Get("{number}/{type}")]
        Task<HttpResponseMessage> GetNumberAsync([Path("number")]int number, [Path("type")]string type);
    }

    public class NumbersApiService
    {
        public const string ApiUrl = "http://numbersapi.com";

        public static HttpClient GetClient()
            => new HttpClient { BaseAddress = new Uri(ApiUrl) };

        private readonly ILogger<NumbersApiService> _logger;
        private readonly RatelimitService _ratelimiter;
        private readonly INumbersApi _api;

        public NumbersApiService(ILogger<NumbersApiService> logger, RatelimitService ratelimiter, INumbersApi api)
        {
            _logger = logger;
            _ratelimiter = ratelimiter;
            _api = api;
        }

        private async Task<string> GetContentStringAsync(HttpResponseMessage response)
        {
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetRandomAsync(NumberType type)
        {
            if (_ratelimiter.IsRatelimited(nameof(NumbersApiService))) return null;

            try
            {
                var response = await _api.GetRandomAsync(type.ToString().ToLower());
                return await GetContentStringAsync(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return null;
        }
        
        public async Task<string> GetDateAsync(DateTime date)
        {
            if (_ratelimiter.IsRatelimited(nameof(NumbersApiService))) return null;

            try
            {
                var response = await _api.GetDateAsync(date);
                return await GetContentStringAsync(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return null;
        }

        public async Task<string> GetNumberAsync(int number, NumberType type)
        {
            if (_ratelimiter.IsRatelimited(nameof(NumbersApiService))) return null;

            try
            {
                var response = await _api.GetNumberAsync(number, type.ToString().ToLower());
                return await GetContentStringAsync(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            return null;
        }
    }
}
