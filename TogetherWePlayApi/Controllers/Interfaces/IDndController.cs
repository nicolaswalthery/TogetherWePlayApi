using Microsoft.AspNetCore.Mvc;

namespace TWP.Api.Controllers.Interfaces
{
    public interface IDndController
    {
        public Task<IActionResult> GetMonsterActivity();
    }
}
