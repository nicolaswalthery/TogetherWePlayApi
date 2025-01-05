using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Application.DataTransferObjects;
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

        public ShootAndLootDto ShootAndLootGeneration()
        {
            var shootAndLootDto = new ShootAndLootDto();

            //Weapon Type
            GetWeaponTypeData(shootAndLootDto);

            //Weapon Rarity
            GetWeaponRarityData(shootAndLootDto);

            //Company
            GetWeaponCompanyData(shootAndLootDto);

            //Model
            GetWeaponModelData(shootAndLootDto);

            //Line
            GetAndMapWeaponLineDataToDto(shootAndLootDto);

            return shootAndLootDto;
        }

        private void GetAndMapWeaponLineDataToDto(ShootAndLootDto shootAndLootDto)
        {
            var weaponLine = _ultraModern5EJsonRepository.GetShootAndLootLineData();
            var weaponLineAddProperties = _ultraModern5EJsonRepository.GetShootAndLootAdditionalProperty();

            var dieResult = RandomSelectionHelpers.RollDie(weaponLine.NumberOfDiceType, weaponLine.DiceType);

            shootAndLootDto.CompanyLineData = new()
            {
                AdditionalProperty = GetByDieResult(weaponLineAddProperties, dieResult).ResultText,
                lineName = GetByDieResult(weaponLine, dieResult).ResultText
            };
        }

        private void GetWeaponModelData(ShootAndLootDto ShootAndLootDto)
        {
            var weaponModelNames = _ultraModern5EJsonRepository.GetShootAndLootModelName();
            var weaponModelBenefits = _ultraModern5EJsonRepository.GetShootAndLootModelBenefit();

            var dieResult = RandomSelectionHelpers.RollDie(weaponModelNames.NumberOfDiceType, weaponModelNames.DiceType);

            var entries = new List<RollTableEntryDto>();
            entries.Add(GetByDieResult(weaponModelNames, dieResult));
            entries.Add(GetByDieResult(weaponModelBenefits, dieResult));
        }

        private void GetWeaponCompanyData(ShootAndLootDto ShootAndLootDto)
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
        }

        private void GetWeaponRarityData(ShootAndLootDto ShootAndLootDto)
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
        }

        private void GetWeaponTypeData(ShootAndLootDto ShootAndLootDto)
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
        }

        private static RollTableEntryDto GetByDieResult(RollTableDto rollTableDto, int rollResult)
            => rollTableDto.Entries.First(e => e.MinRoll <= rollResult && e.MaxRoll >= rollResult);
    }
}
