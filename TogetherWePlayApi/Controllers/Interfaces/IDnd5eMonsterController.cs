using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace TWP.Api.Controllers.Interfaces
{
    public interface IDnd5eMonsterController
    {
        Task<IActionResult> GetAllMonsterStatsCsv();
        Task<IActionResult> GetAllMonsterStatsByCr(int cr);
    }
} 