using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey
{
    public class ModuleInfoTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input, IServiceProvider services)
        {
            var commandService = (CommandService)services.GetService(typeof(CommandService));

            var module = commandService.Modules.FirstOrDefault(x => x.Name.ToLower() == input.ToLower());
            if (module != null)
                return Task.FromResult(TypeReaderResult.FromSuccess(module));
            return Task.FromResult(TypeReaderResult.FromError(CommandError.Unsuccessful, $"A module named `{input}` does not exist"));
        }
    }
}
