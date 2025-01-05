using Microsoft.AspNetCore.Mvc;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Application.DataTransferObjects;
using TWP.Api.Controllers.Interfaces;

namespace TogetherWePlayApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UltraModern5eController : ControllerBase, IUltraModern5eController
    {

        private readonly ILogger<UltraModern5eController> _logger;
        private readonly IUltraModern5eBusinessLayer _ultraModern5EBusinessLayer;

        public UltraModern5eController(ILogger<UltraModern5eController> logger, IUltraModern5eBusinessLayer ultraModern5EBusinessLayer)
        {
            _logger = logger;
            _ultraModern5EBusinessLayer = ultraModern5EBusinessLayer;
        }

        [HttpGet(Name = "GetShootAndLoot")]
        public ShootAndLootDto GetShootAndLoot()
        {
            return _ultraModern5EBusinessLayer.ShootAndLootGeneration();
        }
    }
}
