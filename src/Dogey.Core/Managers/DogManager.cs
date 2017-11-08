using Discord;
using Microsoft.EntityFrameworkCore;
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

        public Task<DogImage> GetDogImageAsync(ulong msgId)
            => _db.Dogs.SingleOrDefaultAsync(x => x.MessageId == msgId);
        public Task<DogImage> GetLastestImageAsync(ulong channelId)
            => _db.Dogs.LastOrDefaultAsync(x => x.ChannelId == channelId);

        public Task<DogImage> GetRandomDogImageAsync(ulong channelId)
        {
            var images = _db.Dogs.Where(x => x.ChannelId == channelId);
            if (images.Count() == 0)
                return null;

            var selected = _random.Next(0, images.Count() + 1);
            return Task.FromResult(images.ToArray().ElementAt(selected));
        }

        public Task<bool> IsDupeImageAsync(IUserMessage msg)
            => _db.Dogs.AnyAsync(x => x.MessageId == msg.Id);
        
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

        public async Task RemoveDogImageAsync(ulong msgId)
        {
            var image = await GetDogImageAsync(msgId);
            if (image == null)
                return;

            _db.Dogs.Remove(image);
            await _db.SaveChangesAsync();
        }

        // Create
        public async Task CreateAsync(DogImage dog)
        {
            await _db.Dogs.AddAsync(dog);
            await _db.SaveChangesAsync();
        }
    }
}
