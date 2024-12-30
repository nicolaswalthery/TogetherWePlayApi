using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Infrastructure.DataTransferObjects;
using TWP.Api.Infrastructure.JsonRepositories.Interfaces;

namespace TWP.Api.Application.BusinessLayers
{
    public class DndEncounterBusinessLayer : IDndEncounterBusinessLayer
    {
        private readonly IMonsterActivitiesJsonRepository _monsterActivitiesJsonRepository;
        public DndEncounterBusinessLayer(IMonsterActivitiesJsonRepository monsterActivitiesJsonRepository)
        {
            _monsterActivitiesJsonRepository = monsterActivitiesJsonRepository;
        }

        public RollTableDto EncounterRandomGenerator()
        {
            return _monsterActivitiesJsonRepository.GetRollTable();
        }
    }
}
