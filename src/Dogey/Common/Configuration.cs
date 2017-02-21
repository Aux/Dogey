using Discord;
using System;
using System.IO;

namespace Dogey
{
    public class Configuration : ConfigurationBase
    {
        public DbMode Database { get; set; } = DbMode.SQLite;
        public string Prefix { get; set; } = "!";
        public AuthTokens Token { get; set; } = new AuthTokens();
        
        public Configuration() : base("config/config.json") { }

        public static Configuration Load()
            => Load<Configuration>();
        
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
    }

    public class AuthTokens
    {
        public string Discord { get; set; } = "";
        public string Youtube { get; set; } = "";
    }
}
