using Common.Randomizer;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Infrastructure.JsonRepositories.Interfaces;
using Common.ResultPattern;

namespace TWP.Api.Application.BusinessLayers
{
    public class Pathfinder2eBusinessLayer : IPathfinder2eBusinessLayer
    {
        private readonly IPathfinder2eMonsterCoreJsonRepository _pathfinder2eCoreMonsterJsonRepository;
        private readonly IPathfinder2eConditionsJsonRepository _pathfinder2eConditionsJsonRepository;

        public Pathfinder2eBusinessLayer(IPathfinder2eMonsterCoreJsonRepository pathfinder2eJsonRepository, IPathfinder2eConditionsJsonRepository pathfinder2EConditionsJsonRepository)
        {
            _pathfinder2eCoreMonsterJsonRepository = pathfinder2eJsonRepository;
            _pathfinder2eConditionsJsonRepository = pathfinder2EConditionsJsonRepository;
        }

        public async Task<Result<Pf2eMonsterDto>> GetOneRandomPf2eCoreMonster()
            => await Safe.ExecuteAsync(async () =>
            {
                var randomSelector = new RandomSelector<Pf2eMonsterDto>();
                var result = randomSelector.SelectOneRandomly(_pathfinder2eCoreMonsterJsonRepository.GetAllPf2eCoreMonsters().ToArray())
                                          .Verify(data => data.IsNull());
                if (result.IsFailure)
                    return Result<Pf2eMonsterDto>.Failure(result.Error!, result.ReasonType);
                return result;
            });
    

        public async Task<Result<Pf2eConditionDto>> GetOneRandomPf2eCondition()
            =>  await Safe.ExecuteAsync(async () =>
            {
                var randomSelector = new RandomSelector<Pf2eConditionDto>();
                var result = randomSelector.SelectOneRandomly(_pathfinder2eConditionsJsonRepository.GetAll().ToArray())
                                          .Verify(data => data.IsNull());
                if (result.IsFailure)
                    return Result<Pf2eConditionDto>.Failure(result.Error!, result.ReasonType);
                return result;
            });
    }   
}
