using Common.Extensions;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.DataTransferObjects;
using TWP.Api.Infrastructure.Helpers;

namespace TWP.Api.Infrastructure.JsonRepositories.Mappers
{
    public static class ShootAndLootCompanyDataJsonFileMapper
    {
        public static RollTableDto ToShootAndLootCompanyNameRollTableDto(this string json)
        {
            var deserializedObject = json.JsonToObject<CompanyDataRoot>();

            var rollTableDto = new RollTableDto()
            {
                DiceType = DiceTypeEnum.d20,
                Genre = GenreEnum.ScienceFiction,
                IsTableCopywriteFree = false,
                MaxRerolls = 1,
                Name = "Shoot And Loot Company Name",
                NumberOfDiceType = 1,
                Subgenres = { SubgenreEnum.SpaceOpera, SubgenreEnum.Cyberpunk, SubgenreEnum.NasaPunk, SubgenreEnum.HardSciFi, SubgenreEnum.PostApo },
                Source = SourceFolderEnum.UM5e,
                Setting = SettingEnum.None,
                SentenceTemplate = string.Empty
            };

            foreach (var companyDatum in deserializedObject.CompanyData) // Matches the updated property
            {
                var (minRoll, maxRoll) = companyDatum.d20.ExtractMinAndMaxRollNumbers();
                rollTableDto.Entries.Add(new RollTableEntryDto()
                {
                    MinRoll = minRoll,
                    MaxRoll = maxRoll,
                    ResultText = companyDatum.Company,
                    Type = RollEntryEnum.Result
                });
            }

            return rollTableDto;
        }

        public static RollTableDto ToShootAndLootDamageTypeRollTableDto(this string json)
        {
            var deserializedObject = json.JsonToObject<CompanyDataRoot>();

            var rollTableDto = new RollTableDto()
            {
                DiceType = DiceTypeEnum.d20,
                Genre = GenreEnum.ScienceFiction,
                IsTableCopywriteFree = false,
                MaxRerolls = 1,
                Name = "Shoot And Loot Damage Type",
                NumberOfDiceType = 1,
                Subgenres = { SubgenreEnum.SpaceOpera, SubgenreEnum.Cyberpunk, SubgenreEnum.NasaPunk, SubgenreEnum.HardSciFi, SubgenreEnum.PostApo },
                Source = SourceFolderEnum.UM5e,
                Setting = SettingEnum.None,
                SentenceTemplate = string.Empty
            };

            foreach (var companyDatum in deserializedObject.CompanyData) // Matches the updated property
            {
                var (minRoll, maxRoll) = companyDatum.d20.ExtractMinAndMaxRollNumbers();
                rollTableDto.Entries.Add(new RollTableEntryDto()
                {
                    MinRoll = minRoll,
                    MaxRoll = maxRoll,
                    ResultText = companyDatum.DamageType,
                    Type = RollEntryEnum.Result
                });
            }

            return rollTableDto;
        }

        public static RollTableDto ToShootAndLootMagazineRollTableDto(this string json)
        {
            var deserializedObject = json.JsonToObject<CompanyDataRoot>();

            var rollTableDto = new RollTableDto()
            {
                DiceType = DiceTypeEnum.d20,
                Genre = GenreEnum.ScienceFiction,
                IsTableCopywriteFree = false,
                MaxRerolls = 1,
                Name = "Shoot And Loot Magazine",
                NumberOfDiceType = 1,
                Subgenres = { SubgenreEnum.SpaceOpera, SubgenreEnum.Cyberpunk, SubgenreEnum.NasaPunk, SubgenreEnum.HardSciFi, SubgenreEnum.PostApo },
                Source = SourceFolderEnum.UM5e,
                Setting = SettingEnum.None,
                SentenceTemplate = string.Empty
            };

            foreach (var companyDatum in deserializedObject.CompanyData) // Matches the updated property
            {
                var (minRoll, maxRoll) = companyDatum.d20.ExtractMinAndMaxRollNumbers();
                rollTableDto.Entries.Add(new RollTableEntryDto()
                {
                    MinRoll = minRoll,
                    MaxRoll = maxRoll,
                    ResultText = companyDatum.Magazine,
                    Type = RollEntryEnum.Result
                });
            }

            return rollTableDto;
        }

        public static RollTableDto ToShootAndLootTechLevelRollTableDto(this string json)
        {
            var deserializedObject = json.JsonToObject<CompanyDataRoot>();

            var rollTableDto = new RollTableDto()
            {
                DiceType = DiceTypeEnum.d20,
                Genre = GenreEnum.ScienceFiction,
                IsTableCopywriteFree = false,
                MaxRerolls = 1,
                Name = "Shoot And Loot Tech Level",
                NumberOfDiceType = 1,
                Subgenres = { SubgenreEnum.SpaceOpera, SubgenreEnum.Cyberpunk, SubgenreEnum.NasaPunk, SubgenreEnum.HardSciFi, SubgenreEnum.PostApo },
                Source = SourceFolderEnum.UM5e,
                Setting = SettingEnum.None,
                SentenceTemplate = string.Empty
            };

            foreach (var companyDatum in deserializedObject.CompanyData) // Matches the updated property
            {
                var (minRoll, maxRoll) = companyDatum.d20.ExtractMinAndMaxRollNumbers();
                rollTableDto.Entries.Add(new RollTableEntryDto()
                {
                    MinRoll = minRoll,
                    MaxRoll = maxRoll,
                    ResultText = companyDatum.TL,
                    Type = RollEntryEnum.Result
                });
            }

            return rollTableDto;
        }

        public class ShootAndLootCompanyDatumDto
        {
            public string d20 { get; set; }
            public string Company { get; set; }
            public string DamageType { get; set; }
            public string Magazine { get; set; }
            public string TL { get; set; }
        }

        internal class CompanyDataRoot
        {
            public List<ShootAndLootCompanyDatumDto> CompanyData { get; set; } // /!\ Must Matches the JSON key !
        }
    }
}
