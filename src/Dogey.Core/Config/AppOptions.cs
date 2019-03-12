namespace Dogey.Config
{
    public class AppOptions
    {
        public AppOptions(bool isgenerating = false)
        {
            if (isgenerating)
            {
                Discord = new DiscordOptions();
                Logging = new LoggingOptions();
            }
        }

        public CommandOptions Commands { get; set; }
        public DiscordOptions Discord { get; set; }
        public GoogleOptions Google { get; set; }
        public WebOptions Web { get; set; }
        public LoggingOptions Logging { get; set; }
    }
}
