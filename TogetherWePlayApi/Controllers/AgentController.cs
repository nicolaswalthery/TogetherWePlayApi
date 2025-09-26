using Common.ResultPattern;
using Microsoft.AspNetCore.Mvc;
using TWP.Api.Application.UseCases.Interfaces;
using TWP.Api.Controllers.Interfaces;

namespace TogetherWePlayApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AgentController : ControllerBase<AgentController>, IAgentController
    {
        private readonly IGameMasterAgentUseCase _gameMasterAgentUseCase;

        public AgentController(IGameMasterAgentUseCase gameMasterAgentUseCase, ILogger<AgentController> logger) : base(logger)
        {
            _gameMasterAgentUseCase = gameMasterAgentUseCase;
        }

        [HttpGet(Name = "AskGameMasterAgent")]
        public async Task<IActionResult> AskGameMasterAgent(string userPrompt)
            => HandleResult(await Safe.ExecuteAsync(async () => await _gameMasterAgentUseCase.AskGameMasterAgent(userPrompt)));
    }
}
