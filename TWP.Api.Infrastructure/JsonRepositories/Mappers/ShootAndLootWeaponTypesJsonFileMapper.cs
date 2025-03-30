using Common.Extensions;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.DataTransferObjects;
using TWP.Api.Infrastructure.Helpers;

namespace TWP.Api.Infrastructure.JsonRepositories.Mappers
{
    public static class ShootAndLootWeaponTypesJsonFileMapper
    {
        public static RollTableDto ToShootAndLootWeaponWeightRollTableDto(this string json)
        {
            var deserializedObject = json.ToObject<WeaponTypesRoot>();
            var rollTableDto = BuildRollTableDto(name: "Weapon Weight");
            foreach (var weaponType in deserializedObject.WeaponTypes)
            {
                var (minRoll, maxRoll) = weaponType.d20.ExtractMinAndMaxRollNumbers();
                rollTableDto.Entries.Add(new RollTableEntryDto()
                {
                    MinRoll = minRoll,
                    MaxRoll = maxRoll,
                    ResultText = weaponType.Weight,
                    Type = RollEntryEnum.Result
                });
            }

            return rollTableDto;
        }

        public static RollTableDto ToShootAndLootWeaponPropertiesRollTableDto(this string json)
        {
            var deserializedObject = json.ToObject<WeaponTypesRoot>();
            var rollTableDto = BuildRollTableDto(name: "Weapon Property");
            foreach (var weaponType in deserializedObject.WeaponTypes)
            {
                var (minRoll, maxRoll) = weaponType.d20.ExtractMinAndMaxRollNumbers();
                rollTableDto.Entries.Add(new RollTableEntryDto()
                {
                    MinRoll = minRoll,
                    MaxRoll = maxRoll,
                    ResultText = weaponType.Properties,
                    Type = RollEntryEnum.Result
                });
            }

            return rollTableDto;
        }

        public static RollTableDto ToShootAndLootWeaponBaseCostRollTableDto(this string json)
        {
            var deserializedObject = json.ToObject<WeaponTypesRoot>();
            var rollTableDto = BuildRollTableDto(name: "Base Cost");
            foreach (var weaponType in deserializedObject.WeaponTypes)
            {
                var (minRoll, maxRoll) = weaponType.d20.ExtractMinAndMaxRollNumbers();
                rollTableDto.Entries.Add(new RollTableEntryDto()
                {
                    MinRoll = minRoll,
                    MaxRoll = maxRoll,
                    ResultText = weaponType.BaseCost,
                    Type = RollEntryEnum.Result
                });
            }

            return rollTableDto;
        }

        public static RollTableDto ToShootAndLootDamageRollTableDto(this string json)
        {
            var deserializedObject = json.ToObject<WeaponTypesRoot>();
            var rollTableDto = BuildRollTableDto(name: "Weapon Damage");
            foreach (var weaponType in deserializedObject.WeaponTypes)
            {
                var (minRoll, maxRoll) = weaponType.d20.ExtractMinAndMaxRollNumbers();
                rollTableDto.Entries.Add(new RollTableEntryDto()
                {
                    MinRoll = minRoll,
                    MaxRoll = maxRoll,
                    ResultText = weaponType.Damage,
                    Type = RollEntryEnum.Result
                });
            }

            return rollTableDto;
        }

        public static RollTableDto ToShootAndLootWeaponRangeRollTableDto(this string json)
        {
            var deserializedObject = json.ToObject<WeaponTypesRoot>();
            var rollTableDto = BuildRollTableDto(name: "Weapon Range");
            foreach (var weaponType in deserializedObject.WeaponTypes)
            {
                var (minRoll, maxRoll) = weaponType.d20.ExtractMinAndMaxRollNumbers();
                rollTableDto.Entries.Add(new RollTableEntryDto()
                {
                    MinRoll = minRoll,
                    MaxRoll = maxRoll,
                    ResultText = weaponType.Range,
                    Type = RollEntryEnum.Result
                });
            }

            return rollTableDto;
        }

        public static RollTableDto ToShootAndLootWeaponTypeRollTableDto(this string json)
        {
            var deserializedObject = json.ToObject<WeaponTypesRoot>();
            var rollTableDto = BuildRollTableDto(name: "Weapon Type");
            foreach (var somethingHappen in deserializedObject.WeaponTypes)
            {
                var (minRoll, maxRoll) = somethingHappen.d20.ExtractMinAndMaxRollNumbers();
                rollTableDto.Entries.Add(new RollTableEntryDto()
                {
                    MinRoll = minRoll,
                    MaxRoll = maxRoll,
                    ResultText = somethingHappen.WeaponType,
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
                Source = SourceFolderEnum.UM5e,
                Setting = SettingEnum.None,
                SentenceTemplate = string.Empty
            };
        }
    }

    public class WeaponTypeRoot
    {
        public string d20 { get; set; }
        public string WeaponType { get; set; }
        public string Damage { get; set; }
        public string BaseCost { get; set; }
        public string Range { get; set; }
        public string Weight { get; set; }
        public string Properties { get; set; }
    }

    internal class WeaponTypesRoot
    {
        public List<WeaponTypeRoot> WeaponTypes { get; set; } // /!\ Must Matches the JSON key !
    }
}
