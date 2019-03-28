using Discord;
using Discord.Commands;
using Dogey.Databases;
using Dogey.Models;
using Dogey.Modules;
using Microsoft.Extensions.Configuration;

namespace Dogey.Services
{
    public class PrefixService
    {
        private readonly ConfigController _controller;
        private readonly LogController _log;
        private readonly IConfiguration _config;

        public PrefixService(
            ConfigController controller, 
            LogController log, 
            IConfiguration config)
        {
            _controller = controller;
            _log = log;
            _config = config;
        }

        public bool TryGetPosition(DogeyCommandContext context, out int argPos)
            => TryGetPosition(context, GetPrefix(context), out argPos);
        public bool TryGetPosition(DogeyCommandContext context, string prefix, out int argPos)
        {
            argPos = 0;
            if (context.User.Id == context.Client.CurrentUser.Id)
                return false;

            var hasStringPrefix = prefix == null ? false : context.Message.HasStringPrefix(prefix, ref argPos);
            return (hasStringPrefix || context.Message.HasMentionPrefix(context.Client.CurrentUser, ref argPos));
        }

        public string GetPrefix(DogeyCommandContext context)
        {
            string prefix = null;
            if (!_controller.TryGetPrefix(context.Guild.Id, out prefix))
                prefix = _config["commands:default_prefix"];
            return prefix;
        }

        public void SetPrefix(IGuild guild, string prefix)
        {
            if (string.IsNullOrWhiteSpace(prefix))
                prefix = null;
            _controller.Add(new GuildConfig
            {
                Id = guild.Id,
                Prefix = prefix
            });
        }
    }
}
