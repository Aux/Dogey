using Discord;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey
{
    public class DogManager : DbManager<DogDatabase>
    {
        private readonly Random _random;

        public DogManager(DogDatabase db, Random random)
            : base(db)
        {
            _random = random;
        }

        public Task<DogImage> GetRandomDogImageAsync(ulong channelId)
        {
            var images = _db.Dogs.Where(x => x.ChannelId == channelId);
            if (images.Count() == 0)
                return null;

            var selected = _random.Next(0, images.Count() + 1);
            return Task.FromResult(images.ToArray().ElementAt(selected));
        }

        public async Task AddDogImageAsync(IUserMessage msg)
        {
            var image = msg.Attachments.FirstOrDefault();

            if (image == null)
                return;

            await CreateAsync(new DogImage
            {
                ChannelId = msg.Channel.Id,
                MessageId = msg.Id,
                Url = image.Url
            });
        }

        // Create
        public async Task CreateAsync(DogImage dog)
        {
            await _db.Dogs.AddAsync(dog);
            await _db.SaveChangesAsync();
        }
    }
}
