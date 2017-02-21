namespace Dogey.MySQL
{
    public class MyConfiguration : ConfigurationBase
    {
        public string Server { get; set; } = "localhost";
        public int Port { get; set; } = 3306;
        public string User { get; set; } = "root";
        public string Password { get; set; } = "";

        public MyConfiguration() : base("config/mysql_config.json") { }

        public MyConfiguration Load()
            => Load<MyConfiguration>();
    }
}
