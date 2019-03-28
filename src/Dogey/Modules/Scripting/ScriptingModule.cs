using Discord;
using Discord.Commands;
using Dogey.Services;
using Scriban;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Scripting
{
    [Name("Scripting"), Group("script")]
    public partial class ScriptingModule : DogeyModuleBase
    {
        private readonly ScriptHandlingService _scripter;

        public ScriptingModule(ScriptHandlingService scripter)
        {
            _scripter = scripter;
        }

        [Command("show")]
        public async Task ShowAsync(string name)
        {
            var file = _scripter.GetFileInfo(name);
            if (!_scripter.TryGetScript(name, out Template template))
            {
                await ReplyAsync($"A script by the name of `{name}` was not found.");
                return;
            }

            await ReplyEmbedAsync(new EmbedBuilder()
                .WithTitle($"Contents of {template.SourceFilePath}")
                .WithDescription(Format.Code(File.ReadAllText(file.FullName), "bash")));
        }

        [Command("list")]
        public async Task ListAsync()
        {
            var scriptFiles = _scripter.GetScriptFiles("*");

            await ReplyEmbedAsync(new EmbedBuilder()
                .WithTitle($"Available Scripts ({scriptFiles.Count()})")
                .WithDescription(string.Join(", ", scriptFiles.Select(x => Path.GetFileNameWithoutExtension(x)))));
        }
    }
}
