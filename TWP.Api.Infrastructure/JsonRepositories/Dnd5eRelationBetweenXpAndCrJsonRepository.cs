using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Infrastructure.JsonRepositories.Interfaces;
using TWP.Api.Core.Enums;

namespace TWP.Api.Infrastructure.JsonRepositories
{
    public class Dnd5eRelationBetweenXpAndCrJsonRepository : JsonRepositoryBase, IDnd5eRelationBetweenXpAndCrJsonRepository
    {
        public Dnd5eRelationBetweenXpAndCrJsonRepository() : base("Dnd5eData", SourceFolderEnum.Dnd5eData, "Dnd5eRelationBetweenXpAndCr")
        {
        }

        public List<Dnd5eRelationBetweenXpAndCrDto> GetAll()
        {
            //TODO: Implement this
            return new List<Dnd5eRelationBetweenXpAndCrDto>();
        }
    }
} 