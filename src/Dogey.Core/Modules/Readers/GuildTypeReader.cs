using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace Dogey.Modules.Readers
{
    public class GuildTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            var discord = context.Client as DiscordSocketClient;

            if (ulong.TryParse(input, out ulong guildId))
            {
                var guild = discord.Guilds.SingleOrDefault(x => x.Id == guildId);
                if (guild == null) return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, $"Could not find a guild with the id `{guildId}`"));
                return Task.FromResult(TypeReaderResult.FromSuccess(guild));
            }

            var guilds = discord.Guilds.Where(x => x.Name.ToLower() == input.ToLower());
            if (guilds.Count() == 0)
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ObjectNotFound, $"Could not find a guild named `{input}`"));
            if (guilds.Count() == 1)
                return Task.FromResult(TypeReaderResult.FromSuccess(guilds.First()));

            string guildInfos = string.Join("\n", guilds.Select(x => $"{x.Name} ({x.Id})"));
            return Task.FromResult(TypeReaderResult.FromError(CommandError.MultipleMatches, $"Found multiple guilds:\n{guildInfos}"));
        }
    }
}
