namespace Dogey
{
    public class PointProfile
    {
        public ulong UserId { get; set; }
        public long TotalPoints { get; set; } = 0;
        public long WalletSize { get; set; } = 250;
        public double? Handicap { get; set; }
        
        public bool IsMaxPoints()
            => TotalPoints >= WalletSize;
    }
}
