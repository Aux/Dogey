using Dogey.Modules;

namespace Dogey.Scripting
{
    public abstract class ScriptProviderBase
    {
        public abstract string ScriptFileExtension { get; }
        public abstract string Execute(string filePath, DogeyCommandContext context);
        public abstract string ExecuteText(string content, DogeyCommandContext context);
    }
}
