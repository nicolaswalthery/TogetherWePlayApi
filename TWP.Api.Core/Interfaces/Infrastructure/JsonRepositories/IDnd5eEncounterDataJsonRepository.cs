using System.Collections.Generic;
using TWP.Api.Core.DataTransferObjects;

namespace TWP.Api.Core.Interfaces.Infrastructure.JsonRepositories
{
    public interface IDnd5eEncounterDataJsonRepository
    {
        List<Dnd5eRelationBetweenXpAndCrDto> GetAll();
    }
} 