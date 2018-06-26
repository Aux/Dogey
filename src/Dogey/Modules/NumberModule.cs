using Discord.Commands;
using System;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    public class NumberModule : DogeyModuleBase
    {
        private readonly NumbersApiService _numbers;

        public NumberModule(NumbersApiService numbers)
        {
            _numbers = numbers;
        }

        [Command("numfact"), Priority(0)]
        public async Task NumfactAsync([Remainder]DateTime date)
        {
            var reply = await _numbers.GetDateAsync(date);
            if (reply == null)
                await ReplyAsync($"Couldn't find a fact for {date}");
            else
                await ReplyAsync(reply);
        }

        [Command("numfact"), Priority(2)]
        public async Task NumfactAsync(int number, NumberType type = NumberType.Trivia)
        {
            var reply = await _numbers.GetNumberAsync(number, type);
            if (reply == null)
                await ReplyAsync($"Couldn't find a {type} for {number}");
            else
                await ReplyAsync(reply);
        }

        [Command("numfact random"), Priority(1)]
        public async Task RandomNumberFactAsync(NumberType type = NumberType.Trivia)
        {
            var reply = await _numbers.GetRandomAsync(type);
            if (reply == null)
                await ReplyAsync($"Couldn't find a {type}");
            else
                await ReplyAsync(reply);
        }
    }
}
