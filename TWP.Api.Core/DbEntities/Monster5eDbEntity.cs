using System.Text.Json;
using TWP.Api.Core.Enums;

namespace TWP.Api.Core.DbEntities
{
    public class Monster5eDbEntity : DbEntity
    {
        public string Name { get; set; } = string.Empty;
        public AlignmentEnum? Alignment { get; set; }
        public string ChallengeRating { get; set; } = String.Empty;
        public int Xp { get; set; }
        public int InitiativeBonus { get; set; }
        public CombatRoleEnum? Role { get; set; }

        // Defense
        public int ArmorClass { get; set; }
        public int? MinionArmorClass { get; set; }
        public int HitPoints { get; set; }

        // Movement
        public string? Speed { get; set; }
        public string? Climb { get; set; }
        public string? Swim { get; set; }
        public string? Fly { get; set; }

        // Abilities
        public int? Strength { get; set; }
        public int? Dexterity { get; set; }
        public int? Constitution { get; set; }
        public int? Intelligence { get; set; }
        public int? Wisdom { get; set; }
        public int? Charisma { get; set; }

        // Skills & Immunities (JSON stored as string)
        public string? Skills { get; set; } // JSON
        public string? DamageImmunities { get; set; }
        public string? Senses { get; set; }
        public string? Languages { get; set; }

        // Saving Throws
        public int? ConSavingThrow { get; set; }
        public int? DexSavingThrow { get; set; }
        public int? StrSavingThrow { get; set; }
        public int? WisSavingThrow { get; set; }
        public int? ChaSavingThrow { get; set; }
        public int? IntSavingThrow { get; set; }
        public int? ProficiencyBonus { get; set; }

        // Equipment & Habitat (JSON stored as string)
        public string? Equipments { get; set; } // JSON
        public string? Habitats { get; set; }
        public string? CreatureType { get; set; }
        public string? MonsterGroup { get; set; }

        // Lore
        public string? Manner { get; set; }
        public string? Lore { get; set; } // JSON
        public int PageSource { get; set; }
        public string Source { get; set; } = String.Empty;

        // Navigation Properties
        public virtual ICollection<ActionDbEntity> Actions { get; set; } = new List<ActionDbEntity>();
        public virtual ICollection<FeatureDbEntity> Features { get; set; } = new List<FeatureDbEntity>();
        public virtual ICollection<MonsterTraitDbEntity> MonsterTraits { get; set; } = new List<MonsterTraitDbEntity>();
        public virtual Symbarum5eDbEntity? Symbarum5e { get; set; }

        // Helper methods for JSON parsing
        public Dictionary<string, object>? GetSkills()
        {
            if (string.IsNullOrEmpty(Skills)) return null;
            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, object>>(Skills);
            }
            catch
            {
                return null;
            }
        }

        public void SetSkills(Dictionary<string, object> skills)
        {
            Skills = JsonSerializer.Serialize(skills);
        }

        public List<string>? GetEquipments()
        {
            if (string.IsNullOrEmpty(Equipments)) return null;
            try
            {
                return JsonSerializer.Deserialize<List<string>>(Equipments);
            }
            catch
            {
                return null;
            }
        }

        public void SetEquipments(List<string> equipments)
        {
            Equipments = JsonSerializer.Serialize(equipments);
        }
    }
}