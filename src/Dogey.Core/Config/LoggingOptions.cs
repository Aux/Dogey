namespace Dogey.Config
{
    public class LoggingOptions
    {
        public LoggingOptions()
        {
            UseRelativeOutput = true;
            OutputDirectory = "logs";
            DateTimeFormat = "yyyy-MM-dd";
            MaxFileSizeKb = 5000;
        }
        
        public bool UseRelativeOutput { get; set; }
        public string OutputDirectory { get; set; }
        public string DateTimeFormat { get; set; }
        public int MaxFileSizeKb { get; set; }
    }
}
