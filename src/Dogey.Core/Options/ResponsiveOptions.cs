using Newtonsoft.Json;

namespace Dogey
{
    public class ResponsiveOptions
    {
        public ResponsiveOptions()
        {
            DefaultExpireSeconds = 15;
        }

        [JsonProperty("expire_seconds")]
        public int DefaultExpireSeconds { get; set; }
    }
}
