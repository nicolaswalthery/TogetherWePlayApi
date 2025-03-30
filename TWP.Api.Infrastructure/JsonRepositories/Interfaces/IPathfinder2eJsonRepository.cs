using TWP.Api.Core.DataTransferObjects;

namespace TWP.Api.Infrastructure.JsonRepositories.Interfaces
{
    public interface IPathfinder2eJsonRepository
    {
        public List<Pf2eMonsterDto> GetAllPf2eCoreMonsters();
    }
}
