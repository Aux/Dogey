using Newtonsoft.Json;

namespace Dogey
{
    public class ResponsiveOptions
    {
        public static readonly string[] DefaultTrueReplies = { "y", "ye", "yes", "yea", "yeah", "t", "true" };
        public static readonly string[] DefaultFalseReplies = { "n", "no", "na", "nah", "f", "false" };

        public ResponsiveOptions()
        {
            ExpireSeconds = 15;
            TrueReplies = DefaultTrueReplies;
            FalseReplies = DefaultFalseReplies;
        }

        [JsonProperty("expire_seconds")]
        public int ExpireSeconds { get; set; }
        [JsonProperty("true_replies")]
        public string[] TrueReplies { get; set; }
        [JsonProperty("false_replies")]
        public string[] FalseReplies { get; set; }
    }
}
