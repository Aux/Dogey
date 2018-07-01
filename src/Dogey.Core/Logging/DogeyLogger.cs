using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Dogey
{
    public class DogeyLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly string _outputDirectory;
        private readonly string _fileNameFormat;

        private string _logFile => Path.Combine(_outputDirectory, GetFileName(DateTime.UtcNow));

        public string GetFileName(DateTime dateTime)
            => dateTime.ToString(_fileNameFormat) + ".txt";

        public DogeyLogger(string categoryName, string outputDirectory, string fileNameFormat)
        {
            _categoryName = categoryName;
            _outputDirectory = outputDirectory;
            _fileNameFormat = fileNameFormat;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!Directory.Exists(_outputDirectory))
                Directory.CreateDirectory(_outputDirectory);
            if (!File.Exists(_logFile))
                File.Create(_logFile).Dispose();

            string logText = $"{DateTime.UtcNow.ToString("hh:mm:ss")} {logLevel}: {_categoryName}[{eventId.Id}]: {formatter(state, exception)}";
            File.AppendAllText(_logFile, logText + "\n");
            Console.WriteLine(logText);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }
    }
}
