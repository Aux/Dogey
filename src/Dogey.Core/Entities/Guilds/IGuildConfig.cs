namespace Dogey
{
    public interface IGuildConfig<TId>
    {
        TId GuildId { get; }
        string Prefix { get; }
    }
}
