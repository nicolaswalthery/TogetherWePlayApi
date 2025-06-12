using Common.ResultPattern;
using Microsoft.AspNetCore.Mvc;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Controllers.Interfaces;

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

        [HttpGet(Name = "GetMonsterActivity")]
        public async Task<IActionResult> GetMonsterActivity()
            => HandleResult(await Safe.ExecuteAsync(() => _dndEncounterBusinessLayer.EncounterRandomGenerator()));
    }
}
