using Microsoft.AspNetCore.Mvc;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Controllers.Interfaces;
using TWP.Api.Infrastructure.DataTransferObjects;

namespace TogetherWePlayApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DndController : ControllerBase, IDndController
    {

        private readonly ILogger<DndController> _logger;
        private readonly IDndEncounterBusinessLayer _dndEncounterBusinessLayer;

        public DndController(ILogger<DndController> logger, IDndEncounterBusinessLayer dndEncounterBusinessLayer)
        {
            _logger = logger;
            _dndEncounterBusinessLayer = dndEncounterBusinessLayer;
        }

        [HttpGet(Name = "GetMonterActivity")]
        public RollTableDto GetMonterActivity()
        {
            return _dndEncounterBusinessLayer.EncounterRandomGenerator();
        }
    }
}
