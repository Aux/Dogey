using Discord;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Utility
{
    public class SQLite
    {
        public static SQLiteConnection Connect(string file)
        {
            string connection = $"Data Source={file};Version=3;";
            
            var SQLite = new SQLiteConnection(connection);
            SQLite.Open();

            return SQLite;
        }

        public static string LastMsgId(Server guild, ulong UserID)
        {
            string serverFolder = $"servers\\{guild.Id}";
            string chatFile = $"{serverFolder}\\chatlog.doge";

            string cmd = $"SELECT MsgID FROM msgs WHERE UserID = {UserID} ORDER BY Timestamp DESC LIMIT 1";
            using (var sql = SQLite.Connect(chatFile))
            using (var sqlcmd = new SQLiteCommand(cmd, sql))
            {
                return sqlcmd.ExecuteScalar().ToString();
            }
        }
    }
}
