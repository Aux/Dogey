using Discord;
using Discord.Commands;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    public class InspectModule : ModuleBase
    {
        public string Inspect<T>(T obj, string property = null)
        {
            var type = obj.GetType();
            var info = type.GetTypeInfo();
            var properties = type.GetProperties()/*.OrderBy(x => x.PropertyType)*/;

            if (property != null)
                return properties.FirstOrDefault(x => x.Name.ToLower() == property)?.GetValue(obj).ToString();

            var builder = new StringBuilder();

            builder.AppendLine($"{info.Name} ({info.Namespace})");
            foreach(var p in properties)
            {
                builder.AppendLine($"{p.Name}: {p.GetValue(obj) ?? "null"}");
            }

            return $"```crystal\n{builder.ToString()}```";
        }
        
        [Command("inspect")]
        public async Task Inspect(ITextChannel channel, string property = null)
        {
            var result = Inspect<ITextChannel>(channel, property);
            await ReplyAsync(result);
        }

        [Command("inspect")]
        public async Task Inspect(IVoiceChannel channel, string property = null)
        {
            var result = Inspect<IVoiceChannel>(channel, property);
            await ReplyAsync(result);
        }

        [Command("inspect")]
        public async Task Inspect(IGuildUser user, string property = null)
        {
            var result = Inspect<IGuildUser>(user, property);
            await ReplyAsync(result);
        }

        [Command("inspect")]
        public async Task Inspect(IUser user, string property = null)
        {
            var result = Inspect<IUser>(user, property);
            await ReplyAsync(result);
        }
        
        [Command("inspect")]
        public async Task Inspect(IUserMessage message, string property = null)
        {
            var result = Inspect<IUserMessage>(message, property);
            await ReplyAsync(result);
        }

        [Command("inspect")]
        public async Task Inspect(IMessage message, string property = null)
        {
            var result = Inspect<IMessage>(message, property);
            await ReplyAsync(result);
        }
    }
}
