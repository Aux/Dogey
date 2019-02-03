using System;
using System.IO;
using System.Threading.Tasks;
using Dogey.Config;
using Microsoft.Extensions.Configuration;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace Dogey
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(string[] args)
        {
            TryGenerateConfiguration();
            var builder = new ConfigurationBuilder()
             .SetBasePath(AppContext.BaseDirectory)
             .AddYamlFile("_config.yml");
            Configuration = builder.Build();
        }

        public static bool TryGenerateConfiguration()
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "_config.yml");
            if (File.Exists(filePath)) return false;

            var serializer = new SerializerBuilder()
                .WithNamingConvention(new UnderscoredNamingConvention())
                .Build();

            var yaml = serializer.Serialize(new AppOptions(true));
            File.WriteAllText(filePath, yaml);
            return true;
        }

        public async Task RunAsync()
        {
            await Task.Delay(-1);
        }
    }
}
