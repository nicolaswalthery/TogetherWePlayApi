using Common.Randomizer;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Infrastructure.JsonRepositories.Interfaces;

namespace TWP.Api.Application.BusinessLayers
{
    public class Pathfinder2eBusinessLayer : IPathfinder2eBusinessLayer
    {
        private IPathfinder2eJsonRepository _pathfinder2EJsonRepository;

        public Pathfinder2eBusinessLayer(IPathfinder2eJsonRepository pathfinder2EJsonRepository)
        {
            _pathfinder2EJsonRepository = pathfinder2EJsonRepository;
        }

        public Pf2eMonsterDto GetOneRandomPf2eCoreMonster()
        {
            var randomSelector = new RandomSelector<Pf2eMonsterDto>();
            return randomSelector.SelectOneRandomly(_pathfinder2EJsonRepository.GetAllPf2eCoreMonsters().ToArray());
        }
    }
}
