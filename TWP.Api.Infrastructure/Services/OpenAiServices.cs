using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using TWP.Api.Application.Interfaces.Services;
using TWP.Api.Core.Enums;

namespace TWP.Api.Infrastructure.Interops
{
    public class OpenAiServices : IOpenAiServices
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatCompletionService;
        private readonly string _modelName;
        private readonly string _visionModelName;

        public OpenAiServices(string apiKey, string modelName, string visionModelName, ILogger<OpenAiServices>? logger = null)
        {
            if (string.IsNullOrEmpty(apiKey))
                throw new ArgumentNullException(nameof(apiKey));

            _modelName = modelName;
            _visionModelName = visionModelName;

            var builder = Kernel.CreateBuilder();
            builder.AddOpenAIChatCompletion(_modelName, apiKey);

            _kernel = builder.Build();
            _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
        }

        public async Task<string> GetChatGptResponseAsync(string message, double temperature = 0.1, int maxTokens = 1000, string? systemPrompt = null, OpenAIResponseFormatEnum responseFormat = OpenAIResponseFormatEnum.Text)
            => await ChatGptResponseAsync(message, systemPrompt, temperature, maxTokens, responseFormat);

        public async Task<string> ChatGptResponseAsync(string message, string? systemPrompt = null, double temperature = 0.1, int maxTokens = 1000, OpenAIResponseFormatEnum responseFormat = OpenAIResponseFormatEnum.Text)
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
                    MaxTokens = maxTokens,
                    ResponseFormat = OpenAIResponseFormatEnum.Text == responseFormat ? "text" : "json_object"
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

        /// <summary>
        /// Analyzes an image using OpenAI Vision API through Semantic Kernel
        /// </summary>
        /// <param name="base64Image">Base64 encoded image string</param>
        /// <param name="prompt">The prompt/instructions for analyzing the image</param>
        /// <param name="temperature">Temperature for response generation (default: 0.2 for more precise analysis)</param>
        /// <param name="maxTokens">Maximum tokens in response (default: 4000 for detailed extraction)</param>
        /// <returns>The analysis result as a string</returns>
        public async Task<string> AnalyzeImageAsync(
            string base64Image,
            string prompt,
            double temperature = 0.2,
            int maxTokens = 4000)
        {
            try
            {
                // Create a new kernel with vision model if different from default
                Kernel visionKernel = _kernel;
                IChatCompletionService visionChatService = _chatCompletionService;

                // If vision model is different, create a specific service for it
                if (_visionModelName != _modelName)
                {
                    var apiKey = _kernel.Services.GetRequiredService<IConfiguration>()["OpenAI:ApiKey"];
                    var visionBuilder = Kernel.CreateBuilder();
                    visionBuilder.AddOpenAIChatCompletion(_visionModelName, apiKey);
                    visionKernel = visionBuilder.Build();
                    visionChatService = visionKernel.GetRequiredService<IChatCompletionService>();
                }

                var chatHistory = new ChatHistory();

                // Create a message with both text and image content
                var messageContent = new ChatMessageContentItemCollection();

                // Add the text prompt
                messageContent.Add(new TextContent(prompt));

                // Add the image content
                // Convert base64 to byte array if needed
                byte[] imageBytes;
                try
                {
                    imageBytes = Convert.FromBase64String(base64Image);
                }
                catch
                {
                    // If base64 conversion fails, assume it's already in the correct format
                    // or includes data URI prefix
                    if (base64Image.StartsWith("data:image"))
                    {
                        // Extract base64 from data URI
                        var base64Data = base64Image.Substring(base64Image.IndexOf(',') + 1);
                        imageBytes = Convert.FromBase64String(base64Data);
                    }
                    else
                    {
                        imageBytes = Convert.FromBase64String(base64Image);
                    }
                }

                // Create image content with the bytes
                var imageContent = new ImageContent(imageBytes, "image/jpeg");
                messageContent.Add(imageContent);

                // Add the combined message to chat history
                chatHistory.AddUserMessage(messageContent);

                // Configure execution settings for vision analysis
                var executionSettings = new OpenAIPromptExecutionSettings
                {
                    Temperature = temperature,
                    MaxTokens = maxTokens,
                    ResponseFormat = "json_object" // Force JSON for structured extraction
                };

                // Get response from Vision API
                var response = await visionChatService.GetChatMessageContentAsync(
                    chatHistory,
                    executionSettings);

                return response.Content ?? string.Empty;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error analyzing image with OpenAI Vision: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Analyzes an image from a file path
        /// </summary>
        public async Task<string> AnalyzeImageFromFileAsync(
            string imagePath,
            string prompt,
            double temperature = 0.2,
            int maxTokens = 4000)
        {
            try
            {
                // Read the image file and convert to base64
                var imageBytes = await File.ReadAllBytesAsync(imagePath);
                var base64Image = Convert.ToBase64String(imageBytes);

                return await AnalyzeImageAsync(base64Image, prompt, temperature, maxTokens);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error reading image file: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Analyzes an image from a byte array
        /// </summary>
        public async Task<string> AnalyzeImageFromBytesAsync(
            byte[] imageBytes,
            string prompt,
            double temperature = 0.2,
            int maxTokens = 4000)
        {
            try
            {
                var base64Image = Convert.ToBase64String(imageBytes);
                return await AnalyzeImageAsync(base64Image, prompt, temperature, maxTokens);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error processing image bytes: {ex.Message}", ex);
            }
        }
    }
}