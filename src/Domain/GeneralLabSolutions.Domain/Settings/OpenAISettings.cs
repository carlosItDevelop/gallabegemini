namespace GeneralLabSolutions.Domain.Settings
{
    public class OpenAISettings
    {
        public string? ApiKey { get; set; } = string.Empty;
        public string? BaseUrl { get; set; } = "https://api.openai.com/v1/chat/completions";
        public string? Model { get; set; } = "gpt-4o-mini";
        public int MaxTokens { get; set; } = 3500;
        public double Temperature { get; set; } = 0.2;
        public string? SystemPrompt { get; set; } = string.Empty;
    }
}