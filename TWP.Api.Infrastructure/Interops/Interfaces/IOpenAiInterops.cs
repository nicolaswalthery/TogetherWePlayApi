using System.Threading.Tasks;
using TWP.Api.Core.Enums;

namespace TWP.Api.Infrastructure.Interops.Interfaces
{
    public interface IOpenAiInterops
    {
        /// <summary>
        /// Sends a message to ChatGPT and returns the response
        /// </summary>
        /// <param name="message">The message to send to ChatGPT</param>
        /// <param name="systemPrompt">Optional system prompt to set the context</param>
        /// <returns>The response from ChatGPT</returns>
        Task<string> GetChatGptResponseAsync(string message, double temperature = 0.1, int maxTokens = 1000, string? systemPrompt = null, OpenAIResponseFormatEnum responseFormat = OpenAIResponseFormatEnum.Text);

        /// <summary>
        /// Sends a message to ChatGPT with custom parameters
        /// </summary>
        /// <param name="message">The message to send to ChatGPT</param>
        /// <param name="systemPrompt">Optional system prompt to set the context</param>
        /// <param name="temperature">Controls randomness in the response (0.0 to 2.0)</param>
        /// <param name="maxTokens">Maximum number of tokens in the response</param>
        /// <returns>The response from ChatGPT</returns>
        Task<string> ChatGptResponseAsync(string message, string? systemPrompt = null, double temperature = 0.7, int maxTokens = 1000, OpenAIResponseFormatEnum responseFormat = OpenAIResponseFormatEnum.Text);
    }
} 