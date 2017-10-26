using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Dogey
{
    public class PointsManager : DbManager<PointsDatabase>
    {
        public PointsManager(PointsDatabase db)
            : base(db) { }

        // Costs
        public Task<Cost> GetCostAsync(string command)
            => _db.Costs.FirstOrDefaultAsync(x => x.Id == command);

        // Points
        public Task<Point> GetPointAsync(ulong msgId)
            => _db.Points.SingleOrDefaultAsync(x => x.MessageId == msgId);
        public Task<Point[]> GetPointsAsync(ulong userId)
            => _db.Points.Where(x => x.UserId == userId).ToArrayAsync();
        public Task<Point[]> GetRecentPointsAsync(ulong userId, int count = 4)
            => _db.Points.Where(x => x.UserId == userId).Take(count).ToArrayAsync();
        public async Task DeletePointAsync(Point point)
        {
            _db.Points.Remove(point);
            await _db.SaveChangesAsync();
        }

        // Profiles
        public Task<bool> ProfileExistsAsync(ulong userId)
            => _db.Profiles.AnyAsync(x => x.UserId == userId);
        public Task<PointProfile> GetProfileAsync(ulong userId)
            => _db.Profiles.SingleOrDefaultAsync(x => x.UserId == userId);
        
        public async Task UpdateTotalPointsAsync(ulong userId, int amount)
        {
            var profile = await GetProfileAsync(userId);
            if (profile.IsMaxPoints()) return;
            profile.TotalPoints += (ulong)amount;
            _db.Profiles.Update(profile);
        }

        public async Task TryCreateProfileAsync(ulong userId)
        {
            if (await ProfileExistsAsync(userId))
                return;

            await CreateAsync(new PointProfile
            {
                UserId = userId
            });
        }
        
        public async Task<PointProfile> UpgradeWalletAsync(ulong userId)
        {
            var profile = await GetProfileAsync(userId);
            return await UpgradeWalletAsync(profile);
        }

        public async Task<PointProfile> UpgradeWalletAsync(PointProfile profile)
        {
            profile.TotalPoints = 0;
            profile.WalletSize = profile.WalletSize * 2;

            _db.Profiles.Update(profile);
            await _db.SaveChangesAsync();
            return profile;
        }

        // Creates
        public async Task CreateAsync(Cost cost)
        {
            await _db.Costs.AddAsync(cost);
            await _db.SaveChangesAsync();
        }

        public async Task CreateAsync(Point point)
        {
            await _db.Points.AddAsync(point);
            await _db.SaveChangesAsync();
        }

        public async Task CreateAsync(PointProfile profile)
        {
            await _db.Profiles.AddAsync(profile);
            await _db.SaveChangesAsync();
        }
    }
}
