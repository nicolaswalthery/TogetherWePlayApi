using System.IO.Enumeration;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.DataTransferObjects;
using TWP.Api.Infrastructure.JsonRepositories.Interfaces;
using TWP.Api.Infrastructure.JsonRepositories.Mappers;

namespace TWP.Api.Infrastructure.JsonRepositories
{
    /// <summary>
    /// The Json Repositories have the responsability to retreive and map the json data to a data transfer object
    /// </summary>
    public class UltraModern5eJsonRepository : JsonRepositoryBase, IUltraModern5eJsonRepository
    {
        public UltraModern5eJsonRepository() : base(folderName: SourceEnum.UM5e)
        {
        }

        public RollTableDto GetTechItemTable_A_RandomTable() 
            => base.GetRollTable(fileName: "TechItemTableARandomTable").ToTechItemTableARandomTableRollTableDto();
    }
}
