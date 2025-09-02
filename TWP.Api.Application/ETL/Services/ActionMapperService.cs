using System.Text.Json;
using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Core.DbEntities;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.Interops.Interfaces;

namespace TWP.Api.Application.ETL.Services
{
    public class ActionMapperService : IActionMapperService
    {
        private readonly IOpenAiInterops _openAiInterops;

        public ActionMapperService(IOpenAiInterops openAiInterops)
        {
            _openAiInterops = openAiInterops;
        }

        public async Task<List<ActionDbEntity>> MapMonsterActionsAsync(
            AideDdMonsterResponseDto monsterDto,
            Guid monsterId)
        {
            var allActions = new List<ActionDbEntity>();

            // Process each action type
            if (monsterDto.Actions?.Any() == true)
            {
                var actions = await ProcessActionBatchAsync(
                    monsterDto.Actions,
                    ActionTypeEnum.Action,
                    monsterId,
                    monsterDto.Name);
                allActions.AddRange(actions);
            }

            if (monsterDto.ReactionActions?.Any() == true)
            {
                var reactions = await ProcessActionBatchAsync(
                    monsterDto.ReactionActions,
                    ActionTypeEnum.Reaction,
                    monsterId,
                    monsterDto.Name);
                allActions.AddRange(reactions);
            }

            if (monsterDto.BonusActions?.Any() == true)
            {
                var bonusActions = await ProcessActionBatchAsync(
                    monsterDto.BonusActions,
                    ActionTypeEnum.Bonus,
                    monsterId,
                    monsterDto.Name);
                allActions.AddRange(bonusActions);
            }

            if (monsterDto.LegendaryActions?.Any() == true)
            {
                var legendaryActions = await ProcessActionBatchAsync(
                    monsterDto.LegendaryActions,
                    ActionTypeEnum.Legendary,
                    monsterId,
                    monsterDto.Name);
                allActions.AddRange(legendaryActions);
            }

            return allActions;
        }

        private async Task<List<ActionDbEntity>> ProcessActionBatchAsync(
            List<string> actionDescriptions,
            ActionTypeEnum actionType,
            Guid monsterId,
            string monsterName)
        {
            var systemPrompt = GetSystemPrompt();
            var userMessage = BuildBatchMessage(actionDescriptions, actionType, monsterName);

            try
            {
                // Use a lower temperature for more consistent parsing
                var response = await _openAiInterops.GetChatGptResponseAsync(
                    userMessage,
                    temperature: 0.1,  // Low temperature for consistency
                    maxTokens: 2000,   // Adjust based on typical batch size
                    systemPrompt: systemPrompt);

                var parsedActions = ParseJsonResponse<List<ParsedAction>>(response);

                return parsedActions.Select(pa => ConvertToActionDbEntity(pa, monsterId, actionType)).ToList();
            }
            catch (Exception ex)
            {
                // Fallback: create basic actions if parsing fails
                return CreateFallbackActions(actionDescriptions, monsterId, actionType);
            }
        }

        private string GetSystemPrompt()
        {
            return @"You are a D&D 5e action parser. Extract structured data from monster action descriptions.
Return ONLY valid JSON array. Each action should be an object with these fields:
{
  ""name"": ""string"",
  ""attackType"": ""Melee"" | ""Ranged"" | ""None"",
  ""description"": ""string"",
  ""shortRange"": ""number or null"",
  ""longRange"": ""number or null"",
  ""attackBonus"": ""number or null"",
  ""damageBonus"": ""number or null"",
  ""damageDice"": ""d4"" | ""d6"" | ""d8"" | ""d10"" | ""d12"" | ""d20"" | null,
  ""numberDamageDice"": ""number or null"",
  ""damageType"": ""Slashing"" | ""Piercing"" | ""Bludgeoning"" | ""Fire"" | ""Cold"" | ""Lightning"" | ""Thunder"" | ""Acid"" | ""Poison"" | ""Necrotic"" | ""Radiant"" | ""Psychic"" | ""Force"" | null,
  ""limitPerDay"": ""number or null"",
  ""isProhibitedForMinion"": ""boolean"",
  ""actionTrigger"": ""string or null"",
  ""advantageCondition"": ""string or null"",
  ""disadvantageCondition"": ""string or null""
}

Parse attack bonuses (e.g., ""+7 to hit""), damage (e.g., ""2d6 + 4""), and ranges (e.g., ""reach 5 ft."" or ""range 80/320 ft."").
For reactions, extract trigger conditions into actionTrigger field.";
        }

        private string BuildBatchMessage(List<string> actions, ActionTypeEnum actionType, string monsterName)
        {
            var actionsJson = JsonSerializer.Serialize(actions);
            return $@"Parse these {actionType} actions for the monster '{monsterName}':

{actionsJson}

Return a JSON array with extracted structured data for each action.";
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

        private ActionDbEntity ConvertToActionDbEntity(ParsedAction parsed, Guid monsterId, ActionTypeEnum actionType)
        {
            return new ActionDbEntity
            {
                Id = Guid.NewGuid(),
                MonsterId = monsterId,
                Name = parsed.Name ?? "Unknown Action",
                Type = actionType,
                AttackType = Enum.TryParse<AttackTypeEnum>(parsed.AttackType, out var at) ? at : AttackTypeEnum.None,
                Description = parsed.Description ?? "",
                ShortRange = parsed.ShortRange,
                LongRange = parsed.LongRange,
                AttackBonus = parsed.AttackBonus,
                DamageBonus = parsed.DamageBonus,
                DamageDice = parsed.DamageDice != null && Enum.TryParse<DiceTypeEnum>(parsed.DamageDice, out var dd) ? dd : null,
                NumberDamageDice = parsed.NumberDamageDice,
                DamageType = parsed.DamageType != null && Enum.TryParse<DamageTypeEnum>(parsed.DamageType, out var dt) ? dt : null,
                LimitPerDay = parsed.LimitPerDay,
                IsProhibitedForMinion = parsed.IsProhibitedForMinion,
                actionTrigger = parsed.ActionTrigger,
                advantageCondition = parsed.AdvantageCondition,
                disadvantageCondition = parsed.DisadvantageCondition
            };
        }

        private List<ActionDbEntity> CreateFallbackActions(List<string> descriptions, Guid monsterId, ActionTypeEnum actionType)
        {
            return descriptions.Select(desc => new ActionDbEntity
            {
                Id = Guid.NewGuid(),
                MonsterId = monsterId,
                Name = ExtractActionName(desc),
                Type = actionType,
                AttackType = AttackTypeEnum.None,
                Description = desc,
                IsProhibitedForMinion = false
            }).ToList();
        }

        private string ExtractActionName(string description)
        {
            // Simple extraction: take text before first period or colon
            var endIndex = description.IndexOfAny(new[] { '.', ':' });
            if (endIndex > 0 && endIndex < 50) // Reasonable name length
            {
                return description.Substring(0, endIndex).Trim();
            }
            return description.Length > 50 ? description.Substring(0, 50) + "..." : description;
        }

        // DTO for parsing OpenAI response
        private class ParsedAction
        {
            public string? Name { get; set; }
            public string? AttackType { get; set; }
            public string? Description { get; set; }
            public string? ShortRange { get; set; }
            public string? LongRange { get; set; }
            public int? AttackBonus { get; set; }
            public int? DamageBonus { get; set; }
            public string? DamageDice { get; set; }
            public int? NumberDamageDice { get; set; }
            public string? DamageType { get; set; }
            public int? LimitPerDay { get; set; }
            public bool IsProhibitedForMinion { get; set; }
            public string? ActionTrigger { get; set; }
            public string? AdvantageCondition { get; set; }
            public string? DisadvantageCondition { get; set; }
        }
    }
}
