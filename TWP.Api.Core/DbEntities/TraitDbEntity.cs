using TWP.Api.Core.Enums;

namespace TWP.Api.Core.DbEntities
{
    public class TraitDbEntity : DbEntity
    {
        public Guid MonsterId { get; set; }
        public string Description { get; set; } = String.Empty;
        public string Title { get; set; } = String.Empty;

        public int? AttackBonus { get; set; }
        public int? DamageBonus { get; set; }
        public DiceTypeEnum? DamageDice { get; set; }
        public int? NumberDamageDice { get; set; }
        public DamageTypeEnum? DamageType { get; set; }
        public string? traitTrigger { get; set; } = String.Empty;
        public string? advantageCondition { get; set; } = String.Empty;
        public string? disadvantageCondition { get; set; } = String.Empty;

        public bool IsOptional { get; set; }

        // Navigation Property
        public virtual Monster5eDbEntity Monster { get; set; } = null!;
    }
}