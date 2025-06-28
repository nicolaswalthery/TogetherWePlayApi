using Microsoft.AspNetCore.Mvc;
using TWP.Api.Infrastructure.Interops;

namespace TogetherWePlayApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LlmController : ControllerBase
    {
        private readonly IOpenAiInterops _llmServices;

        public LlmController(IOpenAiInterops llmServices)
        {
            _llmServices = llmServices;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromBody] ChatRequest request)
        {
            try
            {
                var response = await _llmServices.GetChatGptResponseAsync(
                    request.Message, 
                    request.SystemPrompt);

                return Ok(new { response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("chat-advanced")]
        public async Task<IActionResult> ChatAdvanced([FromBody] ChatAdvancedRequest request)
        {
            try
            {
                var response = await _llmServices.GetChatGptResponseAsync(
                    request.Message, 
                    request.SystemPrompt, 
                    request.Temperature, 
                    request.MaxTokens);

                return Ok(new { response });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = string.Empty;
        public string? SystemPrompt { get; set; }
    }

    public class ChatAdvancedRequest
    {
        public string Message { get; set; } = string.Empty;
        public string? SystemPrompt { get; set; }
        public double Temperature { get; set; } = 0.7;
        public int MaxTokens { get; set; } = 1000;
    }
} 