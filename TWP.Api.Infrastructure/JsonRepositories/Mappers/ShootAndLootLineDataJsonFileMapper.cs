using Common.Extensions;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.DataTransferObjects;
using TWP.Api.Infrastructure.Helpers;

namespace TWP.Api.Infrastructure.JsonRepositories.Mappers
{
    public static class ShootAndLootLineDataJsonFileMapper
    {
        public static RollTableDto ToShootAndLootLineRollTableDto(this string json)
        {
            var deserializedObject = json.ToObject<Root>();

            var rollTableDto = new RollTableDto()
            {
                DiceType = DiceTypeEnum.d20,
                Genre = GenreEnum.ScienceFiction,
                IsTableCopywriteFree = false,
                MaxRerolls = 1,
                Name = "Shoot And Loot Line Name",
                NumberOfDiceType = 1,
                Subgenres = { SubgenreEnum.SpaceOpera, SubgenreEnum.Cyberpunk, SubgenreEnum.NasaPunk, SubgenreEnum.HardSciFi, SubgenreEnum.PostApo },
                Source = SourceEnum.UM5e,
                Setting = SettingEnum.None,
                SentenceTemplate = string.Empty
            };

            foreach (var lineDatum in deserializedObject.LineData) // Matches the updated property
            {
                var (minRoll, maxRoll) = lineDatum.d20.ExtractMinAndMaxRollNumbers();
                rollTableDto.Entries.Add(new RollTableEntryDto()
                {
                    MinRoll = minRoll,
                    MaxRoll = maxRoll,
                    ResultText = lineDatum.Line,
                    Type = RollEntryEnum.Result
                });
            }

            return rollTableDto;
        }

        public static RollTableDto ToShootAndLootAdditionalPropertyRollTableDto(this string json)
        {
            var deserializedObject = json.ToObject<Root>();

            var rollTableDto = new RollTableDto()
            {
                DiceType = DiceTypeEnum.d20,
                Genre = GenreEnum.ScienceFiction,
                IsTableCopywriteFree = false,
                MaxRerolls = 1,
                Name = "Shoot And Loot Line Additional Property",
                NumberOfDiceType = 1,
                Subgenres = { SubgenreEnum.SpaceOpera, SubgenreEnum.Cyberpunk, SubgenreEnum.NasaPunk, SubgenreEnum.HardSciFi, SubgenreEnum.PostApo },
                Source = SourceEnum.UM5e,
                Setting = SettingEnum.None,
                SentenceTemplate = string.Empty
            };

            foreach (var lineDatum in deserializedObject.LineData) // Matches the updated property
            {
                var (minRoll, maxRoll) = lineDatum.d20.ExtractMinAndMaxRollNumbers();
                rollTableDto.Entries.Add(new RollTableEntryDto()
                {
                    MinRoll = minRoll,
                    MaxRoll = maxRoll,
                    ResultText = lineDatum.AdditionalProperty,
                    Type = RollEntryEnum.Result
                });
            }

            return rollTableDto;
        }

        public class ShootAndLootLineDatumDto
        {
            public string d20 { get; set; }
            public string Line { get; set; }
            public string AdditionalProperty { get; set; }
        }

        internal class Root
        {
            public List<ShootAndLootLineDatumDto> LineData { get; set; } // /!\ Must Matches the JSON key !
        }
    }
}
