using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Dogey.Services
{
    public class LoggingService
    {
        private readonly ILoggerFactory _factory;
        private readonly DiscordSocketClient _discordSocketClient;
        private readonly CommandService _commandService;

        public LoggingService(
            ILoggerFactory factory,
            DiscordSocketClient discordSocketClient,
            CommandService commandService)
        {
            _factory = factory;
            _commandService = commandService;
            _discordSocketClient = discordSocketClient;
        }

        public void Start()
        {
            _commandService.Log += OnLogAsync;
            _discordSocketClient.Log += OnLogAsync;
        }

        private Task OnLogAsync(LogMessage msg)
        {
            var logger = _factory.CreateLogger(msg.Source);
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
