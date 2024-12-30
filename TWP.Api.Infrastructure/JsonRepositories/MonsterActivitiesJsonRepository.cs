using TWP.Api.Infrastructure.DbEntities;
using TWP.Api.Infrastructure.JsonRepositories.Interfaces;

namespace TWP.Api.Infrastructure.JsonRepositories
{
    /// <summary>
    /// The Json Repositories have the responsability to retreive and map the json data to a data transfer object
    /// </summary>
    public class MonsterActivitiesJsonRepository : IJsonRepository
    {
        public string GetJsonFile(string folderPath)
        {
            throw new NotImplementedException();
        }

        public RollTableDto GetRollTable(string json)
        {
            throw new NotImplementedException();
        }
    }
}
