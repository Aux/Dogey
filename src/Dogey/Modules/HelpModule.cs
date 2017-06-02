using Discord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Group("help"), Name("Help")]
    public class HelpModule : ModuleBase<DogeyCommandContext>
    {
        private readonly CommandService _commands;
        private readonly IServiceProvider _provider;

        public HelpModule(IServiceProvider provider)
        {
            _commands = provider.GetService<CommandService>();
            _provider = provider;
        }

        [Command]
        public async Task HelpAsync()
        {
            string prefix = (await Context.Guild.GetPrefixAsync()) ?? $"@{Context.Client.CurrentUser.Username} ";
            var modules = _commands.Modules.Where(x => !string.IsNullOrWhiteSpace(x.Summary));

            var builder = new EmbedBuilder()
                .WithFooter(x => x.Text = $"Type `{prefix}help <module>` for more information");

            foreach (var module in modules)
            {
                bool success = false;
                foreach (var command in module.Commands)
                {
                    var result = await command.CheckPreconditionsAsync(Context, _provider);
                    if (result.IsSuccess)
                    {
                        success = true;
                        break;
                    }
                }

                if (!success)
                    continue;

                builder.AddField(module.Name, module.Summary);
            }

            await ReplyAsync("", embed: builder);
        }

        [Command]
        public async Task HelpAsync(string moduleName)
        {
            string prefix = (await Context.Guild.GetPrefixAsync()) ?? $"@{Context.Client.CurrentUser.Username} ";
            var module = _commands.Modules.FirstOrDefault(x => x.Name.ToLower() == moduleName.ToLower());

            if (module == null)
            {
                await ReplyAsync($"The module `{moduleName}` does not exist.");
                return;
            }

            var commands = module.Commands.Where(x => !string.IsNullOrWhiteSpace(x.Summary));

            if (commands.Count() == 0)
            {
                await ReplyAsync($"The module `{module.Name}` has no available commands :(");
                return;
            }

            var builder = new EmbedBuilder()
                .WithTitle($"Commands in {module.Name}")
                .WithFooter(x => x.Text = $"Type `{prefix}help <command>` for more information");

            foreach (var command in commands)
            {
                var result = await command.CheckPreconditionsAsync(Context, _provider);
                if (result.IsSuccess)
                    builder.AddField(prefix + command.Aliases.First(), command.Summary);
            }

            await ReplyAsync("", embed: builder);
        }

        [Command]
        public async Task HelpAsync(string moduleName, string commandName)
        {
            string alias = $"{moduleName} {commandName}".ToLower();
            string prefix = (await Context.Guild.GetPrefixAsync()) ?? $"@{Context.Client.CurrentUser.Username} ";
            var module = _commands.Modules.FirstOrDefault(x => x.Name.ToLower() == moduleName.ToLower());

            if (module == null)
            {
                await ReplyAsync($"The module `{moduleName}` does not exist.");
                return;
            }

            var commands = module.Commands.Where(x => !string.IsNullOrWhiteSpace(x.Summary));

            if (commands.Count() == 0)
            {
                await ReplyAsync($"The module `{module.Name}` has no available commands :(");
                return;
            }

            var command = commands.Where(x => x.Aliases.Contains(alias));
            var builder = new EmbedBuilder();

            var aliases = new List<string>();
            foreach (var overload in command)
            {
                var result = await overload.CheckPreconditionsAsync(Context, _provider);
                if (result.IsSuccess)
                {
                    var sbuilder = new StringBuilder()
                        .Append(prefix + overload.Aliases.First());

                    foreach (var parameter in overload.Parameters)
                    {
                        string p = parameter.Name;

                        if (parameter.IsRemainder)
                            p += "...";
                        if (parameter.IsOptional)
                            p = $"[{p}]";
                        else
                            p = $"<{p}>";

                        sbuilder.Append(" " + p);
                    }

                    builder.AddField(sbuilder.ToString(), overload.Remarks ?? overload.Summary);
                }
                aliases.AddRange(overload.Aliases);
            }

            builder.WithFooter(x => x.Text = $"Aliases: {string.Join(", ", aliases)}");

            await ReplyAsync("", embed: builder);
        }
    }
}
