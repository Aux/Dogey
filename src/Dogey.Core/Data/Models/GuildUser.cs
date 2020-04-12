namespace Dogey
{
    public class GuildUser
    {
        public ulong GuildId { get; set; }
        public ulong UserId { get; set; }
        public bool IsBlocked { get; set; }

        public Guild Guild { get; set; }
        public User User { get; set; }
    }
}
