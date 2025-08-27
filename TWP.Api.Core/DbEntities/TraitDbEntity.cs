namespace TWP.Api.Core.DbEntities
{
    public class TraitDbEntity : DbEntity
    {
        public string Description { get; set; } = String.Empty;
        public string Title { get; set; } = String.Empty;

        // Navigation Property
        public virtual ICollection<MonsterTraitDbEntity> MonsterTraits { get; set; } = new List<MonsterTraitDbEntity>();
    }
}