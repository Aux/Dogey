using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using Dogey.Scripting;
using Microsoft.Extensions.Configuration;
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
        private readonly BuiltinFunctions _builtinFunctions;
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        public ScriptHandlingService(ILogger<ScriptHandlingService> logger, IConfiguration config, HttpClient http)
        {
            _logger = logger;
            _builtinFunctions = new BuiltinFunctions();
            _config = config;
            _http = http;

            DefaultLexerOptions = new LexerOptions
            {
                FrontMatterMarker = FrontMatterMarker,
                Mode = ScriptMode.ScriptOnly
            };

            TotalScripts = GetScriptFiles("*").Length;
        }

        public string[] GetScriptFiles(string fileName)
            => Directory.GetFiles(ScriptsDirectory, fileName + ScriptExtension, SearchOption.AllDirectories);
        public string GetRelativePath(string filePath)
            => "../" + new Uri(ScriptsDirectory).MakeRelativeUri(new Uri(filePath)).OriginalString;
        public FileInfo GetFileInfo(string name)
            => new FileInfo(Path.Combine(AppContext.BaseDirectory, "scripts", GetScriptFiles(name).FirstOrDefault()));
        
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

        public string ExecuteAsync(string name, params ScriptObject[] scriptObjects)
        {
            if (!TryGetScript(name, out Template value))
                return null;
            if (value.HasErrors)
                return string.Join("\n", value.Messages);
            return ExecuteAsync(value, scriptObjects);
        }
        public string ExecuteAsync(Template template, params ScriptObject[] scriptObjects)
        {
            var timer = Stopwatch.StartNew();
            var context = new TemplateContext(_builtinFunctions);
            context.PushGlobal(new HttpFunctions(_http));
            context.PushGlobal(new ConfigFunctions(_config));
            context.PushGlobal(new ParserFunctions());
            foreach (var scriptObject in scriptObjects)
                context.PushGlobal(scriptObject);

            var result = template.Render(context);

            timer.Stop();
            _logger.LogInformation($"Executed `{template.SourceFilePath}` in {timer.ElapsedMilliseconds}ms");

            return result;
        }
    }
}
