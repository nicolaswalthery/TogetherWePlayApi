using System.Text.Json;
using TWP.Api.Application.Interfaces.Services;
using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Core.DbEntities;
using TWP.Api.Core.Enums;

namespace TWP.Api.Application.ETL.Services
{
    public class TraitMapperService : ITraitMapperService
    {
        private readonly IOpenAiServices _openAiInterops;

        public TraitMapperService(IOpenAiServices openAiInterops)
        {
            _openAiInterops = openAiInterops;
        }

        public async Task<List<TraitDbEntity>> MapMonsterTraitsAsync(
            AideDdMonsterResponseDto monsterDto,
            Guid monsterId)
        {
            if (monsterDto.Traits == null || !monsterDto.Traits.Any())
            {
                return new List<TraitDbEntity>();
            }

            var systemPrompt = GetSystemPrompt();
            var userMessage = BuildBatchMessage(monsterDto.Traits, monsterDto.Name);

            try
            {
                // Use a lower temperature for more consistent parsing
                var response = await _openAiInterops.GetChatGptResponseAsync(
                    userMessage,
                    temperature: 0.1,  // Low temperature for consistency
                    maxTokens: 2000,   // Adjust based on typical batch size
                    systemPrompt: systemPrompt);

                var parsedTraits = ParseJsonResponse<List<ParsedTrait>>(response);

                return parsedTraits.Select(pt => ConvertToTraitDbEntity(pt, monsterId)).ToList();
            }
            catch (Exception ex)
            {
                // Fallback: create basic traits if parsing fails
                return CreateFallbackTraits(monsterDto.Traits, monsterId);
            }
        }

        private string GetSystemPrompt()
        {
            return @"You are a D&D 5e trait parser. Extract structured data from monster trait descriptions.
Return ONLY valid JSON array. Each trait should be an object with these fields:
{
  ""title"": ""string"",
  ""description"": ""string"",
  ""attackBonus"": ""number or null"",
  ""damageBonus"": ""number or null"",
  ""damageDice"": ""d4"" | ""d6"" | ""d8"" | ""d10"" | ""d12"" | ""d20"" | null,
  ""numberDamageDice"": ""number or null"",
  ""damageType"": ""Slashing"" | ""Piercing"" | ""Bludgeoning"" | ""Fire"" | ""Cold"" | ""Lightning"" | ""Thunder"" | ""Acid"" | ""Poison"" | ""Necrotic"" | ""Radiant"" | ""Psychic"" | ""Force"" | null,
  ""traitTrigger"": ""string or null"",
  ""advantageCondition"": ""string or null"",
  ""disadvantageCondition"": ""string or null"",
  ""isOptional"": ""boolean""
}

Extract the trait title (usually the first part before a period or colon).
Parse any damage information (e.g., ""1d6 fire damage"").
Identify trigger conditions for reactive traits.
Mark as optional if the trait mentions ""variant"", ""optional"", or ""(optional)"".
Common traits include: Amphibious, Darkvision, Pack Tactics, Keen Senses, Magic Resistance, etc.";
        }

        private string BuildBatchMessage(List<string> traits, string monsterName)
        {
            var traitsJson = JsonSerializer.Serialize(traits);
            return $@"Parse these traits for the monster '{monsterName}':

{traitsJson}

Return a JSON array with extracted structured data for each trait.
The title should be extracted from the beginning of each trait (usually before the first period or colon).
The description should contain the full trait text.";
        }

        private T ParseJsonResponse<T>(string jsonResponse)
        {
            try
            {
                // Clean up response if needed (remove markdown code blocks if present)
                var cleanJson = jsonResponse.Trim();
                if (cleanJson.StartsWith("```json"))
                {
                    cleanJson = cleanJson.Substring(7);
                }
                if (cleanJson.StartsWith("```"))
                {
                    cleanJson = cleanJson.Substring(3);
                }
                if (cleanJson.EndsWith("```"))
                {
                    cleanJson = cleanJson.Substring(0, cleanJson.Length - 3);
                }

                return JsonSerializer.Deserialize<T>(cleanJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }) ?? throw new InvalidOperationException("Failed to deserialize response");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse JSON response: {ex.Message}", ex);
            }
        }

        private TraitDbEntity ConvertToTraitDbEntity(ParsedTrait parsed, Guid monsterId)
        {
            return new TraitDbEntity
            {
                Id = Guid.NewGuid(),
                MonsterId = monsterId,
                Title = parsed.Title ?? "Unknown Trait",
                Description = parsed.Description ?? "",
                AttackBonus = parsed.AttackBonus,
                DamageBonus = parsed.DamageBonus,
                DamageDice = parsed.DamageDice != null && Enum.TryParse<DiceTypeEnum>(parsed.DamageDice, out var dd) ? dd : null,
                NumberDamageDice = parsed.NumberDamageDice,
                DamageType = parsed.DamageType != null && Enum.TryParse<DamageTypeEnum>(parsed.DamageType, out var dt) ? dt : null,
                traitTrigger = parsed.TraitTrigger,
                advantageCondition = parsed.AdvantageCondition,
                disadvantageCondition = parsed.DisadvantageCondition,
                IsOptional = parsed.IsOptional
            };
        }

        private List<TraitDbEntity> CreateFallbackTraits(List<string> descriptions, Guid monsterId)
        {
            return descriptions.Select(desc => new TraitDbEntity
            {
                Id = Guid.NewGuid(),
                MonsterId = monsterId,
                Title = ExtractTraitTitle(desc),
                Description = desc,
                IsOptional = desc.Contains("optional", StringComparison.OrdinalIgnoreCase) ||
                             desc.Contains("variant", StringComparison.OrdinalIgnoreCase)
            }).ToList();
        }

        private string ExtractTraitTitle(string description)
        {
            // Extract title - usually text before first period or colon
            // Common pattern: "Trait Name. Description..." or "Trait Name: Description..."
            var endIndex = description.IndexOfAny(new[] { '.', ':' });
            if (endIndex > 0 && endIndex < 50) // Reasonable title length
            {
                var title = description.Substring(0, endIndex).Trim();

                // Remove common prefixes if present
                if (title.StartsWith("***") && title.EndsWith("***"))
                {
                    title = title.Substring(3, title.Length - 6).Trim();
                }
                else if (title.StartsWith("**") && title.EndsWith("**"))
                {
                    title = title.Substring(2, title.Length - 4).Trim();
                }

                return title;
            }

            // Fallback: use first few words
            var words = description.Split(' ').Take(3);
            return string.Join(" ", words) + (description.Split(' ').Length > 3 ? "..." : "");
        }

        // DTO for parsing OpenAI response
        private class ParsedTrait
        {
            public string? Title { get; set; }
            public string? Description { get; set; }
            public int? AttackBonus { get; set; }
            public int? DamageBonus { get; set; }
            public string? DamageDice { get; set; }
            public int? NumberDamageDice { get; set; }
            public string? DamageType { get; set; }
            public string? TraitTrigger { get; set; }
            public string? AdvantageCondition { get; set; }
            public string? DisadvantageCondition { get; set; }
            public bool IsOptional { get; set; }
        }
    }
}