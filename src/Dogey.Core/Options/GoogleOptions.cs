using Newtonsoft.Json;

namespace Dogey
{
    public class GoogleOptions
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("custom_search_id")]
        public string CustomSearchEngineId { get; set; }
    }
}
