using Common.ResultPattern;
using TWP.Api.Core.DbEntities;

namespace TWP.Api.Core.Interface.Infrastructure
{
    /// <summary>
    /// Repository interface for Monster Building Guidelines
    /// </summary>
    public interface IMonsterBuildingGuidelineRepository
    {
        Task<Result<MonsterBuildingGuidelineDbEntity>> GetByExpAsync(float cr);
        Task<Result<MonsterBuildingGuidelineDbEntity>> GetByNumericCRAsync(float crNumeric);
        Task<Result<List<MonsterBuildingGuidelineDbEntity>>> GetAllActiveAsync();
        Task<Result<List<MonsterBuildingGuidelineDbEntity>>> GetRangeAsync(float minCR, float maxCR);
        Task<Result<MonsterBuildingGuidelineDbEntity>> GetNearestAsync(float crNumeric);
        Task<Result> InsertOrUpdateAsync(MonsterBuildingGuidelineDbEntity guideline);
        Task<Result> SeedDefaultGuidelinesAsync();
        Task<Result<MonsterBuildingGuidelineDbEntity>> GetByExpAsync(int exp);
    }
}
