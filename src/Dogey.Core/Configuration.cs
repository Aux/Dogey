using Discord;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Dogey
{
    public class Configuration
    {
        [JsonIgnore]
        public static string Name = "config/configuration.json";
        public string Prefix { get; set; } = "!";
        public AuthTokens Token { get; set; } = new AuthTokens();

        public void Save(string fileName = null)
        {
            var dir = fileName == null ? Name : fileName;
            string file = Path.Combine(AppContext.BaseDirectory, dir);
            File.WriteAllText(file, ToJson());
        }

        public static Configuration Load(string fileName = null)
        {
            var dir = fileName == null ? Name : fileName;
            string file = Path.Combine(AppContext.BaseDirectory, dir);
            return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(file));
        }
        
        public static void EnsureExists()
        {
            string file = Path.Combine(AppContext.BaseDirectory, Name);
            if (!File.Exists(file))
            {
                string path = Path.GetDirectoryName(file);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var config = new Configuration();

                PrettyConsole.Log(LogSeverity.Warning, "Dogey", "Please enter your token: ");
                string token = Console.ReadLine();

                config.Token.Discord = token;
                config.Save();
            }
            PrettyConsole.Log(LogSeverity.Info, "Dogey", "Configuration Loaded");
        }
        
        public string ToJson()
            => JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public class AuthTokens
    {
        public string Discord { get; set; } = "";
        public string Youtube { get; set; } = "";
    }
}
