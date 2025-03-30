using Common.Extensions;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.DataTransferObjects;
using TWP.Api.Infrastructure.Helpers;

namespace TWP.Api.Infrastructure.JsonRepositories.Mappers
{
    public static class SomethingHappenJsonFileMapper
    {
        public static RollTableDto ToSomethingHappensRollTableDto(this string json)
        {
            var deserializedObject = json.ToObject<SomethingHappensRoot>();

            var rollTableDto = new RollTableDto()
            {
                DiceType = DiceTypeEnum.d100,
                Genre = GenreEnum.Fantasy,
                IsTableCopywriteFree = false,
                MaxRerolls = 1,
                Name = "Something Happen",
                NumberOfDiceType = 1,
                Subgenres = { SubgenreEnum.DarkFantasy },
                Source = SourceFolderEnum.Shadowdark,
                Setting = SettingEnum.None,
                SentenceTemplate = string.Empty
            };

            foreach (var somethingHappen in deserializedObject.SomethingHappens)
            {
                var (minRoll, maxRoll) = somethingHappen.d100.ExtractMinAndMaxRollNumbers();
                rollTableDto.Entries.Add(new RollTableEntryDto()
                {
                    MinRoll = minRoll,
                    MaxRoll = maxRoll,
                    ResultText = somethingHappen.Details,
                    Type = Core.Enums.RollEntryEnum.Result
                });
            }

            return rollTableDto;
        }
    }

    internal class SomethingHappensEntry
    {
        public string d100 { get; set; }
        public string Details { get; set; }
    }

    internal class SomethingHappensRoot
    {
        public List<SomethingHappensEntry> SomethingHappens { get; set; }
    }
}
