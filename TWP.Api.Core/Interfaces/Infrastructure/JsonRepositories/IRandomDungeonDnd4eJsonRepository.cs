using TWP.Api.Infrastructure.DataTransferObjects;

namespace TWP.Api.Core.Interfaces.Infrastructure.JsonRepositories
{
    public interface IRandomDungeonDnd4eJsonRepository
    {
        public RollTableDto GetCorridorsRandomTable();
    }
}
