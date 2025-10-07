using Common.Security;
using Microsoft.AspNetCore.Mvc;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Controllers.Interfaces;

namespace TogetherWePlayApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [RequiredApiKey]
    public class UltraModern5eController : ControllerBase<UltraModern5eController>, IUltraModern5eController
    {
        private readonly IUltraModern5eBusinessLayer _ultraModern5eBusinessLayer;
        private readonly IDndEncounterBusinessLayer _dndEncounterBusinessLayer;

        public UltraModern5eController(IUltraModern5eBusinessLayer ultraModern5EBusinessLayer, IDndEncounterBusinessLayer dndEncounterBusinessLayer, ILogger<UltraModern5eController> logger) : base(logger)
        {
            _ultraModern5eBusinessLayer = ultraModern5EBusinessLayer;
            _dndEncounterBusinessLayer = dndEncounterBusinessLayer;
        }

        [HttpGet(Name = "GetShootAndLoot")]
        public async Task<IActionResult> GetShootAndLoot()
            => HandleResult(await _ultraModern5eBusinessLayer.ShootAndLootGeneration());

        [HttpGet("GenerateTreasureHoard")]
        public async Task<IActionResult> GenerateTreasureHoard([FromQuery] int challengeRating)
            => HandleResult(await _ultraModern5eBusinessLayer.GenerateTreasureHoard(challengeRating));

        [HttpGet("GenerateAdversaryFromDndMonster")]
        public async Task<IActionResult> GenerateAdversaryFromDndMonster([FromQuery] string narrativeDescription)
            => HandleResult(await _dndEncounterBusinessLayer.CreateScifiAdversaries(narrativeDescription));
    }
}
