using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Dogey.Logging
{
    public class BotLoggerProvider : ILoggerProvider
    {
        private readonly LoggingOptions _options;

        public BotLoggerProvider(IConfiguration config)
        {
            _options = new LoggingOptions();
            config.Bind("logging", _options);
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new BotLogger(categoryName, _options);
        }

        public void Dispose()
        {
            return;
        }
    }
}
