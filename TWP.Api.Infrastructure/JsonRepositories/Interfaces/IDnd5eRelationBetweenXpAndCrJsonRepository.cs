using System.Collections.Generic;
using TWP.Api.Core.DataTransferObjects;

namespace TWP.Api.Infrastructure.JsonRepositories.Interfaces
{
    public interface IDnd5eRelationBetweenXpAndCrJsonRepository
    {
        List<Dnd5eRelationBetweenXpAndCrDto> GetAll();
    }
} 