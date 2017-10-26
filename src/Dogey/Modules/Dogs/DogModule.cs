using Discord.Commands;
using System.Net.Http;
using System.Threading.Tasks;

namespace Dogey.Modules.Dogs
{
    [Name("Dogs")]
    public class DogModule : DogeyModuleBase
    {
        private readonly DogManager _manager;
        private readonly HttpClient _http;

        private const ulong _dogeId = 206606983250313216;
        private const ulong _dogId = 318586966788669450;

        public DogModule(DogManager manager)
        {
            _manager = manager;
            _http = new HttpClient();
        }

        protected override void AfterExecute(CommandInfo command)
        {
            _http.Dispose();
        }

        [Command("doge")]
        public async Task DogeAsync()
        {
            var image = await _manager.GetRandomDogImageAsync(_dogeId);
            var stream = await _http.GetStreamAsync(image.Url);
            await ReplyAsync(stream, image.MessageId.ToString() + ".png");
        }

        [Command("dog")]
        public async Task DogAsync()
        {
            var image = await _manager.GetRandomDogImageAsync(_dogId);
            var stream = await _http.GetStreamAsync(image.Url);
            await ReplyAsync(stream, image.MessageId.ToString() + ".png");
        }
    }
}
