using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.JsonRepositories.Interfaces;
using TWP.Api.Infrastructure.JsonRepositories.Mappers;
using TWP.Api.Infrastructure.JsonRepositories.RepositoryBases;

namespace TWP.Api.Infrastructure.JsonRepositories
{
    public class Dnd5eEncounterDataJsonRepository : Dnd5eDataJsonRepositoryBase, IDnd5eRelationBetweenXpAndCrJsonRepository
    {
        private string _dnd5eRelationBetweenXpAndCrJsonFileName = "Dnd5eRelationBetweenXpAndCr";
        public Dnd5eEncounterDataJsonRepository() : base(SourceFolderEnum.Dnd5eEncounterData)
        {
        }

        public List<Dnd5eRelationBetweenXpAndCrDto> GetAll()
           => base.GetJsonByFileName(fileName: _dnd5eRelationBetweenXpAndCrJsonFileName).ToDnd5eRelationBetweenXpAndCrDtoList();
    }
} 