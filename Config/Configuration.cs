using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Config
{
    public class Configuration
    {
        /// <summary>
        /// The default prefix character for all Dogey commands.
        /// </summary>
        public char DefaultPrefix { get; set; }
        /// <summary>
        /// Allow @Dogey to be used as a command identifier instead of the default prefix.
        /// </summary>
        public bool AllowMentionCommands { get; set; }
        /// <summary>
        /// Connection and Bot settings for the Dogey account.
        /// </summary>
        public DiscordCredential Discord { get; set; }
        /// <summary>
        /// Credentials for storing Dogey data in a MySQL database, instead of SQLite.
        /// </summary>
        public MySqlCredential MySQL { get; set; }
        /// <summary>
        /// Credentials for connecting to the Twitch API, required for module(s):
        /// TwitchModule
        /// </summary>
        public TwitchCredential Twitch { get; set; }
        /// <summary>
        /// Credentials for connecting to the Google API, required for module(s):
        /// GoogleModule
        /// YoutubeModule
        /// </summary>
        public GoogleCredential Google { get; set; }
        
        public bool AdminModule { get; set; }
        public bool BotModule { get; set; }
        public bool ChatlogModule { get; set; }
        public bool TwitchModule { get; set; }
        public bool CustomModule { get; set; }

        /// <summary>
        /// Dogey configuration object.
        /// </summary>
        /// <param name="file">Convert a json string to this object.</param>
        public Configuration()
        {
            DefaultPrefix = '~';
            AdminModule = true;
            BotModule = true;
            CustomModule = true;
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
        public Configuration FromFile(string file)
        {
            return JsonConvert.DeserializeObject<Configuration>(System.IO.File.ReadAllText(file));
        }
    }
}
