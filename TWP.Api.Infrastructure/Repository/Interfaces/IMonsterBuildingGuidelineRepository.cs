using Common.ResultPattern;
using TWP.Api.Core.DbEntities;

namespace TWP.Api.Infrastructure.Repository.Interfaces
{
    /// <summary>
    /// Repository interface for Monster Building Guidelines
    /// </summary>
    public interface IMonsterBuildingGuidelineRepository
    {
        Task<Result<MonsterBuildingGuidelineDbEntity>> GetByCRAsync(float cr);
        Task<Result<MonsterBuildingGuidelineDbEntity>> GetByNumericCRAsync(float crNumeric);
        Task<Result<List<MonsterBuildingGuidelineDbEntity>>> GetAllActiveAsync();
        Task<Result<List<MonsterBuildingGuidelineDbEntity>>> GetRangeAsync(float minCR, float maxCR);
        Task<Result<MonsterBuildingGuidelineDbEntity>> GetNearestAsync(float crNumeric);
        Task<Result> InsertOrUpdateAsync(MonsterBuildingGuidelineDbEntity guideline);
        Task<Result> SeedDefaultGuidelinesAsync();
    }
}
