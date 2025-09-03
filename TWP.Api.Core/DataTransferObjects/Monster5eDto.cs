namespace TWP.Api.Core.DataTransferObjects
{
    public class Monster5eDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Alignment { get; set; }
        public string ChallengeRating { get; set; } = string.Empty;
        public float Cr { get; set; }
        public float? CrInLair { get; set; }
        public int Xp { get; set; }
        public int InitiativeBonus { get; set; }
        public string? Role { get; set; }
        public string CreatureSize { get; set; } = string.Empty;

        // Defense
        public int ArmorClass { get; set; }
        public int? MinionArmorClass { get; set; }
        public int HitPoints { get; set; }
        public string HitDice { get; set; } = string.Empty;

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

        // Skills & Immunities
        public Dictionary<string, object>? Skills { get; set; }
        public string? DamageImmunities { get; set; }
        public string? DamageResistances { get; set; }
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

        // Equipment & Habitat
        public List<string>? Equipments { get; set; }
        public string? Habitats { get; set; }
        public string? CreatureType { get; set; }
        public string? CreatureSubType { get; set; }
        public string? MonsterGroup { get; set; }

        // Lore
        public string? Manner { get; set; }
        public string? Lore { get; set; }
        public int PageSource { get; set; }
        public string Source { get; set; } = string.Empty;

        // Related entities
        public List<ActionDto> Actions { get; set; } = new List<ActionDto>();
        public List<TraitDto> Traits { get; set; } = new List<TraitDto>();
        public Symbarum5eDto? Symbarum5e { get; set; }
    }

    public class ActionDto
    {
        public Guid Id { get; set; }
        public Guid MonsterId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // ActionTypeEnum as string
        public string AttackType { get; set; } = string.Empty; // AttackTypeEnum as string
        public string Description { get; set; } = string.Empty;
        public string? ShortRange { get; set; }
        public string? LongRange { get; set; }
        public int? AttackBonus { get; set; }
        public int? DamageBonus { get; set; }
        public string? DamageDice { get; set; } // DiceTypeEnum as string
        public int? NumberDamageDice { get; set; }
        public string? DamageType { get; set; } // DamageTypeEnum as string
        public int? LimitPerDay { get; set; }
        public bool IsProhibitedForMinion { get; set; }
        public string? ActionTrigger { get; set; }
        public string? AdvantageCondition { get; set; }
        public string? DisadvantageCondition { get; set; }
    }

    public class TraitDto
    {
        public Guid Id { get; set; }
        public Guid MonsterId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? AttackBonus { get; set; }
        public int? DamageBonus { get; set; }
        public string? DamageDice { get; set; } // DiceTypeEnum as string
        public int? NumberDamageDice { get; set; }
        public string? DamageType { get; set; } // DamageTypeEnum as string
        public string? TraitTrigger { get; set; }
        public string? AdvantageCondition { get; set; }
        public string? DisadvantageCondition { get; set; }
        public bool IsOptional { get; set; }
    }

    public class Symbarum5eDto
    {
        public Guid Id { get; set; }
        public Guid MonsterId { get; set; }
        public string Shadow { get; set; } = string.Empty;
    }
}