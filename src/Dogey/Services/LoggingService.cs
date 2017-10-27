using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Dogey
{
    public class LoggingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;

        private string _logDirectory { get; }
        private string _logFile => Path.Combine(_logDirectory, $"{DateTime.UtcNow.ToString("yyyy-MM-dd")}.txt");
        
        public LoggingService(DiscordSocketClient discord, CommandService commands)
        {
            _logDirectory = Path.Combine(AppContext.BaseDirectory, "logs");

            _discord = discord;
            _commands = commands;

            _discord.Log += OnLogAsync;
            _commands.Log += OnLogAsync;
        }

        public Task LogAsync(object severity, string source, string message)
        {
            if (!Directory.Exists(_logDirectory))
                Directory.CreateDirectory(_logDirectory);
            if (!File.Exists(_logFile))
                File.Create(_logFile).Dispose();

            string logText = $"{DateTime.UtcNow.ToString("hh:mm:ss")} [{severity}] {source}: {message}";
            File.AppendAllText(_logFile, logText + "\n");

            return PrettyConsole.LogAsync(severity, source, message);
        }

        private Task OnLogAsync(LogMessage msg)
            => LogAsync(msg.Severity, msg.Source, msg.Exception?.ToString() ?? msg.Message);
    }
}
