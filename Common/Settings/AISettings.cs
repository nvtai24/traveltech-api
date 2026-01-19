namespace TravelTechApi.Common.Settings
{
    public class AISettings
    {
        public string Provider { get; set; } = "OpenAI";
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "gpt-4o-mini";
        public int MaxTokens { get; set; } = 4096;
        public decimal Temperature { get; set; } = 0.7m;
    }
}
