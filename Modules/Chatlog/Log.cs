using Discord;
using Dogey.Utility;
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
            
            string msgTable = @"CREATE TABLE msgs (MsgID TINYINT(18) PRIMARY KEY, ChannelID TINYINT(18) NOT NULL, Timestamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP, " + 
                               " UserID TINYINT(18) NOT NULL, Username VARCHAR(500) NOT NULL, Message VARCHAR(5000) NOT NULL, Attachment BLOB DEFAULT NULL);";
            
            using (var sql = SQLite.Connect(chatFile))
            using (var sqlcmd = new SQLiteCommand(msgTable, sql))
            {
                sqlcmd.ExecuteNonQuery();
            }
        }

        public static void Message(Server guild, Message msg)
        {
            string serverFolder = $"servers\\{guild.Id}";
            string chatFile = $"{serverFolder}\\chatlog.doge";
            
            string msgWrite = @"INSERT INTO msgs (MsgID, ChannelID, UserID , Username, Message, Attachment) VALUES (@1,@2,@3,@4,@5,@6);";

            using (var sql = SQLite.Connect(chatFile))
            using (var sqlcmd = new SQLiteCommand(msgWrite, sql))
            {
                string atchUrl = null;
                foreach (Discord.Message.Attachment atch in msg.Attachments) atchUrl += atch.Url + " ";

                sqlcmd.Parameters.AddWithValue("@1", msg.Id);
                sqlcmd.Parameters.AddWithValue("@2", msg.Channel.Id);
                sqlcmd.Parameters.AddWithValue("@3", msg.User.Id);
                sqlcmd.Parameters.AddWithValue("@4", msg.User.Name);
                sqlcmd.Parameters.AddWithValue("@5", msg.RawText);
                sqlcmd.Parameters.AddWithValue("@6", atchUrl);
                sqlcmd.ExecuteNonQuery();
            }
        }
    }
}
