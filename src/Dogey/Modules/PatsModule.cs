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
    [Summary("Pat pat pat pat pat pat!")]
    public class PatsModule : DogeyModuleBase
    {
        private readonly PatsDatabase _db;
        
        public PatsModule(IServiceProvider provider)
        {
            _db = provider.GetService<PatsDatabase>();
        }

        [Command("pats")]
        [Summary("Get your pat stats")]
        public Task PatsAsync()
            => PatsAsync(Context.User);

        [Command("pats")]
        [Summary("Get pat stats for the specified user")]
        public async Task PatsAsync([Remainder]SocketUser user)
        {
            int sent = await _db.CountSentPatsAsync(user.Id);
            int received = await _db.CountReceivedPatsAsync(user.Id);
            
            await ReplyAsync($"{user} has received {received} pats and sent {sent}.");
        }

        [Command("pat")]
        [Summary("Pat the specified user")]
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
            
            string selected = images.ElementAt(new Random().Next(0, images.Count()));

            var stream = File.Open(selected, FileMode.Open);
            string name = Path.GetFileName(selected);
            return Tuple.Create(stream, name);
        }
    }
}
