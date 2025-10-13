using Common.ResultPattern;
using Common.Security;
using Microsoft.AspNetCore.Mvc;
using TWP.Api.Application.ETL;
using TWP.Api.Controllers.Interfaces;

namespace TogetherWePlayApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [RequiredApiKey]
    public class EtlController : ControllerBase<EtlController>, IEtlController
    {
        private readonly IExtractTransformLoad _extractTransformLoad;

        public EtlController(IExtractTransformLoad extractTransformLoad, ILogger<EtlController> logger) : base(logger)
        {
            _extractTransformLoad = extractTransformLoad;
        }

        [HttpPost(Name = "AideDdMonstersEtl")]
        public async Task<IActionResult> AideDdMonstersEtl()
            => HandleResult(await Safe.ExecuteAsync(() => _extractTransformLoad.RunAideDdMonster5eEtl()));

        [HttpPost("DndMonsterImageEtl")]
        public async Task<IActionResult> DndMonsterImageEtl()
            => HandleResult(await Safe.ExecuteAsync(() => _extractTransformLoad.ImportMonstersFromImagesAsync()));
    }
}
