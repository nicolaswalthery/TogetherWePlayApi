using TWP.Api.Core.DataTransferObjects;

namespace TWP.Api.Infrastructure.Interops.Interfaces
{
    public interface IMonsterApiInterops
    {
        Task<MonsterApiResponseDto> GetMonsterDataAsync();
        Task<SpellApiResponseDto> GetSpellDataAsync();
        Task<MonsterApiResponseDto> GetMonstersByChallengeRatingAsync(int cr);
    }
}
