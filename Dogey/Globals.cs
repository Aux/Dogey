using Discord;
using Discord.Commands;
using Dogey.Models;
using Dogey.Tools;
using Dogey.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey
{
    public static class Globals
    {
        public static Configuration Config { get; set; }
        public static CommandService CommandService { get; set; }
        
        public static void EnsureConfigExists()
        {
            if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "data")))
                Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "data"));

            string loc = Path.Combine(AppContext.BaseDirectory, "data/configuration.json");

            if (!File.Exists(loc))
            {
                Config = new Configuration();
                Config.SaveFile(loc);

                DogeyConsole.Log(LogSeverity.Error,
                    "[Startup]",
                    "The configuration file has been created at 'data\\configuration.json', " +
                    "please\n enter your information and restart Dogey.");
                DogeyConsole.NewLine("Press any key to continue...");

                Console.ReadKey();
                Environment.Exit(0);
            }
        }

        public static void EnsureDbExists()
        {
            using (var db = new DataContext())
            {
                db.Database.EnsureCreated();
            }
            DogeyConsole.Log(LogSeverity.Info, "Dogey", "Database loaded.");
        }

        public static void LoadConfig()
        {
            Config = Configuration.LoadFile(@"data\configuration.json");
            DogeyConsole.Log(LogSeverity.Info, "Dogey", "Configuration loaded.");
        }
    }
}
