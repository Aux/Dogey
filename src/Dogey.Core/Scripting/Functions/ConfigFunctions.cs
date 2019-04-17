using System;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Scriban.Runtime;

namespace Dogey.Scripting
{
    public class ConfigFunctions : ScriptObject
    {
        public static readonly string[] BlacklistedOptions = new[]
        {
            "discord:token"
        };

        public ConfigFunctions(IConfiguration config)
        {
            var configFuncs = new ScriptObject();
            configFuncs.Import("getstring", new Func<string, string>((path) => 
            {
                if (BlacklistedOptions.Any(x => x == path)) return null;
                return config[path];
            }));
            configFuncs.Import("getnumber", new Func<string, float?>((path) =>
            {
                if (BlacklistedOptions.Any(x => x == path)) return null;
                if (float.TryParse(config[path], out float value))
                    return value;
                return null;
            }));

            SetValue("config", configFuncs, true);
        }
    }
}
