using Discord;
using Discord.Commands;
using Dogey.Scripting;
using System;
using System.Threading.Tasks;

namespace Dogey.Modules.Scripting
{
    [Name("Scripting"), Group("script")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class AdminScriptingModule : DogeyModuleBase
    {
        private readonly ScriptingService _scripting;

        public AdminScriptingModule(ScriptingService scripting)
        {
            _scripting = scripting;
        }

        [Command("create"), Alias("new")]
        public Task CreateAsync(string name, [Remainder]string content)
        {
            throw new NotImplementedException();
        }

        [Command("update"), Alias("modify")]
        public Task UpdateAsync(string name, [Remainder]string content)
        {
            throw new NotImplementedException();
        }

        [Command("delete"), Alias("remove")]
        public Task DeleteAsync(string name)
        {
            throw new NotImplementedException();
        }

        [Command("execute"), Alias("exec")]
        public async Task ExecuteAsync(string name)
        {
            if (_scripting.TryExecuteScript(name, Context, out string result))
                await ReplyEmbedAsync(new EmbedBuilder()
                    .WithTitle("Result")
                    .WithDescription(result));
            else
                await ReplyAsync($"A script file named `{name}` was not found");
        }

        [Command("evaluate"), Alias("eval")]
        public async Task EvaluateAsync(string language, [Remainder]string content)
        {
            var result = _scripting.ExecuteText(language, content, Context);
            await ReplyEmbedAsync(new EmbedBuilder()
                .WithTitle("Result")
                .WithDescription(result));
        }
    }
}
