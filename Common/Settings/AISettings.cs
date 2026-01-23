namespace TravelTechApi.Common.Settings
{
    public class AISettings
    {
        public string Provider { get; set; } = "OpenAI";
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "gpt-5-chat-latest";
        public int MaxTokens { get; set; } = 8192;
        public decimal Temperature { get; set; } = 0.1m;
    }
}
