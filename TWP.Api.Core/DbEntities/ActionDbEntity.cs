using TWP.Api.Core.Enums;

namespace TWP.Api.Core.DbEntities
{
    public class ActionDbEntity : DbEntity
    {
        public Guid MonsterId { get; set; }
        public string Name { get; set; } = string.Empty;
        public ActionTypeEnum Type { get; set; }
        public AttackTypeEnum AttackType { get; set; }
        public string Description { get; set; } = string.Empty;
        public int? AttackBonus { get; set; }
        public int? DamageBonus { get; set; }
        public DiceTypeEnum? DamageDice { get; set; }
        public int? NumberDamageDice { get; set; }
        public DamageTypeEnum? DamageType { get; set; }
        public int? LimitPerDay { get; set; }
        public bool IsProhibitedForMinion { get; set; }

        // Navigation Property
        public virtual Monster5eDbEntity Monster { get; set; } = null!;
    }
}