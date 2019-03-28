using Scriban.Runtime;
using System;

namespace Dogey.Scripting
{
    public class ParserFunctions : ScriptObject
    {
        public ParserFunctions()
        {
            var parseFunc = new ScriptObject();
            parseFunc.Import("int", new Func<string, int>((value) => int.Parse(value)));
            parseFunc.Import("uint", new Func<string, uint>((value) => uint.Parse(value)));
            parseFunc.Import("long", new Func<string, long>((value) => long.Parse(value)));
            parseFunc.Import("ulong", new Func<string, ulong>((value) => ulong.Parse(value)));
            parseFunc.Import("short", new Func<string, short>((value) => short.Parse(value)));
            parseFunc.Import("ushort", new Func<string, ushort>((value) => ushort.Parse(value)));
            parseFunc.Import("double", new Func<string, double>((value) => double.Parse(value)));
            parseFunc.Import("float", new Func<string, float>((value) => float.Parse(value)));

            SetValue("parse", parseFunc, true);
        }
    }
}
