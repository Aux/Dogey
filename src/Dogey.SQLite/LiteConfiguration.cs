using System;
using System.IO;

namespace Dogey.SQLite
{
    public class LiteConfiguration : ConfigurationBase
    {
        public int RelatedTagsLimit { get; set; } = 3;

        public LiteConfiguration() : base("config/sqlite_config.json") { }

        public static LiteConfiguration Load()
        {
            EnsureExists();
            return Load<LiteConfiguration>();
        }

        public static void EnsureExists()
        {
            string file = Path.Combine(AppContext.BaseDirectory, FileName);
            if (!File.Exists(file))
            {
                string path = Path.GetDirectoryName(file);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                var config = new LiteConfiguration();
                config.SaveJson();
            }
        }
    }
}
