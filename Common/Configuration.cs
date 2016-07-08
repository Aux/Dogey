using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Common
{
    public class Configuration
    {
        public string DiscordToken { get; set; }
        public string GoogleToken { get; set; }
        public char Prefix { get; set; }
        public string Playing { get; set; }
        public string Avatar { get; set; }

        public Configuration FromFile(string file)
        {
            return JsonConvert.DeserializeObject<Configuration>(System.IO.File.ReadAllText(file));
        }

        public void ToFile(string file)
        {
            File.WriteAllText(file, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}