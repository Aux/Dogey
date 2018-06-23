using Discord;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dogey
{
    public class DogApiService : ApiServiceBase
    {
        public const string ApiUrl = "https://api.thedogapi.co.uk/v2/dog.php";
        public const string ImageUrl = "https://i.thedogapi.co.uk/{0}.{1}";

        private readonly IConfiguration _config;
        private readonly LoggingService _logger;
        
        public DogApiService(IConfiguration config, LoggingService logger, HttpClient http)
            : base(http, 10)
        {
            _config = config;
            _logger = logger;
        }

        public string GetImageUrl(string id, string format = "jpg")
            => string.Format(ImageUrl, id, format);
        
        public async Task<DogData> GetDogAsync(string id = null)
        {
            if (IsRatelimited()) return null;

            string query = string.IsNullOrWhiteSpace(id) ? null : query = "?id=" + id;
            
            try
            {
                var response = await SendAsync<DogResponse>(HttpMethod.Get, ApiUrl + query);
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
                return await SendAsync(HttpMethod.Get, data.ImageUrl);
            }
            catch (Exception ex)
            {
                await _logger.LogAsync(LogSeverity.Error, "Dog", ex.ToString());
            }
            return null;
        }
    }
}
