using Discord.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    public class ChooseModule : ModuleBase<SocketCommandContext>
    {
        [Command("choose")]
        [Remarks("Select an option at random")]
        public Task ChooseAsync(params string[] options)
        {
            var r = new Random();

            int selectedIndex = r.Next(0, options.Count());
            string selected = options.ElementAt(selectedIndex);

            return ReplyAsync(selected);
        }
    }
}
