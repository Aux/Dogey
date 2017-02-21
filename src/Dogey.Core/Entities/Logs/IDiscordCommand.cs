namespace Dogey
{
    public interface IDiscordCommand
    {
        string Name { get; }
        string Parameters { get; }
        double ExecuteTime { get; }
    }
}
