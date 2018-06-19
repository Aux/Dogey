using Newtonsoft.Json;

namespace Dogey
{
    public class DogResponse
    {
        [JsonProperty("response_code")]
        public string ResponseCode { get; set; }
        [JsonProperty("api_version")]
        public string ApiVersion { get; set; }
        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("count")]
        public int Count { get; set; }
        [JsonProperty("data")]
        public DogData[] Data { get; set; }
    }
}
