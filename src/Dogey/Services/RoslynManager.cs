using Discord;
using Discord.WebSocket;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Dogey
{
    public class RoslynManager
    {
        public ScriptOptions GetOptions()
        {
            var options = ScriptOptions.Default
            .AddReferences(
                typeof(string).GetTypeInfo().Assembly,
                typeof(Assembly).GetTypeInfo().Assembly,
                typeof(Task).GetTypeInfo().Assembly,
                typeof(Enumerable).GetTypeInfo().Assembly,
                typeof(List<>).GetTypeInfo().Assembly,
                typeof(IGuild).GetTypeInfo().Assembly,
                typeof(SocketGuild).GetTypeInfo().Assembly,
                typeof(Dogey.Entity<>).GetTypeInfo().Assembly
            )
            .AddImports(
                "System",
                "System.Reflection",
                "System.Threading.Tasks",
                "System.Linq",
                "System.Collections.Generic",
                "Discord",
                "Discord.WebSocket",
                "Dogey"
            );

            return options;
        }

        //public async Task<object> CSharpAsync(string code)
        //{
        //    var _timer = new Stopwatch();
        //    _timer.Start();

        //    string reply, type;

        //    try
        //    {
        //        var result = await CSharpScript.EvaluateAsync(code, options, Context);
        //        type = result.GetType().Name;
        //        reply = result.ToString();
        //    }
        //    catch (Exception ex)
        //    {
        //        type = ex.GetType().Name;
        //        reply = ex.Message;
        //    }
        //    _timer.Stop();

        //    return null;
        //}

        public string GetFormattedCode(string language, string rawmsg)
        {
            string code = rawmsg;

            if (code.StartsWith("```"))
                code = code.Substring(3, code.Length - 6);
            if (code.StartsWith(language))
                code = code.Substring(2, code.Length - 2);

            code = code.Trim();
            code = code.Replace(";\n", ";");
            code = code.Replace("; ", ";");
            code = code.Replace(";", ";\n");

            return code;
        }
    }

    public class RoslynResult
    {
        public object Result { get; }
        public Type ResultType { get; }
        public string Language { get; }
        public long ExecuteTime { get; }

        public RoslynResult(object result, string language, long executeTime)
        {
            Result = result;
            ResultType = result.GetType();
            Language = language;
            ExecuteTime = executeTime;
        }
    }
}
