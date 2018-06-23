using Discord.Commands;
using System.Threading.Tasks;

namespace Dogey.Modules
{
    [RequireEnabled]
    public class DogModule : DogeyModuleBase
    {
        private readonly DogApiService _dog;

        public DogModule(DogApiService dog)
        {
            _dog = dog;
        }

        [Command("randomdog")]
        public async Task RandomDogAsync()
        {
            var dog = await _dog.GetDogAsync();
            if (dog == null)
            {
                await ReplyAsync("Couldn't find any dogs :'(");
                return;
            }

            var imageStream = await _dog.GetDogImageAsync(dog);
            await ReplyFileAsync(imageStream, dog.Id + "." + dog.ImageFormat);
        }
    }
}
