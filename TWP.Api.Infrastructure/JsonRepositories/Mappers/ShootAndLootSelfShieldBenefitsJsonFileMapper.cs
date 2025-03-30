using Common.Extensions;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.DataTransferObjects;
using TWP.Api.Infrastructure.Helpers;

namespace TWP.Api.Infrastructure.JsonRepositories.Mappers
{
    public static class ShootAndLootSelfShieldBenefitsJsonFileMapper
    {
        public static RollTableDto ToShootAndLootShieldBenefitsRollTableDto(this string json)
        {
            var deserializedObject = json.ToObject<ShootAndLootShieldBenefitRoot>();
            RollTableDto rollTableDto = BuildRollTableDto(name: "Shoot And Loot Self Shield Benefits");

            foreach (var SelfShieldBenefit in deserializedObject.SelfShieldBenefits)
            {
                var (minRoll, maxRoll) = SelfShieldBenefit.d100.ExtractMinAndMaxRollNumbers();
                rollTableDto.Entries.Add(new RollTableEntryDto()
                {
                    MinRoll = minRoll,
                    MaxRoll = maxRoll,
                    ResultText = SelfShieldBenefit.Model,
                    Type = RollEntryEnum.Result
                });
            }

            return rollTableDto;
        }

        private static RollTableDto BuildRollTableDto(string name)
        {
            return new RollTableDto()
            {
                DiceType = DiceTypeEnum.d100,
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

        public class ShootAndLootShieldBenefitDatumDto
        {
            public string d100 { get; set; }
            public string Model { get; set; }
            public string Benefit { get; set; }
        }

        internal class ShootAndLootShieldBenefitRoot
        {
            public List<ShootAndLootShieldBenefitDatumDto> SelfShieldBenefits { get; set; } // /!\ Must Matches the JSON key !
        }
    }
}
