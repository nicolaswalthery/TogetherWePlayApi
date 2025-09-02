using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Core.DbEntities;

namespace TWP.Api.Application.ETL.Services
{
    public interface IActionMapperService
    {
        Task<List<ActionDbEntity>> MapMonsterActionsAsync(AideDdMonsterResponseDto monsterDto, Guid monsterId);
    }
}
