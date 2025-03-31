using TWP.Api.Core.DataTransferObjects;

namespace TWP.Api.Infrastructure.JsonRepositories.Interfaces
{
    public interface IPathfinder2eMonsterCoreJsonRepository
    {
        public List<Pf2eMonsterDto> GetAllPf2eCoreMonsters();
    }
}
