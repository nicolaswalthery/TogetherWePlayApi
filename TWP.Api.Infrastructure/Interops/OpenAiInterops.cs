using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace TWP.Api.Infrastructure.Interops
{
    public class OpenAiInterops : IOpenAiInterops
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatCompletionService;
        private readonly string _modelName;

        public OpenAiInterops(IConfiguration configuration)
        {
            var apiKey = configuration["OpenAI:ApiKey"] 
                ?? throw new ArgumentNullException("OpenAI:ApiKey configuration is missing");
            
            _modelName = configuration["OpenAI:ModelName"] ?? configuration["OpenAI:DefaultModelName"];
            
            var builder = Kernel.CreateBuilder();
            builder.AddOpenAIChatCompletion(_modelName, apiKey);
            
            _kernel = builder.Build();
            _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
        }

        public async Task<string> GetChatGptResponseAsync(string message, double temperature = 0.1, int maxTokens = 1000, string? systemPrompt = null)
            => await ChatGptResponseAsync(message, systemPrompt, temperature, maxTokens);

        public async Task<string> ChatGptResponseAsync(string message, string? systemPrompt = null, double temperature = 0.1, int maxTokens = 1000)
        {
            try
            {
                var chatHistory = new ChatHistory();
                
                // Add system prompt if provided
                if (!string.IsNullOrEmpty(systemPrompt))
                {
                    chatHistory.AddSystemMessage(systemPrompt);
                }
                
                // Add user message
                chatHistory.AddUserMessage(message);

                // Configure execution settings
                var executionSettings = new OpenAIPromptExecutionSettings
                {
                    Temperature = temperature,
                    MaxTokens = maxTokens
                };

                // Get response from ChatGPT
                var response = await _chatCompletionService.GetChatMessageContentAsync(
                    chatHistory, 
                    executionSettings);

                return response.Content ?? string.Empty;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error calling ChatGPT: {ex.Message}", ex);
            }
        }
    }
}
