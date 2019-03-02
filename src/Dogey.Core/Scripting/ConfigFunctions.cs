using System;
using Microsoft.Extensions.Configuration;
using Scriban.Runtime;

namespace Dogey.Scripting
{
    public class ConfigFunctions : ScriptObject
    {
        private readonly IConfiguration _config;

        public ConfigFunctions(IConfiguration config)
        {
            _config = config;

            this.Import("getconfig", new Func<string, string>((path) => _config[path]));
            this.Import("setconfig", new Action<string, string>((path, value) => _config[path] = value));
        }
    }
}
