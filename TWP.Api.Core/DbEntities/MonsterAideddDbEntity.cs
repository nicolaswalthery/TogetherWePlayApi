namespace TWP.Api.Core.DbEntities
{
    public class MonsterAideddDbEntity : DbEntity
    {
        public string Name { get; set; } = string.Empty;
        public string CR { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public int AC { get; set; }
        public int HP { get; set; }
        public string Speed { get; set; } = string.Empty;
        public string Alignment { get; set; } = string.Empty;
        public string Legendary { get; set; } = string.Empty;
        public string Habitat { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
    }
}
