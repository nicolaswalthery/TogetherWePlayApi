using TWP.Api.Core.Enums;

namespace TWP.Api.Application.Interfaces.Services
{
    public interface IOpenAiInterops
    {
        Task<string> ChatGptResponseAsync(string message, string? systemPrompt = null, double temperature = 0.1, int maxTokens = 1000, OpenAIResponseFormatEnum responseFormat = OpenAIResponseFormatEnum.Text);
        Task<string> GetChatGptResponseAsync(string message, double temperature = 0.1, int maxTokens = 1000, string? systemPrompt = null, OpenAIResponseFormatEnum responseFormat = OpenAIResponseFormatEnum.Text);

        // Vision methods
        Task<string> AnalyzeImageAsync(string base64Image, string prompt, double temperature = 0.2, int maxTokens = 4000);
        Task<string> AnalyzeImageFromFileAsync(string imagePath, string prompt, double temperature = 0.2, int maxTokens = 4000);
        Task<string> AnalyzeImageFromBytesAsync(byte[] imageBytes, string prompt, double temperature = 0.2, int maxTokens = 4000);
    }
}