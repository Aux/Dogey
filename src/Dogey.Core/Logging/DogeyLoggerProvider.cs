using System;
using System.IO;
using Dogey.Config;
using Microsoft.Extensions.Logging;

namespace Dogey.Logging
{
    public class DogeyLoggerProvider : ILoggerProvider
    {
        private readonly LoggingOptions _options;

        public DogeyLoggerProvider(LoggingOptions options)
        {
            _options = options;
        }

        public ILogger CreateLogger(string categoryName)
        {
            string outputDirectory = _options.UseRelativeOutput
                ? Path.Combine(AppContext.BaseDirectory, _options.OutputDirectory)
                : _options.OutputDirectory;

            return new DogeyLogger(categoryName, outputDirectory, _options.DateTimeFormat, _options.MaxFileSizeKb);
        }

        public void Dispose()
        {
            return;
        }
    }
}
