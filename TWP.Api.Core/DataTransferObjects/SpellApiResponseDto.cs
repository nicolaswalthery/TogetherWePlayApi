namespace TWP.Api.Core.DataTransferObjects
{
    public class SpellResultDto
    {
        public string Index { get; set; }
        public string Name { get; set; }
        public int Level { get; set; }
        public string Url { get; set; }
    }

    public class SpellApiResponseDto
    {
        public int Count { get; set; }
        public List<SpellResultDto> Results { get; set; }
    }
}
