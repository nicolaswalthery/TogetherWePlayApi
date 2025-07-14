using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.DataTransferObjects;
using TWP.Api.Infrastructure.JsonRepositories.Interfaces;
using TWP.Api.Infrastructure.JsonRepositories.Mappers;

namespace TWP.Api.Infrastructure.JsonRepositories
{
    /// <summary>
    /// The Json Repositories have the responsability to retreive and map the json data to a data transfer object
    /// </summary>
    public class RandomDungeonDnd4eJsonRepository : RandomTableJsonRepositoryBase, IRandomDungeonDnd4eJsonRepository
    {
        public RandomDungeonDnd4eJsonRepository() : base(folderName: SourceFolderEnum.Dnd4e)
        {   
        }

        public RollTableDto GetCorridorsRandomTable()
            => base.GetJsonByFileName(fileName: "RandomDungeonCorridorsTable").ToRandomDungeonCorridorsTableRollTableDto();

    }
}
