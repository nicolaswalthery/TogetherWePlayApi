using Microsoft.AspNetCore.Mvc;
using TWP.Api.Core.Enums;

namespace TWP.Api.Controllers.Interfaces
{
    public interface IDndController
    {
        Task<IActionResult> GenerateRandomEncounter(EncounterDifficultyEnum encounterDifficulty, IList<int> playerLevels, string encounterNarrativeContext);
    }
}
