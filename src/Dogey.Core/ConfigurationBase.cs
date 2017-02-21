using Newtonsoft.Json;
using System;
using System.IO;

namespace Dogey
{
    public abstract class ConfigurationBase
    {
        [JsonIgnore]
        public static string FileName { get; private set; } = "config/config.json";
        
        public ConfigurationBase(string fileName)
        {
            FileName = fileName;
        }

        public void SaveJson()
        {
            string file = Path.Combine(AppContext.BaseDirectory, FileName);
            File.WriteAllText(file, ToJson());
        }
        
        public static T Load<T>() where T : ConfigurationBase
        {
            string file = Path.Combine(AppContext.BaseDirectory, FileName);
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(file));
        }
        
        public string ToJson()
            => JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}
