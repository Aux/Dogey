using Newtonsoft.Json;

namespace Dogey
{
    public class ForecastMeasurements
    {
        [JsonProperty("temp")]
        public double Temperature { get; set; }
        [JsonProperty("temp_min")]
        public double TemperatureMin { get; set; }
        [JsonProperty("temp_max")]
        public double TemperatureMax { get; set; }
        [JsonProperty("pressure")]
        public double Pressure { get; set; }
        [JsonProperty("humidity")]
        public double Humidity { get; set; }
    }
}
