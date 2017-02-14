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
        public string Token { get; set; } = null;

        public void Save(string fileName = null)
        {
            var dir = fileName == null ? Name : fileName;
            string file = Path.Combine(AppContext.BaseDirectory, dir);
            File.WriteAllText(file, ToJson());
        }

        public static T Load<T>(string fileName = null)
        {
            var dir = fileName == null ? Name : fileName;
            string file = Path.Combine(AppContext.BaseDirectory, dir);
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(file));
        }
        
        public static void EnsureExists()
        {
            string loc = Path.Combine(AppContext.BaseDirectory, Name);

            if (!File.Exists(loc))
            {
                var config = new Configuration();

                PrettyConsole.Log(LogSeverity.Warning, "Dogey", "Please enter your token: ");
                string token = Console.ReadLine();

                config.Token = token;
                config.Save();
            }
            PrettyConsole.Log(LogSeverity.Info, "Dogey", "Configuration Loaded");
        }
        
        public string ToJson()
            => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
