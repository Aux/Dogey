using Newtonsoft.Json;

namespace Dogey
{
    public class Forecast
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("visibility")]
        public ulong Visibility { get; set; }
        [JsonProperty("dt")]
        public long DatetimeTicks { get; set; }

        [JsonProperty("coord")]
        public ForecastCoord Coordinates { get; set; }
        [JsonProperty("weather")]
        public ForecastWeather[] Weather { get; set; }
        [JsonProperty("main")]
        public ForecastMeasurements Measurements { get; set; }
        [JsonProperty("wind")]
        public ForecastWind Wind { get; set; }
        [JsonProperty("sys")]
        public ForecastSystem System { get; set; }
    }
}
