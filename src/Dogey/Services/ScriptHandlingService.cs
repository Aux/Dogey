using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Scriban;
using Scriban.Functions;
using Scriban.Parsing;
using Scriban.Runtime;

namespace Dogey.Services
{
    public class ScriptHandlingService
    {
        public static string ScriptsDirectory = Path.Combine(AppContext.BaseDirectory, "scripts");
        public const string ScriptExtension = ".sbn";
        public const string FrontMatterMarker = "---";

        public LexerOptions DefaultLexerOptions { get; }
        public int TotalScripts { get; }

        private readonly ILogger<ScriptHandlingService> _logger;

        public ScriptHandlingService(ILogger<ScriptHandlingService> logger)
        {
            _logger = logger;

            DefaultLexerOptions = new LexerOptions
            {
                FrontMatterMarker = FrontMatterMarker,
                Mode = ScriptMode.FrontMatterAndContent
            };

            TotalScripts = GetScriptFiles("*").Length;
        }

        private string[] GetScriptFiles(string fileName)
            => Directory.GetFiles(ScriptsDirectory, fileName + ScriptExtension, SearchOption.AllDirectories);
        private string GetRelativePath(string filePath)
            => "../" + new Uri(ScriptsDirectory).MakeRelativeUri(new Uri(filePath)).OriginalString;

        public Template GetScript(string name)
        {
            if (!TryGetScript(name, out Template value))
                throw new ArgumentNullException(nameof(name));
            return value;
        }
        public bool TryGetScript(string name, out Template value)
        {
            value = default;
            var scriptFiles = GetScriptFiles(name);
            if (scriptFiles.Count() == 0)
                return false;

            var fileContent = File.ReadAllText(scriptFiles.First());
            value = Template.Parse(fileContent, GetRelativePath(scriptFiles.First()), lexerOptions: DefaultLexerOptions);
            return true;
        }

        public string ExecuteAsync(string name, ScriptObject scriptObject)
        {
            if (!TryGetScript(name, out Template value))
                return null;
            if (value.HasErrors)
                return string.Join("\n", value.Messages);
            return ExecuteAsync(value, scriptObject);
        }
        public string ExecuteAsync(Template template, ScriptObject scriptObject)
        {
            var timer = Stopwatch.StartNew();
            var context = new TemplateContext(new BuiltinFunctions());
            context.PushGlobal(scriptObject);

            var rendered = template.Render(context);

            timer.Stop();
            _logger.LogInformation($"Executed `{template.SourceFilePath}` in {timer.ElapsedMilliseconds}ms");

            return rendered;
        }
    }
}
