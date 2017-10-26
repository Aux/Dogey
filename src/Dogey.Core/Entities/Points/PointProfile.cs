using System.Collections.Generic;

namespace Dogey
{
    public class PointProfile
    {
        public ulong UserId { get; set; }
        public ulong TotalPoints { get; set; } = 0;
        public ulong WalletSize { get; set; } = 250;
        public double? Handicap { get; set; }

        // Foreign Keys
        //public List<Point> Points { get; set; }

        public bool IsMaxPoints()
            => TotalPoints >= WalletSize;
    }
}
