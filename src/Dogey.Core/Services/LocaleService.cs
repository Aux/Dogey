using FormatWith;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Dogey.Services
{
    public class LocaleService
    {
        public static string LocalePath = Path.Combine(AppContext.BaseDirectory, "common/locale");

        public string DefaultLocaleId => _config["locale:default"];
        public ICollection<string> LocaleIds => _locales.Keys;

        private readonly IConfiguration _config;
        private readonly ILogger<LocaleService> _logging;
        private readonly ConcurrentDictionary<string, IConfiguration> _locales;

        public LocaleService(IConfiguration config, ILogger<LocaleService> logging)
        {
            _config = config;
            _logging = logging;
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
                else
                    _logging.LogInformation($"Loaded locale file {Path.GetFileName(file)}");
            }
        }

        public KeyValuePair<string, IConfiguration> GetLocale(string id)
            => _locales.AsEnumerable().SingleOrDefault(x => x.Key == id);
        public KeyValuePair<string, IConfiguration> GetDefaultLocale()
            => GetLocale(DefaultLocaleId);
        
        public string GetString(string id, object obj, string locale = null)
        {
            locale = locale == null ? DefaultLocaleId : locale;
            IConfiguration value;
            if (!_locales.TryGetValue(locale, out value))
                _locales.TryGetValue(DefaultLocaleId, out value);
            return value[id].FormatWith(obj, MissingKeyBehaviour.Ignore);
        }

        public string GetString(string id, Dictionary<string, object> obj, string locale = null)
        {
            locale = locale == null ? DefaultLocaleId : locale;
            IConfiguration value;
            if (!_locales.TryGetValue(locale, out value))
                _locales.TryGetValue(DefaultLocaleId, out value);
            return value[id].FormatWith(obj, MissingKeyBehaviour.Ignore);
        }
    }
}
