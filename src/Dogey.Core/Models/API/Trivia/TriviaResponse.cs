using Newtonsoft.Json;

namespace Dogey
{
    public class TriviaResponse
    {
        [JsonProperty("response_code")]
        public TriviaReponseCode Code { get; set; }
        [JsonProperty("response_message")]
        public string Message { get; set; }
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("results")]
        public TriviaQuestion[] Results { get; set; }
    }
}
