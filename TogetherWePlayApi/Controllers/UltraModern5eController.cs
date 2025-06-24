using Microsoft.AspNetCore.Mvc;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Application.DataTransferObjects;
using TWP.Api.Controllers.Interfaces;
using Common.ResultPattern;

namespace TogetherWePlayApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UltraModern5eController : ControllerBase<UltraModern5eController>, IUltraModern5eController
    {
        private readonly IUltraModern5eBusinessLayer _ultraModern5eBusinessLayer;

        public UltraModern5eController(IUltraModern5eBusinessLayer ultraModern5EBusinessLayer, ILogger<UltraModern5eController> logger) : base(logger)
        {
            _ultraModern5eBusinessLayer = ultraModern5EBusinessLayer;
        }

        [HttpGet(Name = "GetShootAndLoot")]
        public async Task<IActionResult> GetShootAndLoot()
            => HandleResult(await _ultraModern5eBusinessLayer.ShootAndLootGeneration());
    }
}
