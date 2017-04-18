namespace Discord.Addons.OwnerTools
{
    public class ActivityCommandConfig : CommandConfig
    {
        public override string Summary { get; set; } = "Get or set the bot's current activity";

        public ActivityCommandConfig()
        {
            Aliases.Add("activity");
        }
    }
}
