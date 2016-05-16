using Dogey.Config;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Utility
{
    public class Startup
    {
        /// <summary>
        /// Write the Dogey banner to the console.
        /// </summary>
        public static void DogeyBanner()
        {
            DogeyConsole.Write("┌──────────────────────┐", ConsoleColor.Yellow);
            DogeyConsole.Append($"│    ", ConsoleColor.Yellow);
            DogeyConsole.Append($"Dogey v{ Assembly.GetEntryAssembly().GetName().Version}");
            DogeyConsole.Write("    │", ConsoleColor.Yellow);
            DogeyConsole.Write("└──────────────────────┘\n", ConsoleColor.Yellow);
        }

        /// <summary>
        /// Check if config exists at "config\configuration.json".
        /// </summary>
        /// <returns></returns>
        public static bool ConfigExists()
        {
            if (File.Exists("config\\configuration.json")) { 
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// Create an empty configuration template.
        /// </summary>
        public static void CreateConfig()
        {
            DogeyConsole.Append("[Error] ", ConsoleColor.Red);
            DogeyConsole.Write("Configuration file not found, creating...");

            var config = new Configuration()
            {
                DefaultPrefix = '~',
                Discord = new DiscordCredential(),
                MySQL = new MySqlCredential(),
                Google = new GoogleCredential(),
                Twitch = new TwitchCredential(),
            };

            if (!Directory.Exists("config")) Directory.CreateDirectory("config");

            File.Create("config\\configuration.json").Close();
            File.WriteAllText("config\\configuration.json", JsonConvert.SerializeObject(config, Formatting.Indented));

            DogeyConsole.Append("[Error] ", ConsoleColor.Red);
            DogeyConsole.Append("Configuration created at ");
            DogeyConsole.Append("\"config\\configuration.json\"", ConsoleColor.Gray);
            DogeyConsole.Write(". Please fill in the required fields and restart Dogey.");
        }
    }
}
