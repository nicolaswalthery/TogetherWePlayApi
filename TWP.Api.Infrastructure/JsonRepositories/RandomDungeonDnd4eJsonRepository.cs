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
    public class RandomDungeonDnd4eJsonRepository : JsonRepositoryBase, IRandomDungeonDnd4eJsonRepository
    {
        public RandomDungeonDnd4eJsonRepository() : base(folderName: SourceFolderEnum.Dnd4e)
        {   
        }

        public RollTableDto GetCorridorsRandomTable()
            => base.GetRollTable(fileName: "RandomDungeonCorridorsTable").ToRandomDungeonCorridorsTableRollTableDto();

    }
}
