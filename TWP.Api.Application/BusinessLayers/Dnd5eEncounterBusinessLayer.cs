using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Application.Helpers;
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

        public RollTableEntryDto EncounterRandomGenerator()
        {
            return _ultraModern5EJsonRepository.GetTechItemTable_A_RandomTable().GetRandomlyASingleEntry();
        }
    }
}
