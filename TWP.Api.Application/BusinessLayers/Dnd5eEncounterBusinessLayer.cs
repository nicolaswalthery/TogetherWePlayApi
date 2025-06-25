using Common.ResultPattern;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Application.Helpers;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.DataTransferObjects;
using TWP.Api.Infrastructure.JsonRepositories.Interfaces;

namespace TWP.Api.Application.BusinessLayers
{
    public class Dnd5eEncounterBusinessLayer : IDndEncounterBusinessLayer
    {
        private readonly IMonsterActivitiesJsonRepository _monsterActivitiesJsonRepository;
        private readonly ISomethingHappenJsonRepository _somethingHappenJsonRepository;
        private readonly IUltraModern5eJsonRepository _ultraModern5EJsonRepository;

        public Dnd5eEncounterBusinessLayer(IMonsterActivitiesJsonRepository monsterActivitiesJsonRepository, ISomethingHappenJsonRepository somethingHappenJsonRepository, IUltraModern5eJsonRepository ultraModern5EJsonRepository)
        {
            _monsterActivitiesJsonRepository = monsterActivitiesJsonRepository;
            _somethingHappenJsonRepository = somethingHappenJsonRepository;
            _ultraModern5EJsonRepository = ultraModern5EJsonRepository;
        }

        public async Task<Result<RollTableEntryDto>> EncounterRandomGenerator()
            => await Safe.ExecuteAsync(async () =>
            {
                var result = _ultraModern5EJsonRepository.GetTechItemTable_A_RandomTable().GetRandomlyASingleEntry()
                                                                                          .Verify(data => data.IsNull());
                if (result.IsFailure)
                    return Result<RollTableEntryDto>.Failure(result.Error!, result.ReasonType);

                return result;
            });

        /// <summary>
        /// Compute the experience budget for a party
        /// </summary>
        /// <returns></returns>
        private async Task<Result<int>> ComputeExpBudget(EncounterDifficultyEnum encounterDifficulty, IList<int> playerLevels)
            => await Safe.ExecuteAsync(async () =>
            {
                //TODO
            });
    }
}
