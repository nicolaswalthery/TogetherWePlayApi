using Common.Randomizer;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Infrastructure.JsonRepositories.Interfaces;

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

        public Pf2eMonsterDto GetOneRandomPf2eCoreMonster()
        {
            var randomSelector = new RandomSelector<Pf2eMonsterDto>();
            return randomSelector.SelectOneRandomly(_pathfinder2eCoreMonsterJsonRepository.GetAllPf2eCoreMonsters().ToArray());
        }

        public Pf2eConditionDto GetOneRandomPf2eCondition()
        {
            var randomSelector = new RandomSelector<Pf2eConditionDto>();
            return randomSelector.SelectOneRandomly(_pathfinder2eConditionsJsonRepository.GetAll().ToArray());
        }
    }
}
