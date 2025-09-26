using Microsoft.AspNetCore.Mvc;
using TWP.Api.Core.Enums;

namespace TWP.Api.Controllers.Interfaces
{
    public interface IAgentController
    {
        Task<IActionResult> AskGameMasterAgent(string userPrompt);
    }
}
