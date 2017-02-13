using Discord;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Dogey
{
    public class Configuration
    {
        [JsonIgnore]
        public static readonly string appdir = AppContext.BaseDirectory;

        public string Prefix { get; set; } = "!";
        public Tokens Token { get; set; } = new Tokens();

        public void Save(string dir = "data/configuration.json")
        {
            string file = Path.Combine(appdir, dir);
            File.WriteAllText(file, ToJson());
        }

        public static Configuration Load(string dir = "data/configuration.json")
        {
            string file = Path.Combine(appdir, dir);
            return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(file));
        }
        
        public static void EnsureExists()
        {
            if (!Directory.Exists(Path.Combine(appdir, "data")))
                Directory.CreateDirectory(Path.Combine(appdir, "data"));

            string loc = Path.Combine(appdir, "data/configuration.json");

            if (!File.Exists(loc))
            {
                var config = new Configuration();

                PrettyConsole.Log(LogSeverity.Warning, "Config", "Please enter your token: ");
                string token = Console.ReadLine();

                config.Token.Discord = token;
                config.Save();



                Console.ReadKey();
            }
            PrettyConsole.Log(LogSeverity.Info, "Dogey", "Configuration Loaded");
        }
        
        public string ToJson()
            => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
    
    public class Tokens
    {
        public string Beam { get; set; } = "";
        public string Blizzard { get; set; } = "";
        public string Discord { get; set; } = "";
        public string Github { get; set; } = "";
        public string Google { get; set; } = "";
        public string Riot { get; set; } = "";
        public string Steam { get; set; } = "";
        public string Twitch { get; set; } = "";
    }
}
