using Common.ResultPattern;
using Microsoft.AspNetCore.Mvc;

namespace TWP.Api.Controllers.Interfaces
{
    public interface IEnrichmentDataController
    {
        Task<IActionResult> AiLoreAndMannerDeterminationDataEnrichment();
        Task<IActionResult> AiRoleDeterminationDataEnrichment();
    }
}
