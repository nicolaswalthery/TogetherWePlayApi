using Common.Extensions;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.DataTransferObjects;

namespace TWP.Api.Infrastructure.JsonRepositories.Mappers
{
    public static class RandomDungeonCorridorsJsonFileMapper
    {
        public static RollTableDto ToRandomDungeonCorridorsTableRollTableDto(this string json)
        {
            var deserializedObject = json.ToObject<RandomDungeonCorridorsRoot>();

            var rollTableDto = new RollTableDto()
            {
                DiceType = DiceTypeEnum.d20,
                Genre = GenreEnum.Fantasy,
                IsTableCopywriteFree = false,
                MaxRerolls = 1,
                Name = "Random Dungeon Corridors Table",
                NumberOfDiceType = 1,
                Subgenres = { SubgenreEnum.HeroicFantasy },
                Source = SourceFolderEnum.Dnd4e,
                Setting = SettingEnum.None,
                SentenceTemplate = string.Empty
            };

            foreach (var corridor in deserializedObject.Corridors)
            {
                var minRoll = corridor.d20;
                var maxRoll = minRoll; // In this case, each corridor has a single roll number
                rollTableDto.Entries.Add(new RollTableEntryDto()
                {
                    MinRoll = minRoll,
                    MaxRoll = maxRoll,
                    ResultText = corridor.Corridor,
                    Type = RollEntryEnum.Result
                });
            }

            return rollTableDto;
        }
    }

    internal class RandomDungeonCorridorEntry
    {
        public int d20 { get; set; }
        public string Corridor { get; set; }
    }

    internal class RandomDungeonCorridorsRoot
    {
        public List<RandomDungeonCorridorEntry> Corridors { get; set; }
    }
}
