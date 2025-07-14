using Common.Extensions;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.DataTransferObjects;
using TWP.Api.Infrastructure.Helpers;

namespace TWP.Api.Infrastructure.JsonRepositories.Mappers
{
    public static class MonsterActivitiesJsonFileMapper
    {
        public static RollTableDto ToMonsterActivitiesRollTableDto(this string json)
        {
            var deserializedObject = json.JsonToObject<Root>();

            var rollTableDto = new RollTableDto()
            {
                DiceType = DiceTypeEnum.d6,
                Genre = GenreEnum.Fantasy,
                IsTableCopywriteFree = false,
                MaxRerolls = 1,
                Name = "Monster Activity",
                NumberOfDiceType = 2,
                Subgenres = { SubgenreEnum.DarkFantasy },
                Source = SourceFolderEnum.Shadowdark,
                Setting = SettingEnum.None,
                SentenceTemplate = string.Empty
            };

            foreach (var activity in deserializedObject.ActivityTable) // Matches the updated property
            {
                var (minRoll, maxRoll) = activity.Roll.ExtractMinAndMaxRollNumbers();
                rollTableDto.Entries.Add(new RollTableEntryDto()
                {
                    MinRoll = minRoll,
                    MaxRoll = maxRoll,
                    ResultText = activity.Activity,
                    Type = Core.Enums.RollEntryEnum.Result
                });
            }

            return rollTableDto;
        }
    }

    internal class MonsterActivity
    {
        public string Roll { get; set; }
        public string Activity { get; set; }
    }

    internal class Root
    {
        public List<MonsterActivity> ActivityTable { get; set; } // /!\ Must Matches the JSON key !
    }
}
