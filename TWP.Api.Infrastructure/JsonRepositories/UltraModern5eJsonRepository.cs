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

        private string _shootAndLootCompanyDataJsonFileName = "ShootAndLootCompanyData";
        private string _shootAndLootLineDataJsonFileName = "ShootAndLootLineData";
        private string _shootAndLootSelfShieldBenefitsJsonFileName = "ShootAndLootSelfShieldBenefits";

        public UltraModern5eJsonRepository() : base(folderName: SourceEnum.UM5e)
        {
        }

        #region Treasure Tables
        public RollTableDto GetTechItemTable_A_RandomTable() 
            => base.GetRollTable(fileName: "TechItemTableARandomTable").ToTechItemTableARandomTableRollTableDto();

        #endregion Treasure Tables

        #region Company Data Tables
        public RollTableDto GetShootAndLootCompanyName()
            => base.GetRollTable(fileName: _shootAndLootCompanyDataJsonFileName).ToShootAndLootCompanyNameRollTableDto();

        public RollTableDto GetShootAndLootDamageType()
            => base.GetRollTable(fileName: _shootAndLootCompanyDataJsonFileName).ToShootAndLootDamageTypeRollTableDto();

        public RollTableDto GetShootAndLootMagazine()
            => base.GetRollTable(fileName: _shootAndLootCompanyDataJsonFileName).ToShootAndLootMagazineRollTableDto();

        public RollTableDto GetShootAndLootTechLevel()
            => base.GetRollTable(fileName: _shootAndLootCompanyDataJsonFileName).ToShootAndLootTechLevelRollTableDto();

        #endregion Company Data Tables
        
        #region Line Data

        public RollTableDto GetShootAndLootLineData()
            => base.GetRollTable(fileName: _shootAndLootLineDataJsonFileName).ToShootAndLootLineRollTableDto();

        public RollTableDto GetShootAndLootAdditionalProperty()
            => base.GetRollTable(fileName: _shootAndLootLineDataJsonFileName).ToShootAndLootAdditionalPropertyRollTableDto();

        #endregion Line Data Tables

        #region Model Data

        public RollTableDto GetShootAndLootModelName()
            => base.GetRollTable(fileName: _shootAndLootLineDataJsonFileName).ToShootAndLootModelNameRollTableDto();

        public RollTableDto GetShootAndLootModelBenefit()
            => base.GetRollTable(fileName: _shootAndLootLineDataJsonFileName).ToShootAndLootModelBenefitRollTableDto();

        #endregion Model Data

        #region Shield Benefits Data

        public RollTableDto GetShootAndLootShieldBenefits()
            => base.GetRollTable(fileName: _shootAndLootSelfShieldBenefitsJsonFileName).ToShootAndLootShieldBenefitsRollTableDto();

        #endregion Shield Benefits Data

    }
}
