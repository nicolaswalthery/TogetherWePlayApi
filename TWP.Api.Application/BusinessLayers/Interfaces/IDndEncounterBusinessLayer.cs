using Common.ResultPattern;
using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Core.Enums;

namespace TWP.Api.Application.BusinessLayers.Interfaces
{
    public interface IDndEncounterBusinessLayer
    {
        Task<Result<List<Dnd5eMonsterDto>>> EncounterRandomGenerator(EncounterDifficultyEnum encounterDifficulty, IList<int> playerLevels, string encounterNarrativeContextIList, IList<MonsterHabitatEnum> monsterHabitats);
    }
}
