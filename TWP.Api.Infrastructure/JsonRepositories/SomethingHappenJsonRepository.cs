using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.DataTransferObjects;
using TWP.Api.Infrastructure.JsonRepositories.Interfaces;
using TWP.Api.Infrastructure.JsonRepositories.Mappers;

namespace TWP.Api.Infrastructure.JsonRepositories
{
    /// <summary>
    /// The Json Repositories have the responsability to retreive and map the json data to a data transfer object
    /// </summary>
    public class SomethingHappenJsonRepository : JsonRepositoryBase, ISomethingHappenJsonRepository
    {
        public SomethingHappenJsonRepository() : base(folderName: SourceFolderEnum.Shadowdark, fileName: "SomethingHappensRandomTable")
        {
        }

        public RollTableDto GetRollTable() 
            => base.GetRollTable().ToSomethingHappensRollTableDto();
    }
}
