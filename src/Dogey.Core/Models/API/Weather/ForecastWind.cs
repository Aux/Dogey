using Newtonsoft.Json;

namespace Dogey
{
    public class ForecastWind
    {
        [JsonProperty("speed")]
        public double Speed { get; set; }
        [JsonProperty("degree")]
        public int Degree { get; set; }
    }
}
