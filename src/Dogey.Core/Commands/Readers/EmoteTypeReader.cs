using Discord;
using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Dogey
{
    public class EmoteTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> Read(ICommandContext context, string input, IServiceProvider services)
        {
            if (Emote.TryParse(input, out Emote result))
                return Task.FromResult(TypeReaderResult.FromSuccess(result));
            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, $"`{input}` is not a valid emote"));
        }
    }
}
