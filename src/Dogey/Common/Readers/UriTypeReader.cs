using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Dogey
{
    public class UriTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> Read(ICommandContext context, string input)
        {
            if (Uri.TryCreate(input, UriKind.Absolute, out Uri uri))
                return Task.FromResult(TypeReaderResult.FromSuccess(uri));
            else
                return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, $"`{input}` is not a valid url."));
        }
    }
}
