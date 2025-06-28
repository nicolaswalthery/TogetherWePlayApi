using System.Threading.Tasks;

namespace TWP.Api.Infrastructure.Interops
{
    public interface IOpenAiServices
    {
        /// <summary>
        /// Sends a message to ChatGPT and returns the response
        /// </summary>
        /// <param name="message">The message to send to ChatGPT</param>
        /// <param name="systemPrompt">Optional system prompt to set the context</param>
        /// <returns>The response from ChatGPT</returns>
        Task<string> GetChatGptResponseAsync(string message, string? systemPrompt = null);

        /// <summary>
        /// Sends a message to ChatGPT with custom parameters
        /// </summary>
        /// <param name="message">The message to send to ChatGPT</param>
        /// <param name="systemPrompt">Optional system prompt to set the context</param>
        /// <param name="temperature">Controls randomness in the response (0.0 to 2.0)</param>
        /// <param name="maxTokens">Maximum number of tokens in the response</param>
        /// <returns>The response from ChatGPT</returns>
        Task<string> GetChatGptResponseAsync(string message, string? systemPrompt = null, double temperature = 0.7, int maxTokens = 1000);
    }
} 