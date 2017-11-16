using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey
{
    public class TagService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly LoggingService _logger;
        private readonly TagManager _manager;

        private ConcurrentDictionary<ulong, ModuleInfo> _tagModules;

        public TagService(
            DiscordSocketClient discord,
            CommandService commands,
            LoggingService logger,
            TagManager manager)
        {
            _discord = discord;
            _commands = commands;
            _logger = logger;
            _manager = manager;

            _tagModules = new ConcurrentDictionary<ulong, ModuleInfo>();

            _discord.Ready += OnReadyAsync;
        }

        private async Task OnReadyAsync()
        {
            int guildCount = 0,
                tagCount = 0;

            foreach (var guild in _discord.Guilds)
            {
                var tags = await _manager.GetTagsAsync(guild.Id);
                if (tags.Count() == 0)
                    continue;

                guildCount++;
                foreach (var tag in tags)
                {
                    if (tag.IsCommand)
                    {
                        await AddCommandAsync(tag);
                        tagCount++;
                    }
                }
            }

            await _logger.LogAsync(LogSeverity.Info, "TagService", $"Enabled for {guildCount} guild(s) with {tagCount} tag(s).");
        }

        public async Task AddCommandAsync(Tag tag)
        {
            var aliases = await _manager.GetAliasesAsync(tag.Id);

            var tagModule = await _commands.CreateModuleAsync("", module =>
            {
                module.AddCommand(tag.Name, (context, parameters, provider, info) =>
                {
                    return context.Channel.SendMessageAsync(tag.Content);
                }, cmd =>
                {
                    cmd.AddParameter<string>("garbage", p =>
                    {
                        p.IsRemainder = true;
                        p.IsOptional = true;
                    });
                });
            });

            if (!_tagModules.TryAdd(tag.Id, tagModule))
                await _logger.LogAsync(LogSeverity.Error, "TagService", $"Unable to create command for `{tag.Name} ({tag.Id})`.");
        }

        public async Task RemoveCommandAsync(Tag tag)
        {
            if (_tagModules.TryRemove(tag.Id, out ModuleInfo tagModule))
                await _commands.RemoveModuleAsync(tagModule);
        }
    }
}
