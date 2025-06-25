using Common.ResultPattern;
using Microsoft.AspNetCore.Mvc;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Controllers.Interfaces;
using TWP.Api.Core.Enums;

namespace TogetherWePlayApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DndController : ControllerBase<DndController>, IDndController
    {
        private readonly IDndEncounterBusinessLayer _dndEncounterBusinessLayer;

        public DndController(IDndEncounterBusinessLayer dndEncounterBusinessLayer, ILogger<DndController> logger) : base(logger)
        {
            _dndEncounterBusinessLayer = dndEncounterBusinessLayer;
        }

        [HttpGet(Name = "GenerateRandomEncounter")]
        public async Task<IActionResult> GenerateRandomEncounter([FromQuery] EncounterDifficultyEnum encounterDifficulty, [FromQuery] IList<int> playerLevels, [FromQuery] string encounterNarrativeContext)
            => HandleResult(await Safe.ExecuteAsync(() => _dndEncounterBusinessLayer.EncounterRandomGenerator(encounterDifficulty, playerLevels, encounterNarrativeContext)));
    }
}
