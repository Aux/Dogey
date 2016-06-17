using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Common
{
    public class Configuration
    {
        public string Token { get; set; }
        public char Prefix { get; set; }
        public string Playing { get; set; }

        public Configuration FromFile(string file)
        {
            return JsonConvert.DeserializeObject<Configuration>(System.IO.File.ReadAllText(file));
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}