using Common.Security;
using Microsoft.AspNetCore.Mvc;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Controllers.Interfaces;

namespace TogetherWePlayApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [RequiredApiKey]
    public class Dnd5eMonsterController : ControllerBase<Dnd5eMonsterController>, IDnd5eMonsterController
    {
        private readonly IDnd5eMonsterBusinessLayer _monsterBusinessLayer;

        public Dnd5eMonsterController(IDnd5eMonsterBusinessLayer monsterBusinessLayer, ILogger<Dnd5eMonsterController> logger) : base(logger)
        {
            _monsterBusinessLayer = monsterBusinessLayer;
        }

        [HttpPost("CreateOriginalDndMonster")]
        public async Task<IActionResult> CreateOriginalDndMonster([FromQuery] float challengeRating)
            => HandleResult(await _monsterBusinessLayer.CreateOriginalDndMonster(challengeRating));

        [HttpGet("GetAll5eMonsters")]
        public async Task<IActionResult> GetAll5eMonsters()
            => HandleResult(await _monsterBusinessLayer.GetAll5eMonsters());

        [HttpGet("AllMonsterStatsByCr")]
        public async Task<IActionResult> GetAllMonsterStatsByCr([FromQuery] int cr)
            => HandleResult(await _monsterBusinessLayer.GetAllMonsterStatsByCr(cr));
    }
} 