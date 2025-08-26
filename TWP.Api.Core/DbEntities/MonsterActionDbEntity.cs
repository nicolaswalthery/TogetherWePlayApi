namespace TWP.Api.Core.DbEntities
{
    public class MonsterActionDbEntity : DbEntity
    {
        public Guid MonsterId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? AttackBonus { get; set; }
        public string? Damage { get; set; }
        public string? DamageType { get; set; }
        public int? LimitPerDay { get; set; }
        public bool IsMovement { get; set; }
        public bool IsAction { get; set; }
        public bool IsBonus { get; set; }
        public bool IsReaction { get; set; }
        public bool IsLegendary { get; set; }

        // Navigation Property
        public virtual Monster5eDbEntity Monster { get; set; } = null!;
    }
}