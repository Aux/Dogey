using Dogey.Modules;
using Dogey.Scripting.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Dogey.Scripting
{
    public class ScriptingService
    {
        private readonly ILogger<ScriptingService> _logger;
        private readonly IConfiguration _config;

        private Dictionary<string, ScriptProviderBase> _providers;

        public ScriptingService(
            ILogger<ScriptingService> logger,
            IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _providers = new Dictionary<string, ScriptProviderBase>();

            if (bool.TryParse(_config["scripting:text"], out bool _))
            {
                var text = new TextScriptProvider();
                _providers.Add(text.ScriptFileExtension, text);
                _logger.LogInformation($"Started {nameof(TextScriptProvider)} with {GetScriptFilePaths(text.ScriptFileExtension).Count()} scripts");
            }
            if (bool.TryParse(_config["scripting:scriban"], out bool _))
            {
                var scriban = new ScribanScriptProvider(config);
                _providers.Add(scriban.ScriptFileExtension, scriban);
                _logger.LogInformation($"Started {nameof(ScribanScriptProvider)} with {GetScriptFilePaths(scriban.ScriptFileExtension).Count()} scripts");
            }
            if (bool.TryParse(_config["scripting:lua"], out bool _))
            {
                var lua = new LuaScriptProvider(config);
                _providers.Add(lua.ScriptFileExtension, lua);
                _logger.LogInformation($"Started {nameof(LuaScriptProvider)} with {GetScriptFilePaths(lua.ScriptFileExtension).Count()} scripts");
            }
        }

        public static string GetScriptDirectory()
            => Path.Combine(AppContext.BaseDirectory, "scripts");
        public static string GetRelativePath(string filePath)
        {
            return "../" + new Uri(GetScriptDirectory()).MakeRelativeUri(new Uri(filePath)).OriginalString;
        }

        public IEnumerable<string> GetScriptFilePaths(string extension = null)
            => Directory.GetFiles(GetScriptDirectory(), $"*.{extension ?? "*"}", SearchOption.AllDirectories);
        public string GetScriptPath(string name, string extension = null)
            => Directory.GetFiles(GetScriptDirectory(), $"{name.ToLower()}.{extension ?? "*"}", SearchOption.AllDirectories).FirstOrDefault();

        public bool ScriptExists(string name, string extension = null)
        {
            foreach (var provider in _providers.Where(x => extension == null ? true : x.Key == extension.ToLower()))
                if (GetScriptPath(name, provider.Key) != null) return true;
            return false;
        }

        public bool TryExecuteScript(string name, DogeyCommandContext context, out string result)
        {
            result = default;

            var scriptPath = GetScriptPath(name);
            if (scriptPath == null)
                return false;
            
            var scriptInfo = new FileInfo(scriptPath);
            if (!_providers.TryGetValue(scriptInfo.Extension.ToLower().Substring(1), out var provider))
            {
                result = $"Unable to find a provider for the extension `{scriptInfo.Extension}`";
                return true;
            }

            var timer = Stopwatch.StartNew();
            result = provider.Execute(scriptInfo.FullName, context);
            timer.Stop();

            _logger.LogInformation($"Executed `{GetRelativePath(scriptInfo.FullName)}` in {timer.ElapsedMilliseconds}ms");
            return true;
        }

        public string ExecuteText(string language, string content, DogeyCommandContext context)
        {
            if (!_providers.TryGetValue(language, out var provider))
                return $"Unable to find a provider for the extension `{language}`";
            return provider.ExecuteText(content, context);
        }
    }
}
