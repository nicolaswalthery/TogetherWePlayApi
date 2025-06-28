using Common.ResultPattern;
using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Core.Enums;

namespace TWP.Api.Application.BusinessLayers.Interfaces
{
    public interface IDndEncounterBusinessLayer
    {
        Task<Result<List<Dnd5eMonsterDto>>> EncounterRandomGenerator(EncounterDifficultyEnum encounterDifficulty, IList<int> playerLevels, string encounterNarrativeContextIList, IList<MonsterHabitatEnum> monsterHabitats);
        
        /// <summary>
        /// Gets formatted text representation of an encounter
        /// </summary>
        /// <param name="encounterDifficulty">Difficulty of the encounter</param>
        /// <param name="playerLevels">Player levels</param>
        /// <param name="encounterNarrativeContext">Narrative context</param>
        /// <param name="monsterHabitats">Monster habitats</param>
        /// <param name="formatType">Type of formatting (Full, Summary, or List)</param>
        /// <returns>Formatted text representation of the encounter</returns>
        Task<Result<string>> GetFormattedEncounterAsync(EncounterDifficultyEnum encounterDifficulty, IList<int> playerLevels, string encounterNarrativeContext, IList<MonsterHabitatEnum> monsterHabitats, string formatType = "Summary");
    }
}
