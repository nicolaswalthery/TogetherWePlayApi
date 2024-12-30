using TWP.Api.Infrastructure.JsonRepositories.Interfaces;

namespace TWP.Api.Infrastructure.JsonRepositories
{
    public class MonsterActivitiesJsonRepository : IJsonRepository<string>
    {
        public string Get(string fileName)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetAll(string folderPath)
        {
            throw new NotImplementedException();
        }
    }
}
