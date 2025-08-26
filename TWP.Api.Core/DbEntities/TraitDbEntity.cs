namespace TWP.Api.Core.DbEntities
{
    public class TraitDbEntity : DbEntity
    {
        public string? Description { get; set; }
        public string? Title { get; set; }

        // Navigation Property
        public virtual ICollection<MonsterTraitDbEntity> MonsterTraits { get; set; } = new List<MonsterTraitDbEntity>();
    }
}