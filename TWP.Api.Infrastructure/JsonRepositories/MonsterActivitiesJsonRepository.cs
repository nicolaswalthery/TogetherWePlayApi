using TWP.Api.Infrastructure.DbEntities;
using TWP.Api.Infrastructure.JsonRepositories.Interfaces;

namespace TWP.Api.Infrastructure.JsonRepositories
{
    /// <summary>
    /// The Json Repositories have the responsability to retreive and map the json data to a data transfer object
    /// </summary>
    public class MonsterActivitiesJsonRepository : IJsonRepository<RollTableEntryDto>
    {
        /// <summary>
        /// Get the json file in memory from a folder path
        /// </summary>
        /// <param name="folderPath">The folder path</param>
        /// <returns>The json file</returns>
        public string GetJsonFile(string folderPath)
        {
            throw new NotImplementedException();
        }

        public IList<RollTableEntryDto> GetAll(string json)
        {
            throw new NotImplementedException();
        }
    }
}
