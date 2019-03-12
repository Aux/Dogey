using Scriban.Runtime;
using System;

namespace Dogey.Scripting
{
    public class ParameterFunctions : ScriptObject
    {
        public ParameterFunctions(string[] parameters)
        {
            var paramFunc = new ScriptObject();
            paramFunc.SetValue("value", string.Join(" ", parameters), true);
            paramFunc.Import("get", new Func<int, string>((i) => parameters?[i]));

            SetValue("params", paramFunc, true);
        }
    }
}
