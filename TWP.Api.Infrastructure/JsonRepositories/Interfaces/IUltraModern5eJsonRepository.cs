using TWP.Api.Infrastructure.DataTransferObjects;

namespace TWP.Api.Infrastructure.JsonRepositories.Interfaces
{
    public interface IUltraModern5eJsonRepository
    {
        public RollTableDto GetTechItemTable_A_RandomTable();

        public RollTableDto GetShootAndLootCompanyName();

        public RollTableDto GetShootAndLootDamageType();

        public RollTableDto GetShootAndLootMagazine();

        public RollTableDto GetShootAndLootTechLevel();

        public RollTableDto GetShootAndLootLineData();

        public RollTableDto GetShootAndLootAdditionalProperty();

        public RollTableDto GetShootAndLootModelName();

        public RollTableDto GetShootAndLootModelBenefit();

        public RollTableDto GetShootAndLootShieldBenefits();


    }
}
