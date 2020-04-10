using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;

namespace Dogey.Logging
{
    public class BotLogMessage
    {
        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public LogLevel LogLevel { get; set; }
        public string SourceName { get; set; }
        public string Content { get; set; }

        public string GetTimestamp() => Timestamp.ToString("hh:mm:ss");
        public string GetShortLogLevel() => LogLevel.ToString().Substring(0, 4);
        public override string ToString() => $"{GetTimestamp()} [{GetShortLogLevel()}] {SourceName}: {Content}\n";
    }

    public class BotLogger : ILogger
    {
        private readonly LoggingOptions _options;
        private readonly string _categoryName;
        private readonly string _outputDirectory;

        private int _duplicateLogFileCount = 0;

        private string _logFile => Path.Combine(_outputDirectory, GetFileName(DateTime.UtcNow));

        public BotLogger(string categoryName, LoggingOptions options)
        {
            _options = options;
            _categoryName = categoryName;
            _outputDirectory = _options.UseRelativeOutput
                ? Path.Combine(AppContext.BaseDirectory, _options.OutputDirectory)
                : _options.OutputDirectory;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public string GetFileName(DateTime dateTime)
        {
            var builder = new StringBuilder(dateTime.ToString(_options.DateTimeFormat));
            if (_duplicateLogFileCount != 0)
                builder.Append($" ({_duplicateLogFileCount})");
            builder.Append(".txt");
            return builder.ToString();
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var log = new BotLogMessage
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
            {
                fileInfo.Create().Dispose();
                fileInfo.Refresh();
            }
            if (fileInfo.Length + logText.Length > _options.MaxFileSizeKb * 1000)
                _duplicateLogFileCount++;
            try
            {
                using (var writer = fileInfo.AppendText())
                    writer.Write(logText);
            }
            catch { }

            SendConsole(log);
            Debug.Write(logText);
        }

        private void SendConsole(BotLogMessage log)
        {
            if (!_options.UseColorOutput)
            {
                Console.WriteLine($"{log.GetTimestamp()} [{log.GetShortLogLevel()}] {log.SourceName}: {log.Content}");
                return;
            }

            Console.Write(log.GetTimestamp(), Color.Gray);

            Color levelColor;
            switch (log.LogLevel)
            {
                case LogLevel.Trace:
                    levelColor = Color.White;
                    break;
                case LogLevel.Information:
                    levelColor = Color.Green;
                    break;
                case LogLevel.Warning:
                    levelColor = Color.Yellow;
                    break;
                case LogLevel.Debug:
                    levelColor = Color.Purple;
                    break;
                case LogLevel.Error:
                    levelColor = Color.Red;
                    break;
                case LogLevel.Critical:
                    levelColor = Color.Red;
                    break;
                default:
                    levelColor = Color.White;
                    break;
            }

            Colorful.Console.Write($" [{log.GetShortLogLevel()}] ", levelColor);
            Colorful.Console.Write(log.SourceName, Color.DarkGray);
            Colorful.Console.Write(": ", Color.DarkGray);
            Colorful.Console.Write(log.Content, Color.White);
            Console.WriteLine();
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}
