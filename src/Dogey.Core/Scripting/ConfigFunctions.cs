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
            configFuncs.Import("get", new Func<string, string>((path) => config[path]));
            configFuncs.Import("set", new Action<string, string>((path, value) => config[path] = value));

            SetValue("config", configFuncs, true);
        }
    }
}
