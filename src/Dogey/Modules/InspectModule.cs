using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    public class InspectModule : ModuleBase<SocketCommandContext>
    {
        public Embed Inspect<T>(T obj, string property = null)
        {
            var type = obj.GetType();
            var info = type.GetTypeInfo();
            var properties = type.GetProperties();

            if (property != null)
            {
                return new EmbedBuilder()
                {
                    Description = properties.FirstOrDefault(x => x.Name.ToLower() == property.ToLower())?.GetValue(obj).ToString()
                };
            }

            var builder = new StringBuilder();

            builder.AppendLine($"{info.Name} ({info.Namespace})");
            foreach (var p in properties)
            {
                if (p.GetType().GetTypeInfo().GetInterface("IEnumerable") != null)
                {
                    var value = (IEnumerable)p.GetValue(obj);
                    var contents = value != null ? string.Join(", ", value) : "null";
                    builder.AppendLine($"{p.Name}: {contents}");
                } else
                {
                    var value = p.GetValue(obj);
                    builder.AppendLine($"{p.Name}: {value ?? "null"}");
                }
            }

            var embed = new EmbedBuilder()
            {
                Description = builder.ToString()
            };

            return embed.Build();
        }

        [Command("inspect"), Priority(1)]
        public async Task InspectAsync(SocketChannel channel, string property = null)
        {
            var result = Inspect<SocketChannel>(channel, property);
            await ReplyAsync("", embed: result);
        }

        [Command("inspect"), Priority(0)]
        [RequireContext(ContextType.Guild)]
        public async Task InspectAsync(SocketGuildChannel channel, string property = null)
        {
            var result = Inspect<SocketChannel>(channel, property);
            await ReplyAsync("", embed: result);
        }

        [Command("inspect")]
        public async Task InspectAsync(SocketRole role, string property = null)
        {
            var result = Inspect<SocketRole>(role, property);
            await ReplyAsync("", embed: result);
        }

        [Command("inspect"), Priority(1)]
        public async Task InspectAsync(SocketUser user, string property = null)
        {
            var result = Inspect<SocketUser>(user, property);
            await ReplyAsync("", embed: result);
        }

        [Command("inspect"), Priority(0)]
        [RequireContext(ContextType.Guild)]
        public async Task InspectAsync(SocketGuildUser user, string property = null)
        {
            var result = Inspect<SocketUser>(user, property);
            await ReplyAsync("", embed: result);
        }

        [Command("inspect")]
        public async Task InspectAsync(SocketUserMessage message, string property = null)
        {
            var result = Inspect<SocketMessage>(message, property);
            await ReplyAsync("", embed: result);
        }
    }
}
