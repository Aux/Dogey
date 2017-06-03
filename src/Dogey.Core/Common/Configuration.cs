using Discord;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Dogey
{
    public class Configuration
    {
        [JsonIgnore]
        public static string FileName { get; private set; } = "config/config.json";

        public int RelatedTagsLimit { get; set; } = 3;
        public AuthTokens Token { get; set; } = new AuthTokens();
        public CustomSearchConfig CustomSearch { get; set; } = new CustomSearchConfig();
        
        public Configuration() : this("config/config.json") { }
        public Configuration(string fileName)
        {
            FileName = fileName;
        }
        
        public static void EnsureExists()
        {
            string file = Path.Combine(AppContext.BaseDirectory, FileName);
            if (!File.Exists(file))
            {
                string path = Path.GetDirectoryName(file);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var config = new Configuration();

                PrettyConsole.Log(LogSeverity.Warning, "Dogey", "Please enter your token: ");
                string token = Console.ReadLine();

                config.Token.Discord = token;
                config.SaveJson();
            }
            PrettyConsole.Log(LogSeverity.Info, "Dogey", "Configuration Loaded");
        }

        public void SaveJson()
        {
            string file = Path.Combine(AppContext.BaseDirectory, FileName);
            File.WriteAllText(file, ToJson());
        }

        public static Configuration Load()
        {
            string file = Path.Combine(AppContext.BaseDirectory, FileName);
            return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(file));
        }

        public string ToJson()
            => JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public class AuthTokens
    {
        public string Discord { get; set; } = "";
        public string Google { get; set; } = "";
        public string Github { get; set; } = "";
        public string Twitch { get; set; } = "";
    }

    public class CustomSearchConfig
    {
        public string Token { get; set; } = "";
        public string EngineId { get; set; } = "";
        public int ResultCount { get; set; } = 3;
    }
}
