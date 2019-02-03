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

        public string SuccessEmoji { get; set; } = ":thumbs_up:";
        public DiscordOptions Discord { get; set; }
        public LoggingOptions Logging { get; set; }
    }
}
