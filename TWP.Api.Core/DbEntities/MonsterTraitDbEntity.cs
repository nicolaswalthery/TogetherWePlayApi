namespace TWP.Api.Core.DbEntities
{
    public class MonsterTraitDbEntity : DbEntity
    {
        public Guid MonsterId { get; set; }
        public Guid TraitId { get; set; }

        // Navigation Properties
        public virtual Monster5eDbEntity Monster { get; set; } = null!;
        public virtual TraitDbEntity Trait { get; set; } = null!;
    }
}