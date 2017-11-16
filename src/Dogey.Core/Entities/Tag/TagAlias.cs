namespace Dogey
{
    public class TagAlias
    {
        public ulong Id { get; set; }
        public ulong TagId { get; set; }
        public ulong GuildId { get; set; }
        public ulong OwnerId { get; set; }
        public string Name { get; set; }

        // Foreign Keys
        public Tag Tag { get; set; }
    }
}
