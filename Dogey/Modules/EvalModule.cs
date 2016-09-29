using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Dogey.Attributes;
using Dogey.Enums;
using Dogey.Extensions;
using Dogey.Models;
using Dogey.Types;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        
        public class RoslynGlobals
        {
            public DiscordSocketClient _client { get; set; }
            public DataContext db { get; set; } = new DataContext();

            public RoslynGlobals(DiscordSocketClient c)
            {
                _client = c;
            }
        }

        [Command("evaluate"), Alias("eval")]
        [Description("Execute some c# code.")]
        public async Task Evaluate(IUserMessage msg, [Remainder]string expression)
        {
            var options = ScriptOptions.Default
            .AddReferences(new[]
            {
                typeof(object).GetTypeInfo().Assembly.Location,
                typeof(Object).GetTypeInfo().Assembly.Location,
                typeof(Enumerable).GetTypeInfo().Assembly.Location,
                typeof(DiscordSocketClient).GetTypeInfo().Assembly.Location,
                typeof(Command).GetTypeInfo().Assembly.Location,
                typeof(IMessage).GetTypeInfo().Assembly.Location,
                typeof(DataContext).GetTypeInfo().Assembly.Location,
                typeof(AccessLevel).GetTypeInfo().Assembly.Location,
                typeof(CommandEx).GetTypeInfo().Assembly.Location,
                typeof(DbSet<MessageLog>).GetTypeInfo().Assembly.Location
            })
            .AddImports(new[]
            {
                "System",
                "System.Linq",
                "System.Collections",
                "System.Collections.Generic",
                "Microsoft.EntityFrameworkCore",
                "Discord",
                "Discord.Commands",
                "Discord.WebSocket",
                "Dogey"
            });

            var global = new RoslynGlobals(_client);
            var result = await CSharpScript.EvaluateAsync(expression, options, globals: global);
            await msg.Channel.SendMessageAsync(result.ToString());
        }
    }
}
