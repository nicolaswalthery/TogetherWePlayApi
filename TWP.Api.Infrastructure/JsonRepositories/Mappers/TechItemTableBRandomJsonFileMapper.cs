using Common.Extensions;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.DataTransferObjects;
using TWP.Api.Infrastructure.Helpers;

namespace TWP.Api.Infrastructure.JsonRepositories.Mappers
{
    public static class TechItemTableBRandomJsonFileMapper
    {
        public static RollTableDto ToTechItemTableBRandomTableRollTableDto(this string json)
        {
            var deserializedObject = json.ToObject<TechItemTableARoot>();

            var rollTableDto = new RollTableDto()
            {
                DiceType = DiceTypeEnum.d100,
                Genre = GenreEnum.ScienceFiction,
                IsTableCopywriteFree = false,
                MaxRerolls = 1,
                Name = "Tech Item Table B",
                NumberOfDiceType = 1,
                Subgenres = { SubgenreEnum.SpaceOpera, SubgenreEnum.Cyberpunk, SubgenreEnum.NasaPunk, SubgenreEnum.HardSciFi, SubgenreEnum.PostApo },
                Source = SourceEnum.UM5e,
                Setting = SettingEnum.None,
                SentenceTemplate = string.Empty
            };

            foreach (var techItem in deserializedObject.TechItemTableA)
            {
                var (minRoll, maxRoll) = techItem.Range.ExtractMinAndMaxRollNumbers();
                rollTableDto.Entries.Add(new RollTableEntryDto()
                {
                    MinRoll = minRoll,
                    MaxRoll = maxRoll,
                    ResultText = techItem.Description,
                    Type = Core.Enums.RollEntryEnum.Result
                });
            }

            return rollTableDto;
        }
    }

    internal class TechItemTableBEntry
    {
        public string Range { get; set; }
        public string Tech { get; set; }
    }

    internal class TechItemTableBRoot
    {
        public List<TechItemTableAEntry> TechItemTableB { get; set; }
    }
}
