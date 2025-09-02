using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Core.DbEntities;

namespace TWP.Api.Application.ETL.Services
{
    public interface ITraitMapperService
    {
        Task<List<TraitDbEntity>> MapMonsterTraitsAsync(AideDdMonsterResponseDto monsterDto, Guid monsterId);
    }
}
