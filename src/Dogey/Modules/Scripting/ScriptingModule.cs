using Discord;
using Discord.Commands;
using Dogey.Scripting;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules.Scripting
{
    [Name("Scripting"), Group("script")]
    public class ScriptingModule : DogeyModuleBase
    {
        private readonly ScriptingService _scripting;

        public ScriptingModule(ScriptingService scripting)
        {
            _scripting = scripting;
        }

        [Command("show")]
        public Task ShowAsync(string name)
        {
            var filePath = _scripting.GetScriptPath(name);
            if (filePath != null)
                return ShowInternalAsync(filePath);

            var part = name.Split('.');
            if (part.Length == 2)
                return ShowAsync(part[0], part[1]);
            else
                return ReplyAsync($"`{name}` is not a valid script file");
        }
        [Command("show")]
        public Task ShowAsync(string name, string extension)
        {
            var file = _scripting.GetScriptPath(name, extension);
            return ShowInternalAsync(file);
        }

        private Task ShowInternalAsync(string filePath)
        {
            return ReplyEmbedAsync(new EmbedBuilder()
                .WithTitle($"Contents of {ScriptingService.GetRelativePath(filePath)}")
                .WithDescription(Format.Code(File.ReadAllText(filePath), Path.GetExtension(filePath).Substring(1))));
        }

        [Command("list")]
        public async Task ListAsync()
        {
            var scriptFiles = _scripting.GetScriptFilePaths();

            await ReplyEmbedAsync(new EmbedBuilder()
                .WithTitle($"Available Scripts ({scriptFiles.Count()})")
                .WithDescription(string.Join(", ", scriptFiles.Select(x => Path.GetFileNameWithoutExtension(x)))));
        }
    }
}
