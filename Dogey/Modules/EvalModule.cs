using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Attributes;
using Dogey.Enums;
using Dogey.Extensions;
using Dogey.Models;
using Dogey.Types;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Module, Name("Owner")]
    [MinPermissions(AccessLevel.Owner)]
    public class EvalModule
    {
        private DiscordSocketClient _client;

        public EvalModule(DiscordSocketClient client)
        {
            _client = client;
        }

        [Command("evaluate"), Alias("eval")]
        [Description("Execute some c# code.")]
        public async Task Evaluate(IUserMessage msg, [Remainder]string expression)
        {
            var options = ScriptOptions.Default.AddReferences(new[]
            {
                typeof(IMessage).AssemblyQualifiedName,
                typeof(Enumerable).AssemblyQualifiedName
            });

            options.WithImports(new[]
            {
                "System",
                "System.Linq",
                "System.Collections",
                "System.Collections.Generic",
                "Discord",
            });

            string code = "using System; using System.Linq; using System.Collections; using System.Collections.Generic; " +
                          "using Discord; using Discord.WebSocket; using Dogey.Models; using Dogey.Enums;" + expression;

            var result = await CSharpScript.EvaluateAsync(code, options);
            await msg.Channel.SendMessageAsync(result.ToString());
        }
    }
}
