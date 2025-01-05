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
        private string _shootAndLootModelDataJsonFileName = "ShootAndLootModelData";
        private string _shootAndLootSelfShieldBenefitsJsonFileName = "ShootAndLootSelfShieldBenefits";
        private string _shootAndLootWeaponRaritiesJsonFileName = "ShootAndLootWeaponRarities";
        private string _shootAndLootWeaponTypeJsonFileName = "ShootAndLootWeaponTypes";

        public UltraModern5eJsonRepository() : base(folderName: SourceEnum.UM5e)
        {
        }

        #region Treasure Tables
        public RollTableDto GetTechItemTable_A_RandomTable() 
            => base.GetRollTable(fileName: "TechItemTableARandomTable").ToTechItemTableARandomTableRollTableDto();

        public RollTableDto GetTechItemTable_B_RandomTable()
            => base.GetRollTable(fileName: "TechItemTableBRandomTable").ToTechItemTableBRandomTableRollTableDto();

        public RollTableDto GetTechItemTable_C_RandomTable()
            => base.GetRollTable(fileName: "TechItemTableCRandomTable").ToTechItemTableCRandomTableRollTableDto();

        public RollTableDto GetTechItemTable_D_RandomTable()
            => base.GetRollTable(fileName: "TechItemTableDRandomTable").ToTechItemTableDRandomTableRollTableDto();

        public RollTableDto GetTechItemTable_E_RandomTable()
            => base.GetRollTable(fileName: "TechItemTableERandomTable").ToTechItemTableERandomTableRollTableDto();

        public RollTableDto GetTechItemTable_F_RandomTable()
            => base.GetRollTable(fileName: "TechItemTableFRandomTable").ToTechItemTableFRandomTableRollTableDto();

        public RollTableDto GetTechItemTable_G_RandomTable()
            => base.GetRollTable(fileName: "TechItemTableGRandomTable").ToTechItemTableGRandomTableRollTableDto();

        public RollTableDto GetTechItemTable_H_RandomTable()
            => base.GetRollTable(fileName: "TechItemTableHRandomTable").ToTechItemTableHRandomTableRollTableDto();

        public RollTableDto GetTechItemTable_I_RandomTable()
            => base.GetRollTable(fileName: "TechItemTableIRandomTable").ToTechItemTableIRandomTableRollTableDto();


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
            => base.GetRollTable(fileName: _shootAndLootModelDataJsonFileName).ToShootAndLootModelNameRollTableDto();

        public RollTableDto GetShootAndLootModelBenefit()
            => base.GetRollTable(fileName: _shootAndLootModelDataJsonFileName).ToShootAndLootModelBenefitsRollTableDto();

        #endregion Model Data

        #region Shield Benefits Data

        public RollTableDto GetShootAndLootShieldBenefits()
            => base.GetRollTable(fileName: _shootAndLootSelfShieldBenefitsJsonFileName).ToShootAndLootShieldBenefitsRollTableDto();

        #endregion Shield Benefits Data

        #region Weapon Rarities Data

        public RollTableDto GetShootAndLootWeaponRarities()
            => base.GetRollTable(fileName: _shootAndLootWeaponRaritiesJsonFileName).ToShootAndLootWeaponRarityRollTableDto();

        public RollTableDto GetShootAndLootWeaponCostMultipliers()
            => base.GetRollTable(fileName: _shootAndLootWeaponRaritiesJsonFileName).ToShootAndLootWeaponCostMultipliersRollTableDto();

        public RollTableDto GetShootAndLootWeaponNumberOfLines()
            => base.GetRollTable(fileName: _shootAndLootWeaponRaritiesJsonFileName).ToShootAndLootWeaponNumberOfLinesRollTableDto();

        public RollTableDto GetShootAndLootWeaponNumberOfModels()
            => base.GetRollTable(fileName: _shootAndLootWeaponRaritiesJsonFileName).ToShootAndLootWeaponNumberOfModelsRollTableDto();

        public RollTableDto GetShootAndLootWeaponBenefits()
            => base.GetRollTable(fileName: _shootAndLootWeaponRaritiesJsonFileName).ToShootAndLootWeaponBenefitsRollTableDto();

        #endregion Weapon Rarities Data

        #region Weapon Type Data

        public RollTableDto GetShootAndLootWeaponType()
            => base.GetRollTable(fileName: _shootAndLootWeaponTypeJsonFileName).ToShootAndLootWeaponTypeRollTableDto();

        public RollTableDto GetShootAndLootWeaponWeight()
            => base.GetRollTable(fileName: _shootAndLootWeaponTypeJsonFileName).ToShootAndLootWeaponWeightRollTableDto();

        public RollTableDto GetShootAndLootWeaponProperties()
            => base.GetRollTable(fileName: _shootAndLootWeaponTypeJsonFileName).ToShootAndLootWeaponPropertiesRollTableDto();

        public RollTableDto GetShootAndLootWeaponBaseCost()
            => base.GetRollTable(fileName: _shootAndLootWeaponTypeJsonFileName).ToShootAndLootWeaponBaseCostRollTableDto();

        public RollTableDto GetShootAndLootWeaponDamage()
            => base.GetRollTable(fileName: _shootAndLootWeaponTypeJsonFileName).ToShootAndLootDamageRollTableDto();

        public RollTableDto GetShootAndLootWeaponRange()
            => base.GetRollTable(fileName: _shootAndLootWeaponTypeJsonFileName).ToShootAndLootWeaponRangeRollTableDto();

        #endregion Weapon Type Data

    }
}
