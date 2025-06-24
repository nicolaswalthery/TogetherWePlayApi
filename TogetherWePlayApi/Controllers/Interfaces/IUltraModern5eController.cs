using TWP.Api.Application.DataTransferObjects;
using TWP.Api.Infrastructure.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;

namespace TWP.Api.Controllers.Interfaces
{
    public interface IUltraModern5eController
    {
        public Task<IActionResult> GetShootAndLoot();
    }
}
