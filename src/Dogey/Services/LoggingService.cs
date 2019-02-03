using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace Dogey.Services
{
    public class LoggingService : Service
    {
        private readonly ILoggerFactory _factory;
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;

        public LoggingService(
            ILoggerFactory factory,
            DiscordSocketClient discord,
            CommandService commands)
        {
            _factory = factory;
            _commands = commands;
            _discord = discord;
        }
        
        public override void Start()
        {
            _discord.Log += OnLogAsync;
            _commands.Log += OnLogAsync;
        }

        public override void Stop()
            => throw new NotImplementedException();

        private Task OnLogAsync(LogMessage msg)
        {
            var logger = _factory.CreateLogger("Discord." + msg.Source);
            string message = msg.Exception?.ToString() ?? msg.Message;
            switch (msg.Severity)
            {
                case LogSeverity.Debug:
                    logger.LogDebug(message);
                    break;
                case LogSeverity.Warning:
                    logger.LogWarning(message);
                    break;
                case LogSeverity.Error:
                    logger.LogError(message);
                    break;
                case LogSeverity.Critical:
                    logger.LogCritical(message);
                    break;
                default:
                    logger.LogInformation(message);
                    break;
            }
            return Task.CompletedTask;
        }
    }
}