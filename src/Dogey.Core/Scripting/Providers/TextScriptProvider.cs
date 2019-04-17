using Dogey.Modules;
using System.IO;

namespace Dogey.Scripting.Providers
{
    public class TextScriptProvider : ScriptProviderBase
    {
        public override string ScriptFileExtension { get; }

        public TextScriptProvider() : base()
        {
            ScriptFileExtension = "txt";
        }

        public override string Execute(string filePath, DogeyCommandContext context)
        {
            return File.ReadAllText(filePath);
        }

        public override string ExecuteText(string content, DogeyCommandContext context)
        {
            return content; // ???
        }
    }
}
