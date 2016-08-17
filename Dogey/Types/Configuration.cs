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
        /// <summary> Path to the dll's root directory. </summary>
        [JsonIgnore]
        public readonly string appdir = AppContext.BaseDirectory;

        /// <summary> The prefix for all commands. </summary>
        public char Prefix { get; set; }
        /// <summary> The Id of users who have owner command access. </summary>
        public List<ulong> Owner { get; set; }
        /// <summary> The Id of the owner's guild. </summary>
        public ulong OwnerGuild { get; set; }
        /// <summary> The bot's oauth token. </summary>
        public Token Token { get; set; }

        public Configuration()
        {
            Prefix = '~';
            Owner = new List<ulong>();
            Token = new Token();
        }

        public Configuration FromFile(string file)
        {
            string loc = Path.Combine(appdir, file);
            return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(loc));
        }

        public void Save(string file)
        {
            string loc = Path.Combine(appdir, file);
            string json = ToJson();
            File.WriteAllText(loc, json);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}
