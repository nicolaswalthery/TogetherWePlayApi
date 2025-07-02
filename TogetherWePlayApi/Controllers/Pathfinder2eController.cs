using Microsoft.AspNetCore.Mvc;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Controllers.Interfaces;
using TWP.Api.Core.DataTransferObjects;
using Common.ResultPattern;

namespace TogetherWePlayApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Pathfinder2eController : ControllerBase<Pathfinder2eController>, IPathfinder2eController
    {
        private readonly IPathfinder2eBusinessLayer _pathfinder2eBusinessLayer;

        public Pathfinder2eController(IPathfinder2eBusinessLayer pathfinder2eBusinessLayer, ILogger<Pathfinder2eController> logger) : base(logger)
        {
            _pathfinder2eBusinessLayer = pathfinder2eBusinessLayer;
        }

        [HttpGet("random/core-monster")]
        public async Task<IActionResult> GetOneRandomCoreMonster()
            => HandleResult(await _pathfinder2eBusinessLayer.GetOneRandomPf2eCoreMonster());

        [HttpGet("random/condition")]
        public async Task<IActionResult> GetOneRandomCondition()
            => HandleResult(await _pathfinder2eBusinessLayer.GetOneRandomPf2eCondition());
    }
}
