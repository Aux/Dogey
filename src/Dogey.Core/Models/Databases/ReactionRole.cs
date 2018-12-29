namespace Dogey
{
    public class ReactionRole
    {
        public ulong Id { get; set; }
        public ulong GuildId { get; set; }
        public ulong RoleId { get; set; }
        public ulong MessageId { get; set; }
        public string EmoteName { get; set; }
    }
}
