using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace Dogey.Commands.Readers
{
    public class UriTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            if (Uri.TryCreate(input, UriKind.Absolute, out Uri uri))
                return Task.FromResult(TypeReaderResult.FromSuccess(uri));
            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, $"`{input}` is not a valid url."));
        }
    }
}
