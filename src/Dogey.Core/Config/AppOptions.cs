namespace Dogey.Config
{
    public class AppOptions
    {
        public AppOptions(bool isgenerating = false)
        {
            if (isgenerating)
            {
                Discord = new DiscordOptions();
            }
        }

        public string SuccessEmoji { get; set; } = "👍";
        public DiscordOptions Discord { get; set; }
    }
}
