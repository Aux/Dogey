using Scriban.Runtime;
using System.Linq;

namespace Dogey.Scripting
{
    public class ParameterFunctions : ScriptObject
    {
        public ParameterFunctions(string[] parameters)
        {
            var paramFunc = new ScriptObject();
            paramFunc.SetValue("array", parameters, true);
            paramFunc.SetValue("value", string.Join(" ", parameters), true);
            paramFunc.SetValue("count", parameters.Count(), true);

            SetValue("parameters", paramFunc, true);
        }
    }
}
