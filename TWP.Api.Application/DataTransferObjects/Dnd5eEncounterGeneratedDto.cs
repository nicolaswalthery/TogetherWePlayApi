using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Core.Enums;

namespace TWP.Api.Application.DataTransferObjects
{
    public class Dnd5eEncounterGeneratedDto
    {
        public List<Dnd5eMonsterDto> Monsters { get; set; }
        public EncounterDifficultyEnum EncounterDifficulty { get; set; }
        public int Cr { get; set; }
        public string EncounterNarrativeContext { get; set; }
        public IList<MonsterHabitatEnum> MonsterHabitats { get; set; }
        public string OpenAiResponse { get; set; }
        public string FormattedEncounterData { get; set; }
    }
}
