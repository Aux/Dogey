using Discord;
using Dogey.Common;
using Dogey.Common.Modules;
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
            string bannerText = $"Dogey v{Assembly.GetEntryAssembly().GetName().Version}";
            
            DogeyConsole.Write($"┌────────────{new string('─', bannerText.Count())}────────────┐", ConsoleColor.Yellow);
            DogeyConsole.Append("│            ", ConsoleColor.Yellow);
            DogeyConsole.Append(bannerText);
            DogeyConsole.Write("            │", ConsoleColor.Yellow);
            DogeyConsole.Write($"└────────────{new string('─', bannerText.Count())}────────────┘\n", ConsoleColor.Yellow);
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
            DogeyConsole.Log(LogSeverity.Error, "Startup", "Configuration file not found.");

            var config = new Configuration()
            {
                Prefix = '~'
            };

            if (!Directory.Exists("config")) Directory.CreateDirectory("config");

            File.Create("config\\configuration.json").Close();
            File.WriteAllText("config\\configuration.json", JsonConvert.SerializeObject(config, Formatting.Indented));

            DogeyConsole.Log(LogSeverity.Error, "Startup", "Configuration file created at 'config\\configuration.json', " +
                "please enter your bot token and restart Dogey.");
        }
        
    }
}
