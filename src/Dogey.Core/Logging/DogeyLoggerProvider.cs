using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Dogey
{
    public class DogeyLoggerProvider : ILoggerProvider
    {
        private readonly FileLoggerOptions _options;

        public DogeyLoggerProvider(FileLoggerOptions options)
        {
            _options = options;
        }

        public ILogger CreateLogger(string categoryName)
        {
            string outputDirectory = _options.UseRelativeOutput 
                ? Path.Combine(AppContext.BaseDirectory, _options.OutputDirectory) 
                : _options.OutputDirectory;

            return new DogeyLogger(categoryName, outputDirectory, _options.FileNameFormat);
        }

        public void Dispose()
        {
            return;
        }
    }
}
