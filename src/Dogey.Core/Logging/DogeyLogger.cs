using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;

namespace Dogey
{
    public class DogeyLogger : ILogger
    {
        private readonly string _categoryName;
        private readonly string _outputDirectory;
        private readonly string _dateTimeFormat;
        private readonly int _maxFileSizeKb;

        private int _duplicateLogFileCount = 0;

        private string _logFile => Path.Combine(_outputDirectory, GetFileName(DateTime.UtcNow));

        public DogeyLogger(string categoryName, string outputDirectory, string dateTimeFormat, int maxFileSizeKb)
        {
            _categoryName = categoryName;
            _outputDirectory = outputDirectory;
            _dateTimeFormat = dateTimeFormat;
            _maxFileSizeKb = maxFileSizeKb;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public string GetFileName(DateTime dateTime)
        {
            var builder = new StringBuilder(dateTime.ToString(_dateTimeFormat));
            if (_duplicateLogFileCount != 0)
                builder.Append($" ({_duplicateLogFileCount})");
            builder.Append(".txt");
            return builder.ToString();
        }
        
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string logText = $"{DateTime.UtcNow.ToString("hh:mm:ss")} {logLevel}: {_categoryName}[{eventId.Id}]: {formatter(state, exception)}\n";

            if (!Directory.Exists(_outputDirectory))
                Directory.CreateDirectory(_outputDirectory);

            var fileInfo = new FileInfo(_logFile);
            if (!fileInfo.Exists)
                fileInfo.Create().Dispose();
            if (fileInfo.Length + logText.Length > _maxFileSizeKb * 1000)
                _duplicateLogFileCount++;
            
            File.AppendAllText(_logFile, logText);
            Console.WriteLine(logText);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }
    }
}
