using Common.ResultPattern;
using Microsoft.AspNetCore.Mvc;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Controllers.Interfaces;
using TWP.Api.Infrastructure.DataTransferObjects;

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

        [HttpGet(Name = "GetMonterActivity")]
        public async Task<Result<RollTableEntryDto>> GetMonterActivity()
            => HandleResult(await Safe.ExecuteAsync(async () => await _dndEncounterBusinessLayer.EncounterRandomGenerator()));
    }
}
