using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Dogey
{
    public class TagTypeReader : TypeReader
    {
        public override async Task<TypeReaderResult> Read(ICommandContext context, string input, IServiceProvider services)
        {
            var manager = services.GetService<TagManager>();

            var tag = await manager.FindTagAsync(context.Guild.Id, input);
            if (tag != null)
                return TypeReaderResult.FromSuccess(tag);
            else
                return TypeReaderResult.FromError(CommandError.ObjectNotFound, $"Unable to find a tag like `{input}`.");
        }
    }
}
