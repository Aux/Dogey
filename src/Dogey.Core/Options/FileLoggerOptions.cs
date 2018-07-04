using Newtonsoft.Json;

namespace Dogey
{
    public class FileLoggerOptions
    {
        public FileLoggerOptions()
        {
            UseRelativeOutput = true;
            OutputDirectory = "logs";
            DateTimeFormat = "yyyy-MM-dd";
            MaxFileSizeKb = 5000;
        }

        [JsonProperty("use_relative_output")]
        public bool UseRelativeOutput { get; set; }
        [JsonProperty("output_directory")]
        public string OutputDirectory { get; set; }
        [JsonProperty("datetime_format")]
        public string DateTimeFormat { get; set; }
        [JsonProperty("max_filesize_kb")]
        public int MaxFileSizeKb { get; set; }
    }
}
