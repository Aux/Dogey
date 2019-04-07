namespace Dogey.Config
{
    public class AppOptions
    {
        public AppOptions(bool isgenerating = false)
        {
            if (isgenerating)
            {
                Commands = new CommandOptions();
                Discord = new DiscordOptions();
                Google = new GoogleOptions();
                Github = new GithubOptions();
                Twitch = new TwitchOptions();
                Logging = new LoggingOptions();
            }
        }

        public CommandOptions Commands { get; set; }
        public DiscordOptions Discord { get; set; }
        public GoogleOptions Google { get; set; }
        public GithubOptions Github { get; set; }
        public TwitchOptions Twitch { get; set; }
        public LoggingOptions Logging { get; set; }
    }
}
