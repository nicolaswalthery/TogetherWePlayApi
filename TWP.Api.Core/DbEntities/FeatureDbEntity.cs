namespace TWP.Api.Core.DbEntities
{
    public class FeatureDbEntity : DbEntity
    {
        public Guid MonsterId { get; set; }
        public string Description { get; set; } = String.Empty;
        public string Title { get; set; } = String.Empty;
        public bool IsOptional { get; set; }

        // Navigation Property
        public virtual Monster5eDbEntity Monster { get; set; } = null!;
    }
}