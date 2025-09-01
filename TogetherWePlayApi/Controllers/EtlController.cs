using Common.ResultPattern;
using Microsoft.AspNetCore.Mvc;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Application.ETL;
using TWP.Api.Controllers.Interfaces;
using TWP.Api.Core.Enums;

namespace TogetherWePlayApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EtlController : ControllerBase<EtlController>, IEtlController
    {
        private readonly IExtractTransformLoad _extractTransformLoad;

        public EtlController(IExtractTransformLoad extractTransformLoad, ILogger<EtlController> logger) : base(logger)
        {
            _extractTransformLoad = extractTransformLoad;
        }

        [HttpGet(Name = "AideDdMonstersEtl")]
        public async Task<IActionResult> AideDdMonstersEtl()
            => HandleResult(await Safe.ExecuteAsync(() => _extractTransformLoad.RunAideDdMonster5eEtl()));
    }
}
