using Common.ResultPattern;
using TWP.Api.Application.DataTransferObjects;
using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Core.Enums;

namespace TWP.Api.Application.BusinessLayers.Interfaces
{
    public interface IDndEncounterBusinessLayer
    {
        Task<Result<Dnd5eEncounterGeneratedDto>> EncounterRandomGenerator(EncounterDifficultyEnum encounterDifficulty, IList<int> playerLevels, string encounterNarrativeContextIList, MonsterHabitatEnum monsterHabitats, bool generateWithEncounterTemplate = false);

        Task<Result<List<Monster5eDto>>> CreateScifiAdversaries(string narrativeDescription, CombatRoleEnum? forcedCombatRole = null);
    }
}
