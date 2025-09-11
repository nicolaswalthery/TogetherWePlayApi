using Microsoft.AspNetCore.Mvc;

namespace TWP.Api.Controllers.Interfaces
{
    public interface IEtlController
    {
        Task<IActionResult> AideDdMonstersEtl();
        Task<IActionResult> DndMonsterImageEtl();
    }
}
