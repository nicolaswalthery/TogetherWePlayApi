using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.DataTransferObjects;
using TWP.Api.Infrastructure.JsonRepositories.Interfaces;
using TWP.Api.Infrastructure.JsonRepositories.Mappers;
using static TWP.Api.Infrastructure.JsonRepositories.Mappers.ShootAndLootCompanyDataJsonFileMapper;

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

        #region Treasure Tables
        public RollTableDto GetTechItemTable_A_RandomTable() 
            => base.GetRollTable(fileName: "TechItemTableARandomTable").ToTechItemTableARandomTableRollTableDto();

        #endregion Treasure Tables

        #region Company Data Tables
        public RollTableDto GetShootAndLootCompanyName()
            => base.GetRollTable(fileName: "ShootAndLootCompanyData").ToShootAndLootCompanyNameRollTableDto();

        public RollTableDto GetShootAndLootDamageType()
            => base.GetRollTable(fileName: "ShootAndLootCompanyData").ToShootAndLootDamageTypeRollTableDto();

        public RollTableDto GetShootAndLootMagazine()
            => base.GetRollTable(fileName: "ShootAndLootCompanyData").ToShootAndLootMagazineRollTableDto();

        public RollTableDto GetShootAndLootTechLevel()
            => base.GetRollTable(fileName: "ShootAndLootCompanyData").ToShootAndLootTechLevelRollTableDto();

        #endregion Company Data Tables
        
        #region Line Data

        public RollTableDto GetShootAndLootLineData()
            => base.GetRollTable(fileName: "ShootAndLootLineData").ToShootAndLootLineRollTableDto();

        public RollTableDto GetShootAndLootAdditionalProperty()
            => base.GetRollTable(fileName: "ShootAndLootLineData").ToShootAndLootAdditionalPropertyRollTableDto();

        #endregion Line Data Tables
    }
}
