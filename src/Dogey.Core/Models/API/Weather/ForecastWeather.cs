using Newtonsoft.Json;

namespace Dogey
{
    public class ForecastWeather
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("main")]
        public string Main { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("icon")]
        public string IconId { get; set; }
    }
}
