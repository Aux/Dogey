namespace Dogey
{
    public interface IGuildConfig<T>
    {
        T GuildId { get; }
        string Prefix { get; }
    }
}
