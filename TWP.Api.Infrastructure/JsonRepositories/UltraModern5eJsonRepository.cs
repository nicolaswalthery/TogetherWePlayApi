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
    public class UltraModern5eJsonRepository : RandomTableJsonRepositoryBase, IUltraModern5eJsonRepository
    {
        private string _shootAndLootCompanyDataJsonFileName = "ShootAndLootCompanyData";
        private string _shootAndLootLineDataJsonFileName = "ShootAndLootLineData";
        private string _shootAndLootModelDataJsonFileName = "ShootAndLootModelData";
        private string _shootAndLootSelfShieldBenefitsJsonFileName = "ShootAndLootSelfShieldBenefits";
        private string _shootAndLootWeaponRaritiesJsonFileName = "ShootAndLootWeaponRarities";
        private string _shootAndLootWeaponTypeJsonFileName = "ShootAndLootWeaponTypes";

        public UltraModern5eJsonRepository() : base(folderName: SourceFolderEnum.UM5e)
        {
        }

        #region Treasure Tables
        public RollTableDto GetTechItemTable_A_RandomTable() 
            => base.GetJsonByFileName(fileName: "TechItemTableARandomTable").ToTechItemTableARandomTableRollTableDto();

        public RollTableDto GetTechItemTable_B_RandomTable()
            => base.GetJsonByFileName(fileName: "TechItemTableBRandomTable").ToTechItemTableBRandomTableRollTableDto();

        public RollTableDto GetTechItemTable_C_RandomTable()
            => base.GetJsonByFileName(fileName: "TechItemTableCRandomTable").ToTechItemTableCRandomTableRollTableDto();

        public RollTableDto GetTechItemTable_D_RandomTable()
            => base.GetJsonByFileName(fileName: "TechItemTableDRandomTable").ToTechItemTableDRandomTableRollTableDto();

        public RollTableDto GetTechItemTable_E_RandomTable()
            => base.GetJsonByFileName(fileName: "TechItemTableERandomTable").ToTechItemTableERandomTableRollTableDto();

        public RollTableDto GetTechItemTable_F_RandomTable()
            => base.GetJsonByFileName(fileName: "TechItemTableFRandomTable").ToTechItemTableFRandomTableRollTableDto();

        public RollTableDto GetTechItemTable_G_RandomTable()
            => base.GetJsonByFileName(fileName: "TechItemTableGRandomTable").ToTechItemTableGRandomTableRollTableDto();

        public RollTableDto GetTechItemTable_H_RandomTable()
            => base.GetJsonByFileName(fileName: "TechItemTableHRandomTable").ToTechItemTableHRandomTableRollTableDto();

        public RollTableDto GetTechItemTable_I_RandomTable()
            => base.GetJsonByFileName(fileName: "TechItemTableIRandomTable").ToTechItemTableIRandomTableRollTableDto();


        #endregion Treasure Tables

        #region Company Data Tables
        public RollTableDto GetShootAndLootCompanyName()
            => base.GetJsonByFileName(fileName: _shootAndLootCompanyDataJsonFileName).ToShootAndLootCompanyNameRollTableDto();

        public RollTableDto GetShootAndLootDamageType()
            => base.GetJsonByFileName(fileName: _shootAndLootCompanyDataJsonFileName).ToShootAndLootDamageTypeRollTableDto();

        public RollTableDto GetShootAndLootMagazine()
            => base.GetJsonByFileName(fileName: _shootAndLootCompanyDataJsonFileName).ToShootAndLootMagazineRollTableDto();

        public RollTableDto GetShootAndLootTechLevel()
            => base.GetJsonByFileName(fileName: _shootAndLootCompanyDataJsonFileName).ToShootAndLootTechLevelRollTableDto();

        #endregion Company Data Tables
        
        #region Line Data

        public RollTableDto GetShootAndLootLineData()
            => base.GetJsonByFileName(fileName: _shootAndLootLineDataJsonFileName).ToShootAndLootLineRollTableDto();

        public RollTableDto GetShootAndLootAdditionalProperty()
            => base.GetJsonByFileName(fileName: _shootAndLootLineDataJsonFileName).ToShootAndLootAdditionalPropertyRollTableDto();

        #endregion Line Data Tables

        #region Model Data

        public RollTableDto GetShootAndLootModelName()
            => base.GetJsonByFileName(fileName: _shootAndLootModelDataJsonFileName).ToShootAndLootModelNameRollTableDto();

        public RollTableDto GetShootAndLootModelBenefit()
            => base.GetJsonByFileName(fileName: _shootAndLootModelDataJsonFileName).ToShootAndLootModelBenefitsRollTableDto();

        #endregion Model Data

        #region Shield Benefits Data

        public RollTableDto GetShootAndLootShieldBenefits()
            => base.GetJsonByFileName(fileName: _shootAndLootSelfShieldBenefitsJsonFileName).ToShootAndLootShieldBenefitsRollTableDto();

        #endregion Shield Benefits Data

        #region Weapon Rarities Data

        public RollTableDto GetShootAndLootWeaponRarities()
            => base.GetJsonByFileName(fileName: _shootAndLootWeaponRaritiesJsonFileName).ToShootAndLootWeaponRarityRollTableDto();

        public RollTableDto GetShootAndLootWeaponCostMultipliers()
            => base.GetJsonByFileName(fileName: _shootAndLootWeaponRaritiesJsonFileName).ToShootAndLootWeaponCostMultipliersRollTableDto();

        public RollTableDto GetShootAndLootWeaponNumberOfLines()
            => base.GetJsonByFileName(fileName: _shootAndLootWeaponRaritiesJsonFileName).ToShootAndLootWeaponNumberOfLinesRollTableDto();

        public RollTableDto GetShootAndLootWeaponNumberOfModels()
            => base.GetJsonByFileName(fileName: _shootAndLootWeaponRaritiesJsonFileName).ToShootAndLootWeaponNumberOfModelsRollTableDto();

        public RollTableDto GetShootAndLootWeaponBenefits()
            => base.GetJsonByFileName(fileName: _shootAndLootWeaponRaritiesJsonFileName).ToShootAndLootWeaponBenefitsRollTableDto();

        #endregion Weapon Rarities Data

        #region Weapon Type Data

        public RollTableDto GetShootAndLootWeaponType()
            => base.GetJsonByFileName(fileName: _shootAndLootWeaponTypeJsonFileName).ToShootAndLootWeaponTypeRollTableDto();

        public RollTableDto GetShootAndLootWeaponWeight()
            => base.GetJsonByFileName(fileName: _shootAndLootWeaponTypeJsonFileName).ToShootAndLootWeaponWeightRollTableDto();

        public RollTableDto GetShootAndLootWeaponProperties()
            => base.GetJsonByFileName(fileName: _shootAndLootWeaponTypeJsonFileName).ToShootAndLootWeaponPropertiesRollTableDto();

        public RollTableDto GetShootAndLootWeaponBaseCost()
            => base.GetJsonByFileName(fileName: _shootAndLootWeaponTypeJsonFileName).ToShootAndLootWeaponBaseCostRollTableDto();

        public RollTableDto GetShootAndLootWeaponDamage()
            => base.GetJsonByFileName(fileName: _shootAndLootWeaponTypeJsonFileName).ToShootAndLootDamageRollTableDto();

        public RollTableDto GetShootAndLootWeaponRange()
            => base.GetJsonByFileName(fileName: _shootAndLootWeaponTypeJsonFileName).ToShootAndLootWeaponRangeRollTableDto();

        #endregion Weapon Type Data

        #region Self Shield Benefits
        public RollTableDto GetShootAndLootSelfShieldBenefits()
            => base.GetJsonByFileName(fileName: "ShootAndLootSelfShieldBenefits").ToShootAndLootShieldBenefitsRollTableDto();
        #endregion Self Shield Benefits

    }
}
