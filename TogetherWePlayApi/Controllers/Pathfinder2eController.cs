using Microsoft.AspNetCore.Mvc;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Controllers.Interfaces;
using TWP.Api.Core.DataTransferObjects;

namespace TogetherWePlayApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class Pathfinder2eController : ControllerBase, IPathfinder2eController
    {

        private readonly ILogger<Pathfinder2eController> _logger;
        private readonly IPathfinder2eBusinessLayer _pathfinder2eBusinessLayer;

        public Pathfinder2eController(ILogger<Pathfinder2eController> logger, IPathfinder2eBusinessLayer pathfinder2eBusinessLayer)
        {
            _logger = logger;
            _pathfinder2eBusinessLayer = pathfinder2eBusinessLayer;
        }

        [HttpGet(Name = "GetOneRandomCoreMonster")]
        public Pf2eMonsterDto GetOneRandomCoreMonster()
        {
            return _pathfinder2eBusinessLayer.GetOneRandomPf2eCoreMonster();
        }
    }
}
