using TWP.Api.Core.DataTransferObjects;

namespace TWP.Api.Infrastructure.Interops.Interfaces
{
    public interface IAideDdInterops
    {
        Task<AideDdMonsterResponseDto> GetMonsterByName(string monsterName);
    }
}
