using Common.Extensions;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.DataTransferObjects;
using TWP.Api.Infrastructure.Helpers;

namespace TWP.Api.Infrastructure.JsonRepositories.Mappers
{
    public static class ShootAndLootModelDataJsonFileMapper
    {
        public static RollTableDto ToShootAndLootModelNameRollTableDto(this string json)
        {
            var deserializedObject = json.ToObject<Root>();

            var rollTableDto = new RollTableDto()
            {
                DiceType = DiceTypeEnum.d20,
                Genre = GenreEnum.ScienceFiction,
                IsTableCopywriteFree = false,
                MaxRerolls = 1,
                Name = "Shoot And Loot Model Name",
                NumberOfDiceType = 1,
                Subgenres = { SubgenreEnum.SpaceOpera, SubgenreEnum.Cyberpunk, SubgenreEnum.NasaPunk, SubgenreEnum.HardSciFi, SubgenreEnum.PostApo },
                Source = SourceEnum.UM5e,
                Setting = SettingEnum.None,
                SentenceTemplate = string.Empty
            };

            foreach (var modelData in deserializedObject.ShootAndLootModelData) // Matches the updated property
            {
                var (minRoll, maxRoll) = modelData.d20.ExtractMinAndMaxRollNumbers();
                rollTableDto.Entries.Add(new RollTableEntryDto()
                {
                    MinRoll = minRoll,
                    MaxRoll = maxRoll,
                    ResultText = modelData.Model,
                    Type = RollEntryEnum.Result
                });
            }

            return rollTableDto;
        }

        public static RollTableDto ToShootAndLootModelBenefitsRollTableDto(this string json)
        {
            var deserializedObject = json.ToObject<Root>();

            var rollTableDto = new RollTableDto()
            {
                DiceType = DiceTypeEnum.d20,
                Genre = GenreEnum.ScienceFiction,
                IsTableCopywriteFree = false,
                MaxRerolls = 1,
                Name = "Shoot And Loot Model Benefit",
                NumberOfDiceType = 1,
                Subgenres = { SubgenreEnum.SpaceOpera, SubgenreEnum.Cyberpunk, SubgenreEnum.NasaPunk, SubgenreEnum.HardSciFi, SubgenreEnum.PostApo },
                Source = SourceEnum.UM5e,
                Setting = SettingEnum.None,
                SentenceTemplate = string.Empty
            };

            foreach (var modelDatum in deserializedObject.ShootAndLootModelData) // Matches the updated property
            {
                var (minRoll, maxRoll) = modelDatum.d20.ExtractMinAndMaxRollNumbers();
                rollTableDto.Entries.Add(new RollTableEntryDto()
                {
                    MinRoll = minRoll,
                    MaxRoll = maxRoll,
                    ResultText = modelDatum.Benefit,
                    Type = RollEntryEnum.Result
                });
            }

            return rollTableDto;
        }

        public class ShootAndLootModelDatumDto
        {
            public string d20 { get; set; }
            public string Model { get; set; }
            public string Benefit { get; set; }
        }

        internal class Root
        {
            public List<ShootAndLootModelDatumDto> ShootAndLootModelData { get; set; } // /!\ Must Matches the JSON key !
        }
    }
}
