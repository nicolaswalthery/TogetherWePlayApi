using Common.ResultPattern;
using TWP.Api.Core.DbEntities;

namespace TWP.Api.Core.Interface.Infrastructure
{
    public interface IAideDdMonster5eRepository
    {
        Task<Result<List<MonsterAideddDbEntity>>> GetAllAsync();
        Task<Result<List<MonsterAideddDbEntity>>> FindByCrOrLessAsync(double cr);
    }
}
