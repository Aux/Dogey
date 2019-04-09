using Discord.Commands;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Dogey.Commands.Readers
{
    public class RegexTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            try
            {
                var regex = new Regex(input);
                regex.Match("");
                return Task.FromResult(TypeReaderResult.FromSuccess(regex));
            }
            catch
            {
                return Task.FromResult(TypeReaderResult.FromError(CommandError.Unsuccessful, $"Invalid regex specified: `{input}`"));
            }
        }
    }
}
