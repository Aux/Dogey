using System;
using Microsoft.Extensions.Configuration;
using Scriban.Runtime;

namespace Dogey.Scripting
{
    public class ConfigFunctions : ScriptObject
    {
        public ConfigFunctions(IConfiguration config)
        {
            var configFuncs = new ScriptObject();
            configFuncs.Import("getstring", new Func<string, string>((path) => config[path]));
            configFuncs.Import("getnumber", new Func<string, float?>((path) => 
            {
                if (float.TryParse(config[path], out float value))
                    return value;
                return null;
            }));

            SetValue("config", configFuncs, true);
        }
    }
}
