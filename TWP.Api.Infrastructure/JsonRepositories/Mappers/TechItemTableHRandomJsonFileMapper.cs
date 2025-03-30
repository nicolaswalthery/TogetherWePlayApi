using Common.Extensions;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.DataTransferObjects;
using TWP.Api.Infrastructure.Helpers;

namespace TWP.Api.Infrastructure.JsonRepositories.Mappers
{
    public static class TechItemTableHRandomJsonFileMapper
    {
        public static RollTableDto ToTechItemTableHRandomTableRollTableDto(this string json)
        {
            var deserializedObject = json.ToObject<TechItemTableHRoot>();

            var rollTableDto = new RollTableDto()
            {
                DiceType = DiceTypeEnum.d100,
                Genre = GenreEnum.ScienceFiction,
                IsTableCopywriteFree = false,
                MaxRerolls = 1,
                Name = "Tech Item Table H",
                NumberOfDiceType = 1,
                Subgenres = { SubgenreEnum.SpaceOpera, SubgenreEnum.Cyberpunk, SubgenreEnum.NasaPunk, SubgenreEnum.HardSciFi, SubgenreEnum.PostApo },
                Source = SourceFolderEnum.UM5e,
                Setting = SettingEnum.None,
                SentenceTemplate = string.Empty
            };

            foreach (var techItem in deserializedObject.TechItemTableH)
            {
                var (minRoll, maxRoll) = techItem.Range.ExtractMinAndMaxRollNumbers();
                rollTableDto.Entries.Add(new RollTableEntryDto()
                {
                    MinRoll = minRoll,
                    MaxRoll = maxRoll,
                    ResultText = techItem.Tech,
                    Type = Core.Enums.RollEntryEnum.Result
                });
            }

            return rollTableDto;
        }
    }

    internal class TechItemTableHEntry
    {
        public string Range { get; set; }
        public string Tech { get; set; }
    }

    internal class TechItemTableHRoot
    {
        public List<TechItemTableHEntry> TechItemTableH { get; set; }
    }
}
