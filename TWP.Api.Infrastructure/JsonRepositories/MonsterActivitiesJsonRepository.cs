using TWP.Api.Infrastructure.JsonRepositories.Interfaces;
using Common.Extensions;
using TWP.Api.Infrastructure.JsonRepositories.Mappers;
using TWP.Api.Infrastructure.DataTransferObjects;
using TWP.Api.Core.Enums;

namespace TWP.Api.Infrastructure.JsonRepositories
{
    /// <summary>
    /// The Json Repositories have the responsability to retreive and map the json data to a data transfer object
    /// </summary>
    public class MonsterActivitiesJsonRepository : JsonRepositoryBase, IMonsterActivitiesJsonRepository
    {
        public MonsterActivitiesJsonRepository() : base(folderName: SourceEnum.Shadowdark, fileName: "MonsterActivitiesRandomTable")
        {   
        }

        public RollTableDto GetRollTable() 
            => base.GetRollTable().ToMonsterActivitiesRollTableDto();
    }
}
