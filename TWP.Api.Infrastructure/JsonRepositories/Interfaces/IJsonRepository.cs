using TWP.Api.Infrastructure.DataTransferObjects;

namespace TWP.Api.Infrastructure.JsonRepositories.Interfaces
{
    public interface IJsonRepository
    {
        /// <summary>
        /// Get a roll table.
        /// </summary>
        /// <returns>The RollTableDto representing a random table</returns>
        RollTableDto GetRollTable();
    }
}
