using TWP.Api.Infrastructure.DbEntities;

namespace TWP.Api.Infrastructure.JsonRepositories.Interfaces
{
    public interface IJsonRepository
    {
        /// <summary>
        /// Get the json file in memory from a folder path
        /// </summary>
        /// <param name="folderPath">The folder path</param>
        /// <returns>The json file</returns>
        string GetJsonFile(string folderPath);

        /// <summary>
        /// Get all json file entries and map them to a RollTableDto.
        /// </summary>
        /// <param name="json">json</param>
        /// <returns>The json file</returns>
        RollTableDto GetRollTable(string json);
    }
}
