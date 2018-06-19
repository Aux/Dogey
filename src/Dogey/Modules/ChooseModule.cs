using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [RequireEnabled]
    public class ChooseModule : DogeyModuleBase
    {
        private readonly Random _random;

        public ChooseModule(Random random)
        {
            _random = random;
        }

        [Command("choose")]
        public async Task ChooseAsync(params string[] choices)
        {
            int selectedIndex = _random.Next(0, choices.Count());
            var selected = choices.ElementAt(selectedIndex);

            await ReplyAsync(selected);
        }

        [Command("choosemany")]
        public async Task ChooseManyAsync(int amount, params string[] choices)
        {
            var selected = new List<string>();
            var selectedIndices = new List<int>();

            for (int i = 0; i <= amount; i++)
            {
                int selectedIndex = _random.Next(0, choices.Count());
                selected.Add(choices.ElementAt(selectedIndex));
            }

            await ReplyAsync(string.Join(", ", selected));
        }
    }
}
