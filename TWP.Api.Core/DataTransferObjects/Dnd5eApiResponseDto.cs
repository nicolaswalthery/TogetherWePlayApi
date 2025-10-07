namespace TWP.Api.Core.DataTransferObjects
{
    public class Dnd5eApiResponseDto
    {
        public string Index { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class MonsterApiResponseDto
    {
        public int Count { get; set; }
        public List<Dnd5eApiResponseDto> Results { get; set; }
    }
}
