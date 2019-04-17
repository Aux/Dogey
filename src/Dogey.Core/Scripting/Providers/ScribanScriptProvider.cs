using Dogey.Modules;
using Microsoft.Extensions.Configuration;
using Scriban;
using Scriban.Functions;
using Scriban.Parsing;
using System.IO;
using System.Linq;

namespace Dogey.Scripting.Providers
{
    public class ScribanScriptProvider : ScriptProviderBase
    {
        public override string ScriptFileExtension { get; }
        public LexerOptions DefaultLexerOptions { get; }

        private readonly IConfiguration _config;

        private BuiltinFunctions _builtInFunctions;
        private ConfigFunctions _configFunctions;
        private ParserFunctions _parserFunctions;

        public ScribanScriptProvider(IConfiguration config)
            : base()
        {
            ScriptFileExtension = "scriban";
            DefaultLexerOptions = new LexerOptions
            {
                Mode = ScriptMode.ScriptOnly
            };

            _config = config;

            _builtInFunctions = new BuiltinFunctions();
            _configFunctions = new ConfigFunctions(config);
            _parserFunctions = new ParserFunctions();
        }

        public TemplateContext GetState(DogeyCommandContext context)
        {
            var templateContext = new TemplateContext();
            templateContext.EnableRelaxedMemberAccess = true;

            var discordFunctions = new DiscordFunctions(context);
            templateContext.PushGlobal(discordFunctions);

            var parameters = context.Message.Content.Split(' ');
            var parameterFunctions = new ParameterFunctions(parameters.Skip(1).ToArray());
            templateContext.PushGlobal(parameterFunctions);

            templateContext.PushGlobal(_builtInFunctions);
            templateContext.PushGlobal(_configFunctions);
            templateContext.PushGlobal(_parserFunctions);
            return templateContext;
        }

        public override string Execute(string filePath, DogeyCommandContext context)
        {
            var content = File.ReadAllText(filePath);
            var template = Template.Parse(content, ScriptingService.GetRelativePath(filePath), lexerOptions: DefaultLexerOptions);
            return ExecuteTemplate(template, context);
        }
        public override string ExecuteText(string content, DogeyCommandContext context)
        {
            var template = Template.Parse(content, lexerOptions: DefaultLexerOptions);
            return ExecuteTemplate(template, context);
        }
        private string ExecuteTemplate(Template template, DogeyCommandContext context)
        {
            if (template.HasErrors)
                return string.Join("\n", template.Messages);

            var state = GetState(context);
            return template.Render(state);
        }
    }
}
