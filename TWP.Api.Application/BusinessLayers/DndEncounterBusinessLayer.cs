using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Infrastructure.DataTransferObjects;
using TWP.Api.Infrastructure.JsonRepositories.Interfaces;

namespace TWP.Api.Application.BusinessLayers
{
    public class DndEncounterBusinessLayer : IDndEncounterBusinessLayer
    {
        private readonly IMonsterActivitiesJsonRepository _monsterActivitiesJsonRepository;
        private readonly ISomethingHappenJsonRepository _somethingHappenJsonRepository;

        public DndEncounterBusinessLayer(IMonsterActivitiesJsonRepository monsterActivitiesJsonRepository, ISomethingHappenJsonRepository somethingHappenJsonRepository)
        {
            _monsterActivitiesJsonRepository = monsterActivitiesJsonRepository;
            _somethingHappenJsonRepository = somethingHappenJsonRepository;
        }

        public RollTableDto EncounterRandomGenerator()
        {
            return _somethingHappenJsonRepository.GetRollTable();
        }
    }
}
