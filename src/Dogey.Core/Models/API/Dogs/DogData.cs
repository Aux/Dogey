using Newtonsoft.Json;
using System;

namespace Dogey
{
    public class DogData
    {
        [JsonProperty("id")]
        public string Id { get; set; }
        [JsonProperty("time")]
        public DateTime Timestamp { get; set; }
        [JsonProperty("url")]
        public string ImageUrl { get; set; }
        [JsonProperty("format")]
        public string ImageFormat { get; set; }
        [JsonProperty("verified")]
        public int IsVerified { get; set; }
        [JsonProperty("checked")]
        public int IsChecked { get; set; }
    }
}
