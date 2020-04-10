using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace Dogey.Services
{
    public class LocaleService
    {
        public static string LocalePath = Path.Combine(AppContext.BaseDirectory, "common/locale");

        private readonly IConfiguration _config;
        private readonly ILogger<LocaleService> _logging;
        private readonly ConcurrentDictionary<string, IConfiguration> _locales;

        public LocaleService(IConfiguration config)
        {
            _config = config;
            _locales = new ConcurrentDictionary<string, IConfiguration>();
            var files = Directory.GetFiles(LocalePath, "*.yml");
            
            // Future: Only load actively used files; default & subscribed locales
            // Should I bother to validate contents?
            foreach (var file in files)
            {
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Path.Combine(AppContext.BaseDirectory, "common/locale"))
                    .AddYamlFile(file);
                var locale = builder.Build();
                if (!_locales.TryAdd(Path.GetFileNameWithoutExtension(file), locale))
                    _logging.LogError($"Unable to load locale file {Path.GetFileName(file)}");
            }
        }

        public string GetString(string id, string locale = null)
        {
            IConfiguration value;
            if (!_locales.TryGetValue(locale, out value))
                _locales.TryGetValue(_config["locale:default"], out value);
            return value[id];
        }
    }
}
