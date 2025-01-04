using Common.Extensions;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.DataTransferObjects;
using TWP.Api.Infrastructure.Helpers;

namespace TWP.Api.Infrastructure.JsonRepositories.Mappers
{
    public static class ShootAndLootWeaponRaritiesJsonFileMapper
    {
        public static RollTableDto ToShootAndLootWeaponRarityRollTableDto(this string json)
        {
            var deserializedObject = json.ToObject<WeaponRaritiesRoot>();
            var rollTableDto = BuildRollTableDto(name: "Weapon Rarities");
            foreach (var somethingHappen in deserializedObject.WeaponRarities)
            {
                var (minRoll, maxRoll) = somethingHappen.d20.ExtractMinAndMaxRollNumbers();
                rollTableDto.Entries.Add(new RollTableEntryDto()
                {
                    MinRoll = minRoll,
                    MaxRoll = maxRoll,
                    ResultText = somethingHappen.Rarity,
                    Type = RollEntryEnum.Result
                });
            }

            return rollTableDto;
        }

        public static RollTableDto ToShootAndLootWeaponCostMultipliersRollTableDto(this string json)
        {
            var deserializedObject = json.ToObject<WeaponRaritiesRoot>();
            var rollTableDto = BuildRollTableDto(name: "Cost Multipliers");
            foreach (var somethingHappen in deserializedObject.WeaponRarities)
            {
                var (minRoll, maxRoll) = somethingHappen.d20.ExtractMinAndMaxRollNumbers();
                rollTableDto.Entries.Add(new RollTableEntryDto()
                {
                    MinRoll = minRoll,
                    MaxRoll = maxRoll,
                    ResultText = somethingHappen.CostMultiplier,
                    Type = RollEntryEnum.Result
                });
            }

            return rollTableDto;
        }

        public static RollTableDto ToShootAndLootWeaponNumberOfLinesRollTableDto(this string json)
        {
            var deserializedObject = json.ToObject<WeaponRaritiesRoot>();
            var rollTableDto = BuildRollTableDto(name: "Number of lines");
            foreach (var somethingHappen in deserializedObject.WeaponRarities)
            {
                var (minRoll, maxRoll) = somethingHappen.d20.ExtractMinAndMaxRollNumbers();
                rollTableDto.Entries.Add(new RollTableEntryDto()
                {
                    MinRoll = minRoll,
                    MaxRoll = maxRoll,
                    ResultText = somethingHappen.NumberOfLines,
                    Type = RollEntryEnum.Result
                });
            }

            return rollTableDto;
        }

        public static RollTableDto ToShootAndLootWeaponNumberOfModelsRollTableDto(this string json)
        {
            var deserializedObject = json.ToObject<WeaponRaritiesRoot>();
            var rollTableDto = BuildRollTableDto(name: "Number of models");
            foreach (var somethingHappen in deserializedObject.WeaponRarities)
            {
                var (minRoll, maxRoll) = somethingHappen.d20.ExtractMinAndMaxRollNumbers();
                rollTableDto.Entries.Add(new RollTableEntryDto()
                {
                    MinRoll = minRoll,
                    MaxRoll = maxRoll,
                    ResultText = somethingHappen.NumberOfModels,
                    Type = RollEntryEnum.Result
                });
            }

            return rollTableDto;
        }

        public static RollTableDto ToShootAndLootWeaponBenefitsRollTableDto(this string json)
        {
            var deserializedObject = json.ToObject<WeaponRaritiesRoot>();
            var rollTableDto = BuildRollTableDto(name: "Benefits");
            foreach (var somethingHappen in deserializedObject.WeaponRarities)
            {
                var (minRoll, maxRoll) = somethingHappen.d20.ExtractMinAndMaxRollNumbers();
                rollTableDto.Entries.Add(new RollTableEntryDto()
                {
                    MinRoll = minRoll,
                    MaxRoll = maxRoll,
                    ResultText = somethingHappen.Benefit,
                    Type = RollEntryEnum.Result
                });
            }

            return rollTableDto;
        }

        private static RollTableDto BuildRollTableDto(string name)
        {
            return new RollTableDto()
            {
                DiceType = DiceTypeEnum.d20,
                Genre = GenreEnum.ScienceFiction,
                IsTableCopywriteFree = false,
                MaxRerolls = 1,
                Name = name,
                NumberOfDiceType = 1,
                Subgenres = { SubgenreEnum.SpaceOpera, SubgenreEnum.Cyberpunk, SubgenreEnum.NasaPunk, SubgenreEnum.HardSciFi, SubgenreEnum.PostApo },
                Source = SourceEnum.UM5e,
                Setting = SettingEnum.None,
                SentenceTemplate = string.Empty
            };
        }
    }

    public class ShootAndLootWeaponRaritiesDatumDto
    {
        public string d20 { get; set; }
        public string Rarity { get; set; }
        public string Benefit { get; set; }
        public string NumberOfLines { get; set; }
        public string NumberOfModels { get; set; }
        public string CostMultiplier { get; set; }
    }

    internal class WeaponRaritiesRoot
    {
        public List<ShootAndLootWeaponRaritiesDatumDto> WeaponRarities { get; set; } // /!\ Must Matches the JSON key !
    }
}
