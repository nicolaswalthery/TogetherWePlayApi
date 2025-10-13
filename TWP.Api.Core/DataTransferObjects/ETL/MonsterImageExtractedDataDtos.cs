namespace TWP.Api.Core.DataTransferObjects.ETL;

public class MonsterImageExtractedDataDtos
{
    // DTOs for JSON parsing
    public class MonsterImageData
    {
        public string? Name { get; set; }
        public string? Alignment { get; set; }
        public string? Size { get; set; }
        public string? Type { get; set; }
        public string? Subtype { get; set; }
        public int? ArmorClass { get; set; }
        public int? MinionArmorClass { get; set; }
        public int? HitPoints { get; set; }
        public int? MinionHitPoints { get; set; }
        public string? HitDice { get; set; }
        public string? Speed { get; set; }
        public string? Climb { get; set; }
        public string? Swim { get; set; }
        public string? Fly { get; set; }
        public int? Strength { get; set; }
        public int? Dexterity { get; set; }
        public int? Constitution { get; set; }
        public int? Intelligence { get; set; }
        public int? Wisdom { get; set; }
        public int? Charisma { get; set; }
        public string? ChallengeRating { get; set; }
        public int? ProficiencyBonus { get; set; }
        public SavingThrowsData? SavingThrows { get; set; }
        public Dictionary<string, int>? Skills { get; set; }
        public string? DamageResistances { get; set; }
        public string? DamageImmunities { get; set; }
        public string? ConditionImmunities { get; set; }
        public string? Senses { get; set; }
        public string? Languages { get; set; }
        public List<TraitImageData>? Traits { get; set; }
        public List<ActionImageData>? Actions { get; set; }
        public List<ReactionImageData>? Reactions { get; set; }
        public List<LegendaryActionImageData>? LegendaryActions { get; set; }
        public List<LairActionImageData>? LairActions { get; set; }
    }

    public class SavingThrowsData
    {
        public int? Str { get; set; }
        public int? Dex { get; set; }
        public int? Con { get; set; }
        public int? Int { get; set; }
        public int? Wis { get; set; }
        public int? Cha { get; set; }
    }

    public class TraitImageData
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }

    public class ActionImageData
    {
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? AttackType { get; set; }
        public string? Description { get; set; }
        public int? AttackBonus { get; set; }
        public string? Reach { get; set; }
        public string? Range { get; set; }
        public string? Damage { get; set; }
        public string? DamageType { get; set; }
    }

    public class ReactionImageData
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Trigger { get; set; }
    }

    public class LegendaryActionImageData
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Cost { get; set; }
    }

    public class LairActionImageData
    {
        public string? Description { get; set; }
    }
}