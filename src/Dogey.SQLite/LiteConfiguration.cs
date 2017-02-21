namespace Dogey.SQLite
{
    public class LiteConfiguration : ConfigurationBase
    {
        public int RelatedTagsLimit { get; set; } = 5;

        public LiteConfiguration() : base("config/sqlite_config.json") { }

        public LiteConfiguration Load()
            => Load<LiteConfiguration>();
    }
}
