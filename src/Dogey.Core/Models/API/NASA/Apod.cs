using Newtonsoft.Json;
using System;

namespace Dogey
{
    public class Apod
    {
        [JsonProperty("date")]
        public DateTime Date { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("explanation")]
        public string Explanation { get; set; }
        [JsonProperty("hdurl")]
        public string HDImageUrl { get; set; }
        [JsonProperty("url")]
        public string ImageUrl { get; set; }

        [JsonProperty("media_type")]
        public string MediaType { get; set; }
        [JsonProperty("service_version")]
        public string Version { get; set; }
    }
}
