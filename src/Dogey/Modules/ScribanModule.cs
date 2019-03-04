using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Dogey.Scripting;
using Dogey.Services;
using Scriban;
using Scriban.Functions;
using Scriban.Runtime;

namespace Dogey.Modules
{
    [Name("Scriban"), Group("script")]
    public class ScribanModule : DogeyModuleBase
    {
        private readonly ScriptHandlingService _scripter;

        public ScribanModule(ScriptHandlingService scripter)
        {
            _scripter = scripter;
        }

        [Command("file")]
        public async Task EvalFileAsync(string name)
        {
            if (!_scripter.TryGetScript(name, out Template template))
            {
                await ReplyAsync($"A script by the name of `{name}` was not found.");
                return;
            }
            
            await ReplyAsync(_scripter.ExecuteAsync(name, new ScriptObject()));
        }

        [Command("evaluate"), Alias("eval")]
        public async Task EvalAsync([Remainder]string text)
        {
            var context = new TemplateContext();
            context.EnableRelaxedMemberAccess = true;
            context.PushGlobal(new DiscordFunctions(Context));
            context.PushGlobal(new BuiltinFunctions());

            var template = Template.Parse(text);

            if (template.HasErrors)
                await ReplyAsync(string.Join("\n", template.Messages));
            else
            {
                var result = template.Render(context);
                if (string.IsNullOrWhiteSpace(result))
                    return;
                await ReplyEmbedAsync(new EmbedBuilder()
                    .WithTitle("Result")
                    .WithDescription(result));
            }
        }
    }
}
