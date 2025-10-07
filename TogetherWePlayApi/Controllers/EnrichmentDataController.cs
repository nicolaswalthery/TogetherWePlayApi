using Common.ResultPattern;
using Microsoft.AspNetCore.Mvc;
using TWP.Api.Application.DataEnrichmentLayer.Interfaces;
using TWP.Api.Controllers.Interfaces;

namespace TogetherWePlayApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EnrichmentDataController : ControllerBase<EnrichmentDataController>, IEnrichmentDataController
    {
        private readonly IMonster5eDataEnrichmentLayer _monster5eDataEnrichmentLayer;

        public EnrichmentDataController(IMonster5eDataEnrichmentLayer monster5eDataEnrichmentLayer, ILogger<EnrichmentDataController> logger) : base(logger)
        {
            _monster5eDataEnrichmentLayer = monster5eDataEnrichmentLayer;
        }

        [HttpGet("lore-manner")]
        public async Task<IActionResult> AiLoreAndMannerDeterminationDataEnrichment()
            => HandleResult(await Safe.ExecuteAsync(() => _monster5eDataEnrichmentLayer.AiLoreAndMannerDetermination()));

        [HttpGet("role")]
        public async Task<IActionResult> AiRoleDeterminationDataEnrichment()
            => HandleResult(await Safe.ExecuteAsync(() => _monster5eDataEnrichmentLayer.AiRoleDetermination()));
    }
}
