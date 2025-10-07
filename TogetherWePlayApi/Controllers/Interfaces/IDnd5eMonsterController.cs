using Microsoft.AspNetCore.Mvc;

namespace TWP.Api.Controllers.Interfaces
{
    public interface IDnd5eMonsterController
    {
        Task<IActionResult> GetAll5eMonsters();
        Task<IActionResult> GetAllMonsterStatsByCr(int cr);
        Task<IActionResult> CreateOriginalDndMonster(float challengeRating);
    }
} 