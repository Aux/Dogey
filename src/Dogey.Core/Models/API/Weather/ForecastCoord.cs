using Newtonsoft.Json;

namespace Dogey
{
    public class ForecastCoord
    {
        [JsonProperty("lon")]
        public double Longitude { get; set; }
        [JsonProperty("lat")]
        public double Latitude { get; set; }
    }
}
