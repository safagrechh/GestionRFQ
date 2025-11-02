namespace EX.Core.Services
{
    public class GoogleAIOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = "https://generativelanguage.googleapis.com";
        public string Model { get; set; } = "gemini-2.5-flash";
        public double DefaultTemperature { get; set; } = 0.3;
        public int DefaultMaxTokens { get; set; } = 256;
    }
}