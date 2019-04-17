using Dogey.Modules;
using Microsoft.Extensions.Configuration;
using NLua;
using System.IO;
using System.Linq;

namespace Dogey.Scripting.Providers
{
    public class LuaScriptProvider : ScriptProviderBase
    {
        public override string ScriptFileExtension { get; }

        private readonly IConfiguration _config;

        public LuaScriptProvider(IConfiguration config)
            : base()
        {
            ScriptFileExtension = "lua";

            _config = config;
        }

        public Lua GetState(Lua lua, DogeyCommandContext context)
        {
            lua.State.Encoding = System.Text.Encoding.UTF8;
            lua["discord"] = context;

            return lua;
        }

        public override string Execute(string filePath, DogeyCommandContext context)
        {
            var content = File.ReadAllText(filePath);
            return ExecuteText(content, context);
        }

        public override string ExecuteText(string content, DogeyCommandContext context)
        {
            using (var lua = new Lua())
            {
                GetState(lua, context);

                var result = lua.DoString(content).FirstOrDefault();
                return result.ToString();
            }
        }
    }
}
