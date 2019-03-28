using Discord;
using Discord.Commands;
using Dogey.Scripting;
using Dogey.Services;
using Microsoft.Extensions.Configuration;
using Scriban;
using Scriban.Functions;
using Scriban.Parsing;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Modules.Scripting
{
    [Name("Scriban"), Group("script")]
    [RequireUserPermission(GuildPermission.Administrator)]
    public class AdminScriptingModule : DogeyModuleBase
    {
        private readonly ScriptHandlingService _scripter;
        private readonly ResponsiveService _responsive;
        private readonly IConfiguration _config;
        private readonly HttpClient _http;

        public AdminScriptingModule(
            ScriptHandlingService scripter, 
            ResponsiveService responsive, 
            IConfiguration config, 
            HttpClient http)
        {
            _scripter = scripter;
            _responsive = responsive;
            _config = config;
            _http = http;
        }

        [Command("create"), Alias("new")]
        public async Task CreateAsync(string name, [Remainder]string content)
        {
            var file = _scripter.GetFileInfo(name);
            if (file.Exists)
            {
                await ReplyAsync($"A script by the name of `{name}` already exists.");
                return;
            }

            var template = Template.Parse(content, lexerOptions: new LexerOptions { Mode = ScriptMode.ScriptOnly });
            if (template.HasErrors)
            {
                await ReplyAsync(string.Join("\n", template.Messages));
                return;
            }

            using (var stream = file.CreateText())
            {
                stream.Write(content);
                stream.Flush();
            }

            await ReplyAsync($"Created `{name}` successfully");
        }

        [Command("update"), Alias("modify")]
        public async Task UpdateAsync(string name, [Remainder]string text)
        {
            var file = _scripter.GetFileInfo(name);
            if (!file.Exists)
            {
                await ReplyAsync($"A script by the name of `{name}` was not found.");
                return;
            }

            var template = Template.Parse(text, lexerOptions: _scripter.DefaultLexerOptions);
            if (template.HasErrors)
            {
                await ReplyAsync(string.Join("\n", template.Messages));
                return;
            }

            using (var stream = file.OpenWrite())
            {
                stream.SetLength(0);
                stream.Write(Encoding.UTF8.GetBytes(text).AsSpan());
                stream.Flush();
            }

            await ReplyAsync($"Updated `{name}` successfully.");
        }

        [Command("delete"), Alias("remove")]
        public async Task DeleteAsync(string name)
        {
            var file = _scripter.GetFileInfo(name);
            if (!file.Exists)
            {
                await ReplyAsync($"A script by the name of `{name}` was not found.");
                return;
            }

            await ReplyAsync($"Please type `{name}` to confirm deletion of this script.");
            var msg = await _responsive.WaitForMessageAsync((m) =>
            {
                if (m.Author.Id == Context.User.Id && m.Channel.Id == Context.Channel.Id)
                {
                    if (m.Content == name)
                        return true;
                }
                return false;
            });

            if (msg == null)
            {
                await ReplyAsync($"`{name}` was not deleted.");
                return;
            }

            file.Delete();
            await ReplyAsync($"Deleted `{name}` successfully.");
        }

        [Command("file")]
        public async Task EvalFileAsync(string name, params string[] parameters)
        {
            if (!_scripter.TryGetScript(name, out Template template))
            {
                await ReplyAsync($"A script by the name of `{name}` was not found.");
                return;
            }

            await ReplyAsync(_scripter.ExecuteAsync(name,
                new DiscordFunctions(Context),
                new ParameterFunctions(parameters)));
        }

        [Command("evaluate"), Alias("eval")]
        public async Task EvalAsync([Remainder]string text)
        {
            var context = new TemplateContext();
            context.EnableRelaxedMemberAccess = true;
            context.PushGlobal(new DiscordFunctions(Context));
            context.PushGlobal(new ConfigFunctions(_config));
            context.PushGlobal(new HttpFunctions(_http));
            context.PushGlobal(new BuiltinFunctions());

            var template = Template.Parse(text, lexerOptions: new LexerOptions
            {
                Mode = ScriptMode.ScriptOnly
            });

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
