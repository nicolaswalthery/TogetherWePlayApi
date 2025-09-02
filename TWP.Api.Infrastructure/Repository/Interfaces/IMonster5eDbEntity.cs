using Common.ResultPattern;
using TWP.Api.Core.DbEntities;

namespace TWP.Api.Infrastructure.Repository.Interfaces
{
    public interface IMonster5eRepository
    {
        Task<Result<List<Monster5eDbEntity>>> GetAllAsync();
        Task<Result> Insert(Monster5eDbEntity monster5EDbEntity);
        Task<Result> InsertMany(List<Monster5eDbEntity> monster5EDbEntities);
    }
}
