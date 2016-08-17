using Discord;
using Dogey.Types;
using Dogey.Utilities;
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

        public static bool ConfigExists()
        {
            if (!Directory.Exists(Path.Combine(AppContext.BaseDirectory, "data")))
                Directory.CreateDirectory(Path.Combine(AppContext.BaseDirectory, "data"));

            string loc = Path.Combine(AppContext.BaseDirectory, @"data\configuration.json");

            return File.Exists(loc);
        }

        public static void CreateConfig()
        {
            string loc = Path.Combine(AppContext.BaseDirectory, @"data\configuration.json");

            Config = new Configuration();
            Config.Save(loc);

            DogeyConsole.Log(LogSeverity.Error,
                "[Startup]",
                "The configuration file has been created at 'data\\configuration.json', " +
                "please\n enter your information and restart Dogey.");
            DogeyConsole.NewLine("Press any key to continue...");
        }
    }
}
