namespace Dogey.Logging
{
    public class LoggingOptions
    {
        public bool UseColorOutput { get; } = true;
        public bool UseRelativeOutput { get; } = true;
        public int MaxFileSizeKb { get; } = 5000;
        public string OutputDirectory { get; } = "common/logs";
        public string DateTimeFormat { get; } = "yyyy-MM-dd";
    }
}
