namespace GeneralLabSolutions.Domain.Services
{
    public record ChatMessage(string Role, string Content);

    public interface IOpenAIService
    {
        Task<string> ChatAsync(
            IEnumerable<ChatMessage> messages,
            string? model = null,
            int? maxTokens = null,
            double? temperature = null);
    }
}
