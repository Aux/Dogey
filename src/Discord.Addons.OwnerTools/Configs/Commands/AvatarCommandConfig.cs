namespace Discord.Addons.OwnerTools
{
    public class AvatarCommandConfig : CommandConfig
    {
        public override string Summary { get; set; } = "Get or set the bot's current avatar";

        public AvatarCommandConfig()
        {
            Aliases.Add("avatar");
        }
    }
}
