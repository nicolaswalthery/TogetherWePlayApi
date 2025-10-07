using TWP.Api.Core.DataTransferObjects;

namespace TWP.Api.Application.Interfaces.Services
{
    public interface IAideDdInterops
    {
        Task<AideDdMonsterResponseDto> GetMonsterByName(string monsterName);
    }
}
