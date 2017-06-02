using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Group("help"), Name("Help")]
    public class HelpModule : ModuleBase<DogeyCommandContext>
    {
        private readonly CommandService _commands;

        public HelpModule(CommandService commands)
        {
            _commands = commands;
        }

        [Command]
        public async Task HelpAsync()
        {
            string prefix = (await Context.Guild.GetPrefixAsync()) ?? $"@{Context.Client.CurrentUser.Username} ";
            var modules = _commands.Modules.Where(x => !string.IsNullOrWhiteSpace(x.Summary));

            var builder = new EmbedBuilder()
                .WithTitle($"Modules for {Context.Guild}")
                .WithFooter(x => x.Text = $"Type `{prefix}help <module>` for more information");

            foreach (var module in modules)
                builder.AddField(module.Name, module.Summary);

            await ReplyAsync("", embed: builder);
        }

        [Command]
        public async Task HelpAsync(string moduleName)
        {
            string prefix = (await Context.Guild.GetPrefixAsync()) ?? $"@{Context.Client.CurrentUser.Username} ";
            var module = _commands.Modules.FirstOrDefault(x => x.Name.ToLower() == moduleName.ToLower());

            if (module == null)
            {
                await ReplyAsync($"The module {moduleName} does not exist.");
                return;
            }

            var commands = module.Commands.Where(x => !string.IsNullOrWhiteSpace(x.Summary));

            if (commands.Count() == 0)
            {
                await ReplyAsync($"The module {moduleName} has no available commands :(");
                return;
            }

            var builder = new EmbedBuilder()
                .WithTitle($"Commands for {module.Name}")
                .WithFooter(x => x.Text = $"Type `{prefix}help <module> [command]` for more information");

            foreach (var command in commands)
                builder.AddField(prefix + command.Aliases.First(), command.Summary);

            await ReplyAsync("", embed: builder);
        }
    }
}
