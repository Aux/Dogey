namespace Dogey.MySQL
{
    public class MyConfiguration
    {
        public static string Name { get; } = "config/mysql_configuration.json";

        public string Server { get; set; } = "localhost";
        public int Port { get; set; } = 3306;
        public string User { get; set; } = "root";
        public string Password { get; set; } = "";
    }
}
