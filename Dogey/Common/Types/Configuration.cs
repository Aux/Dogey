using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Types
{
    public class Configuration
    {
        [JsonIgnore]
        public static readonly string appdir = AppContext.BaseDirectory;

        public string DefaultPrefix { get; set; } = "~";
        public bool IsSelfbot { get; set; }
        public List<ulong> Owners { get; set; } = new List<ulong>();
        public Tokens Token { get; set; } = new Tokens();

        public void SaveFile(string file)
        {
            string loc = Path.Combine(appdir, file);
            string json = ToJson();
            File.WriteAllText(loc, json);
        }

        public static Configuration LoadFile(string file)
        {
            string loc = Path.Combine(appdir, file);
            return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(loc));
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }

    public class Tokens
    {
        public string Discord { get; set; } = "";
        public string Google { get; set; } = "";
    }
}
