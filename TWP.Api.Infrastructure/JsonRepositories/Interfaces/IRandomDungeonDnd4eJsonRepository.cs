using TWP.Api.Infrastructure.DataTransferObjects;

namespace TWP.Api.Infrastructure.JsonRepositories.Interfaces
{
    public interface IRandomDungeonDnd4eJsonRepository
    {
        public RollTableDto GetCorridorsRandomTable();
    }
}
