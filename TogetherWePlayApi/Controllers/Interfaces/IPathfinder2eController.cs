using TWP.Api.Core.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace TWP.Api.Controllers.Interfaces
{
    public interface IPathfinder2eController
    {
        public Task<IActionResult> GetOneRandomCoreMonster();
        public Task<IActionResult> GetOneRandomCondition();
    }
}
