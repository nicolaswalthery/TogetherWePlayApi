using TWP.Api.Application.BusinessLayers.Interfaces;
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

        public string EncounterRandomGenerator()
        {
            //TEST TO DEL
            var result = _monsterActivitiesJsonRepository.GetRollTable(String.Empty);
            throw new NotImplementedException();
        }
    }
}
