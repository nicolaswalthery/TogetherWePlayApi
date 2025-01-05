using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Application.Helpers;
using TWP.Api.Infrastructure.DataTransferObjects;
using TWP.Api.Infrastructure.JsonRepositories.Interfaces;

namespace TWP.Api.Application.BusinessLayers
{
    public class UltraModern5eBusinessLayer : IUltraModern5eBusinessLayer
    {

        private IUltraModern5eJsonRepository _ultraModern5EJsonRepository;

        public UltraModern5eBusinessLayer(IUltraModern5eJsonRepository ultraModern5EJsonRepository)
        {
            _ultraModern5EJsonRepository = ultraModern5EJsonRepository;
        }

        public List<RollTableEntryDto> ShootAndLootGeneration()
        {
            //Weapon Type
            var weaponTypeData = GetWeaponTypeData();

            //Weapon Rarity
            var weaponRarityData = GetWeaponRarityData();

            //Company
            var weaponCompanyData = GetWeaponCompanyData();

            //Model
            var weaponModelData = GetWeaponModelData();

            //Line
            var weaponLineData = GetWeaponLineData();




        }

        private List<RollTableEntryDto> GetWeaponLineData()
        {
            var weaponLine = _ultraModern5EJsonRepository.GetShootAndLootLineData();
            var weaponLineAddProperties = _ultraModern5EJsonRepository.GetShootAndLootAdditionalProperty();

            var dieResult = RandomSelectionHelpers.RollDie(weaponLine.NumberOfDiceType, weaponLine.DiceType);

            var entries = new List<RollTableEntryDto>();
            entries.Add(GetByDieResult(weaponLine, dieResult));
            entries.Add(GetByDieResult(weaponLineAddProperties, dieResult));

            return entries;
        }

        private List<RollTableEntryDto> GetWeaponModelData()
        {
            var weaponModelNames = _ultraModern5EJsonRepository.GetShootAndLootModelName();
            var weaponModelBenefits = _ultraModern5EJsonRepository.GetShootAndLootModelBenefit();

            var dieResult = RandomSelectionHelpers.RollDie(weaponModelNames.NumberOfDiceType, weaponModelNames.DiceType);

            var entries = new List<RollTableEntryDto>();
            entries.Add(GetByDieResult(weaponModelNames, dieResult));
            entries.Add(GetByDieResult(weaponModelBenefits, dieResult));

            return entries;
        }

        private List<RollTableEntryDto> GetWeaponCompanyData()
        {
            var weaponCompanyNames = _ultraModern5EJsonRepository.GetShootAndLootCompanyName();
            var weaponDamageTypes = _ultraModern5EJsonRepository.GetShootAndLootDamageType();
            var weaponMagazine = _ultraModern5EJsonRepository.GetShootAndLootMagazine();
            var weaponTechLevel = _ultraModern5EJsonRepository.GetShootAndLootTechLevel();

            var dieResult = RandomSelectionHelpers.RollDie(weaponCompanyNames.NumberOfDiceType, weaponCompanyNames.DiceType);

            var entries = new List<RollTableEntryDto>();
            entries.Add(GetByDieResult(weaponCompanyNames, dieResult));
            entries.Add(GetByDieResult(weaponDamageTypes, dieResult));
            entries.Add(GetByDieResult(weaponMagazine, dieResult));
            entries.Add(GetByDieResult(weaponTechLevel, dieResult));

            return entries;
        }

        private List<RollTableEntryDto> GetWeaponRarityData()
        {
            var weaponRarities = _ultraModern5EJsonRepository.GetShootAndLootWeaponRarities();
            var weaponCostMultipliers = _ultraModern5EJsonRepository.GetShootAndLootWeaponCostMultipliers();
            var weaponNumberOfLines = _ultraModern5EJsonRepository.GetShootAndLootWeaponNumberOfLines();
            var weaponNumberOfModels = _ultraModern5EJsonRepository.GetShootAndLootWeaponNumberOfModels();
            var weaponBenefits = _ultraModern5EJsonRepository.GetShootAndLootWeaponBenefits();

            var dieResult = RandomSelectionHelpers.RollDie(weaponRarities.NumberOfDiceType, weaponRarities.DiceType);

            var entries = new List<RollTableEntryDto>();
            entries.Add(GetByDieResult(weaponRarities, dieResult));
            entries.Add(GetByDieResult(weaponCostMultipliers, dieResult));
            entries.Add(GetByDieResult(weaponNumberOfLines, dieResult));
            entries.Add(GetByDieResult(weaponNumberOfModels, dieResult));
            entries.Add(GetByDieResult(weaponBenefits, dieResult));

            return entries;
        }

        private List<RollTableEntryDto> GetWeaponTypeData()
        {
            var weaponType = _ultraModern5EJsonRepository.GetShootAndLootWeaponType();
            var weaponWeight = _ultraModern5EJsonRepository.GetShootAndLootWeaponWeight();
            var weaponProperties = _ultraModern5EJsonRepository.GetShootAndLootWeaponProperties();
            var weaponBaseCost = _ultraModern5EJsonRepository.GetShootAndLootWeaponBaseCost();
            var weaponDamage = _ultraModern5EJsonRepository.GetShootAndLootWeaponDamage();
            var weaponRange = _ultraModern5EJsonRepository.GetShootAndLootWeaponRange();

            var dieResult = RandomSelectionHelpers.RollDie(weaponType.NumberOfDiceType, weaponType.DiceType);

            var entries = new List<RollTableEntryDto>();
            entries.Add(GetByDieResult(weaponType, dieResult));
            entries.Add(GetByDieResult(weaponWeight, dieResult));
            entries.Add(GetByDieResult(weaponProperties, dieResult));
            entries.Add(GetByDieResult(weaponBaseCost, dieResult));
            entries.Add(GetByDieResult(weaponDamage, dieResult));
            entries.Add(GetByDieResult(weaponRange, dieResult));

            return entries;
        }

        private static RollTableEntryDto GetByDieResult(RollTableDto rollTableDto, int rollResult)
            => rollTableDto.Entries.First(e => e.MinRoll <= rollResult && e.MaxRoll >= rollResult);
    }
}
