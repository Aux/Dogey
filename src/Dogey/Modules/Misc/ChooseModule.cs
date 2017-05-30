using Discord.Commands;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [Group("choose"), Name("Choose")]
    [Summary("")]
    public class ChooseModule : ModuleBase<DogeyCommandContext>
    {
        private readonly Random _random;

        public ChooseModule(IServiceProvider provider)
        {
            _random = provider.GetService<Random>();
        }

        [Command]
        [Remarks("Select an option at random")]
        public Task ChooseAsync(params string[] options)
        {
            var r = new Random();

            int index = r.Next(0, options.Count());
            string selected = options.ElementAt(index);

            return ReplyAsync(selected);
        }

        [Command]
        [Remarks("Select multiple options at random")]
        public Task ChooseAsync(int number, params string[] options)
        {
            var r = new Random();
            var indices = new List<int>();

            for (int i = 0; i <= number; i++)
                indices.Add(r.Next(0, options.Count()));

            var selected = new List<string>();
            foreach (var index in indices)
                selected.Add(options.ElementAt(index));
            
            return ReplyAsync(string.Join(", ", selected));
        }
    }
}
