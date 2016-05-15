using Discord;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Modules.Chatlog
{
    public class Log
    {
        public static bool Exists(Server guild)
        {
            string serverFolder = $"servers\\{guild.Id}";
            string chatFile = $"{serverFolder}\\chatlog.doge";

            if (!Directory.Exists(serverFolder)) return false;
            if (!File.Exists(chatFile)) return false;

            return true;
        }

        public static void Create(Server guild)
        {
            string serverFolder = $"servers\\{guild.Id}";
            string chatFile = $"{serverFolder}\\chatlog.doge";

            if (!Directory.Exists(serverFolder)) Directory.CreateDirectory(serverFolder);
            if (!File.Exists(chatFile)) SQLiteConnection.CreateFile(chatFile);

            //SQLite stuff later
        }
    }
}
