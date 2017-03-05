using Discord.Commands;
using System.Threading.Tasks;

namespace Dogey.SQLite
{
    public static class SocketCommandContextExtension
    {
        public static async Task LogAsync(this SocketCommandContext context, long ms)
        {
            using (var db = new LogDatabase())
            {
                var msg = await db.GetMessageAsync(context.Message.Id);
                var log = new LiteDiscordCommand(msg.Id, context.Message.Id, ms);

                await db.Commands.AddAsync(log);
                await db.SaveChangesAsync();
            }
        }
    }
}
