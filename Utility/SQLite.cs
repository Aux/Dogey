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
    }
}
