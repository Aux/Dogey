using Newtonsoft.Json;

namespace Dogey
{
    public class TokenOptions
    {
        [JsonProperty("discord")]
        public string Discord { get; set; }
        [JsonProperty("github")]
        public string GitHub { get; set; }
        [JsonProperty("openweather")]
        public string OpenWeather { get; set; }
    }
}
