using TWP.Api.Infrastructure.JsonRepositories.Interfaces;
using Common.Extensions;
using TWP.Api.Infrastructure.JsonRepositories.Mappers;
using TWP.Api.Infrastructure.DataTransferObjects;

namespace TWP.Api.Infrastructure.JsonRepositories
{
    /// <summary>
    /// The Json Repositories have the responsability to retreive and map the json data to a data transfer object
    /// </summary>
    public class MonsterActivitiesJsonRepository : JsonRepositoryBase, IMonsterActivitiesJsonRepository
    {
        private readonly string _fileName = "MonsterActivitiesRandomTable";
        public MonsterActivitiesJsonRepository() : base("Shadowdark")
        {   
        }

        public RollTableDto GetRollTable() 
            => _fileName.GetJsonFile(base.FullRandomTablesRelativePath).ToDto();
    }
}
