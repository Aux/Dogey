using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Name("Pats")]
    public class PatsModule : ModuleBase<SocketCommandContext>, IDisposable
    {
        private readonly PatsDatabase _db;
        
        public PatsModule(IServiceProvider provider)
        {
            _db = provider.GetService<PatsDatabase>();
        }

        [Command("pats")]
        public Task PatsAsync()
            => PatsAsync(Context.User);

        [Command("pats")]
        public async Task PatsAsync([Remainder]SocketUser user)
        {
            int sent = await _db.CountSentPatsAsync(user.Id);
            int received = await _db.CountReceivedPatsAsync(user.Id);
            
            await ReplyAsync($"{user} has received {received} pats and sent {sent}.");
        }

        [Command("pat")]
        public async Task PatAsync([Remainder]SocketUser user)
        {
            int recentPats = await _db.CountRecentPatsAsync(Context.User.Id);
            if (recentPats > 0)
            {
                await ReplyAsync("You can only send a pat once every 10 minutes :'(");
                return;
            }
            
            await _db.CreatePatAsync(Context.User, user);
            int received = await _db.CountReceivedPatsAsync(user.Id);
            var patFile = GetRandomPat();
            await Context.Channel.SendFileAsync(patFile.Item1, patFile.Item2, $"{user.Username} has been patted {received} times!");
        }

        private Tuple<FileStream, string> GetRandomPat()
        {
            string dir = Path.Combine(AppContext.BaseDirectory, "pats");
            var images = Directory.EnumerateFiles(dir);
            
            string selected = images.ElementAt(new Random().Next(0, images.Count() + 1));

            var stream = File.Open(selected, FileMode.Open);
            string name = Path.GetFileName(selected);
            return Tuple.Create(stream, name);
        }

        public void Dispose()
        {
            _db.Dispose();
        }
    }
}
