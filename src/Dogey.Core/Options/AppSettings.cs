using Newtonsoft.Json;

namespace Dogey
{
    public class AppSettings
    {
        public AppSettings() { }
        public AppSettings(bool isgenerating)
        {
            Tokens = new TokenOptions();
            Google = new GoogleOptions();
            FileLogger = new FileLoggerOptions();
        }

        [JsonProperty("tokens")]
        public TokenOptions Tokens { get; set; }
        [JsonProperty("google")]
        public GoogleOptions Google { get; set; }
        [JsonProperty("filelogger")]
        public FileLoggerOptions FileLogger { get; set; }
    }
}
