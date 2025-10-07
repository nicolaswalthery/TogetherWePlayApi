namespace TWP.Api.Core.DbEntities
{
    public class Symbarum5eDbEntity : DbEntity
    {
        public Guid MonsterId { get; set; }
        public string Shadow { get; set; } = String.Empty;

        // Navigation Property
        public virtual Monster5eDbEntity Monster { get; set; } = null!;
    }
}