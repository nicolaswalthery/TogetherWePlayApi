using System.Text.Json.Serialization;

namespace TWP.Api.Application.DataTransferObjects
{
    /// <summary>
    /// DTO for sci-fi adversary analysis
    /// </summary>
    public class ScifiAdversaryAnalysisDto
    {
        [JsonPropertyName("suggestedCR")]
        public float SuggestedCR { get; set; }

        [JsonPropertyName("threatLevel")]
        public string ThreatLevel { get; set; }

        [JsonPropertyName("suggestedRoles")]
        public List<string> SuggestedRoles { get; set; }

        [JsonPropertyName("keyTraits")]
        public List<string> KeyTraits { get; set; }

        [JsonPropertyName("weaponType")]
        public string WeaponType { get; set; }

        [JsonPropertyName("techLevel")]
        public int TechLevel { get; set; }
    }
}
