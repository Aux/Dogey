using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Dogey.Logging
{
    public class DogeyLogMessage
    {
        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public LogLevel LogLevel { get; set; }
        public string SourceName { get; set; }
        public string Content { get; set; }

        public string GetTimestamp() => Timestamp.ToString("hh:mm:ss");
        public string GetShortLogLevel() => LogLevel.ToString().Substring(0, 4);
        public override string ToString() => $"{GetTimestamp()} [{GetShortLogLevel()}] {SourceName}: {Content}\n";
    }

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
            var log = new DogeyLogMessage
            {
                LogLevel = logLevel,
                SourceName = _categoryName,
                Content = formatter(state, exception)
            };

            string logText = log.ToString();

            if (!Directory.Exists(_outputDirectory))
                Directory.CreateDirectory(_outputDirectory);

            var fileInfo = new FileInfo(_logFile);
            if (!fileInfo.Exists)
                fileInfo.Create().Dispose();
            if (fileInfo.Length + logText.Length > _maxFileSizeKb * 1000)
                _duplicateLogFileCount++;

            File.AppendAllText(_logFile, logText);
            SendConsole(log);
            Debug.Write(logText);
        }

        private void SendConsole(DogeyLogMessage log)
        {
            Console.Write(log.GetTimestamp(), Color.Gray);

            Color levelColor;
            switch (log.LogLevel)
            {
                case LogLevel.Trace:
                    levelColor = Color.White;
                    break;
                case LogLevel.Information:
                    levelColor = Color.DarkGreen;
                    break;
                case LogLevel.Warning:
                    levelColor = Color.Yellow;
                    break;
                case LogLevel.Debug:
                    levelColor = Color.Magenta;
                    break;
                case LogLevel.Error:
                    levelColor = Color.Red;
                    break;
                case LogLevel.Critical:
                    levelColor = Color.DarkRed;
                    break;
                default:
                    levelColor = Color.White;
                    break;
            }

            Console.Write(" [", levelColor);
            Console.Write(log.GetShortLogLevel(), levelColor);
            Console.Write("] ", levelColor);

            Console.Write(log.SourceName, Color.DarkGray);
            Console.Write(": ", Color.DarkGray);
            Console.Write(log.Content, Color.White);
            Console.WriteLine();
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}
