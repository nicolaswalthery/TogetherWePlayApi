using TWP.Api.Core.DataTransferObjects;

namespace TWP.Api.Core.Interfaces.Infrastructure.JsonRepositories
{
    public interface IPathfinder2eMonsterCoreJsonRepository
    {
        public List<Pf2eMonsterDto> GetAllPf2eCoreMonsters();
    }
}
