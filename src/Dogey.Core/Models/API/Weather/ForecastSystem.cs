using Newtonsoft.Json;

namespace Dogey
{
    public class ForecastSystem
    {
        //[JsonProperty("type")]
        //public int Type { get; set; }
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("message")]
        public double Message { get; set; }
        [JsonProperty("country")]
        public string CountryCode { get; set; }
        [JsonProperty("sunrise")]
        public long SunriseAtTicks { get; set; }
        [JsonProperty("sunset")]
        public long SunsetAtTicks { get; set; }
    }
}
