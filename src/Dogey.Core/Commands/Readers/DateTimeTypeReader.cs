using Discord.Commands;
using System;
using System.Globalization;
using System.Threading.Tasks;

namespace Dogey
{
    public class DateTimeTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> Read(ICommandContext context, string input, IServiceProvider services)
        {
            if (DateTime.TryParseExact(input, "dd/MM", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime result))
                return Task.FromResult(TypeReaderResult.FromSuccess(result));
            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed, $"`{input}` is not a valid date"));
        }
    }
}
