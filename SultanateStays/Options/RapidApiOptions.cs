namespace SultanateStays.Options
{
    public class RapidApiOptions
    {
        public const string SectionName = "RapidApi";

        public string BaseUrl { get; set; } = string.Empty;

        public string Host { get; set; } = string.Empty;

        public string ApiKey { get; set; } = string.Empty;
    }
}
