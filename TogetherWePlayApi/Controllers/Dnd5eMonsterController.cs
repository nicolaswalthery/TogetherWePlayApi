using Microsoft.AspNetCore.Mvc;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Controllers.Interfaces;
using Common.ResultPattern;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace TogetherWePlayApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Dnd5eMonsterController : ControllerBase<Dnd5eMonsterController>, IDnd5eMonsterController
    {
        private readonly IDnd5eMonsterBusinessLayer _monsterBusinessLayer;

        public Dnd5eMonsterController(IDnd5eMonsterBusinessLayer monsterBusinessLayer, ILogger<Dnd5eMonsterController> logger) : base(logger)
        {
            _monsterBusinessLayer = monsterBusinessLayer;
        }

        [HttpGet("AllMonsterStatsCsv")]
        public async Task<IActionResult> GetAllMonsterStatsCsv()
            => HandleResult(await _monsterBusinessLayer.GetAllMonsterStatsCsv());
    }
} 