using Common.ResultPattern;
using System.Text;
using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Core.Enums;

namespace TWP.Api.Application.Helpers
{
    /// <summary>
    /// Static helper class for formatting D&D 5e encounter data
    /// </summary>
    public static class EncounterFormatter
    {
        /// <summary>
        /// Gets formatted text representation of an encounter with full null checking
        /// </summary>
        /// <param name="encounterDifficulty">Difficulty of the encounter</param>
        /// <param name="playerLevels">Player levels</param>
        /// <param name="encounterNarrativeContext">Narrative context</param>
        /// <param name="monsterHabitats">Monster habitats</param>
        /// <param name="pickedMonsters">Selected monsters for the encounter</param>
        /// <param name="expEncounterBudget">XP budget for the encounter</param>
        /// <param name="formatType">Type of formatting (Full, Summary, or List)</param>
        /// <returns>Formatted text representation of the encounter</returns>
        public static Result<string> GetFormattedEncounterSafe(
            EncounterDifficultyEnum? encounterDifficulty,
            IList<int> playerLevels,
            string encounterNarrativeContext,
            IList<MonsterHabitatEnum> monsterHabitats,
            IList<Monster5eDto> pickedMonsters,
            int? expEncounterBudget,
            string formatType = "full")
        {
            try
            {
                var sb = new StringBuilder();

                // Header
                sb.AppendLine("====================================");
                sb.AppendLine("       D&D 5E ENCOUNTER DATA       ");
                sb.AppendLine("====================================");
                sb.AppendLine();

                // Encounter Details Section
                var hasEncounterDetails = encounterDifficulty.HasValue || expEncounterBudget.HasValue;
                if (hasEncounterDetails)
                {
                    sb.AppendLine("ENCOUNTER DETAILS:");
                    sb.AppendLine("------------------");

                    if (encounterDifficulty.HasValue)
                    {
                        sb.AppendLine($"Difficulty: {encounterDifficulty.Value.ToString()}");
                    }

                    if (expEncounterBudget.HasValue && expEncounterBudget.Value > 0)
                    {
                        sb.AppendLine($"XP Budget: {expEncounterBudget.Value:N0} XP");
                    }
                    sb.AppendLine();
                }

                // Party Information
                if (playerLevels?.Any() == true)
                {
                    sb.AppendLine("PARTY INFORMATION:");
                    sb.AppendLine("------------------");
                    sb.AppendLine($"Number of Players: {playerLevels.Count}");

                    var validLevels = playerLevels.Where(l => l > 0 && l <= 20).ToList();
                    if (validLevels.Any())
                    {
                        sb.AppendLine($"Player Levels: {string.Join(", ", validLevels.OrderBy(l => l))}");
                        sb.AppendLine($"Minimum Level: {validLevels.Min()}");
                        sb.AppendLine($"Maximum Level: {validLevels.Max()}");
                        sb.AppendLine($"Average Level: {validLevels.Average():F1}");
                    }
                    sb.AppendLine();
                }

                // Narrative Context
                if (!string.IsNullOrWhiteSpace(encounterNarrativeContext))
                {
                    sb.AppendLine("NARRATIVE CONTEXT:");
                    sb.AppendLine("------------------");
                    // Trim and format the narrative context
                    var formattedContext = encounterNarrativeContext.Trim();
                    // Word wrap if needed
                    sb.AppendLine(WrapText(formattedContext, 80));
                    sb.AppendLine();
                }

                // Monster Habitats
                if (monsterHabitats?.Any() == true)
                {
                    sb.AppendLine("MONSTER HABITATS:");
                    sb.AppendLine("------------------");
                    var habitatList = monsterHabitats
                        .Where(h => h != null)
                        .Select(h => h.ToString())
                        .Distinct()
                        .OrderBy(h => h);
                    sb.AppendLine(string.Join(", ", habitatList));
                    sb.AppendLine();
                }

                // Monsters Section
                if (pickedMonsters?.Any() == true)
                {
                    sb.AppendLine("ENCOUNTER MONSTERS:");
                    sb.AppendLine("------------------");

                    // Filter out null monsters
                    var validMonsters = pickedMonsters.Where(m => m != null).ToList();

                    // Calculate total XP from monsters
                    var totalMonsterXP = validMonsters.Where(m => m.Xp > 0).Sum(m => m.Xp);
                    sb.AppendLine($"Total Monsters: {validMonsters.Count}");

                    if (totalMonsterXP > 0)
                    {
                        sb.AppendLine($"Total XP from Monsters: {totalMonsterXP:N0} XP");
                    }

                    if (expEncounterBudget.HasValue && expEncounterBudget.Value > 0 && totalMonsterXP > 0)
                    {
                        var remainingBudget = expEncounterBudget.Value - totalMonsterXP;
                        sb.AppendLine($"Remaining XP Budget: {remainingBudget:N0} XP");
                        var budgetUsagePercentage = (totalMonsterXP / (double)expEncounterBudget.Value) * 100;
                        sb.AppendLine($"Budget Usage: {budgetUsagePercentage:F1}%");
                    }

                    sb.AppendLine();

                    // Group monsters by name for count
                    var monsterGroups = validMonsters
                        .Where(m => !string.IsNullOrWhiteSpace(m.Name))
                        .GroupBy(m => m.Name)
                        .OrderByDescending(g => g.Count())
                        .ThenBy(g => g.Key);

                    if (monsterGroups.Any())
                    {
                        sb.AppendLine("Monster Summary:");
                        foreach (var group in monsterGroups)
                        {
                            var count = group.Count();
                            var firstMonster = group.First();
                            var crDisplay = firstMonster.ChallengeRating.ToString();
                            var xpDisplay = firstMonster.Xp > 0 ? $"{firstMonster.Xp:N0}" : "?";

                            if (count > 1)
                            {
                                sb.AppendLine($"  • {count}x {group.Key} (CR {crDisplay}, {xpDisplay} XP each)");
                            }
                            else
                            {
                                sb.AppendLine($"  • {group.Key} (CR {crDisplay}, {xpDisplay} XP)");
                            }
                        }
                        sb.AppendLine();
                    }

                    sb.AppendLine("====================================");
                    sb.AppendLine();

                    // Add detailed monster data based on format type
                    var monsterData = (formatType?.ToLower() ?? "summary") switch
                    {
                        "full" => MonsterFormatterHelper.FormatMonstersList(validMonsters),
                        "list" => MonsterFormatterHelper.FormatMonstersList(validMonsters),
                        "summary" => MonsterFormatterHelper.FormatMonstersSummary(validMonsters),
                        _ => MonsterFormatterHelper.FormatMonstersSummary(validMonsters)
                    };

                    if (!string.IsNullOrWhiteSpace(monsterData))
                    {
                        sb.AppendLine("DETAILED MONSTER STATISTICS:");
                        sb.AppendLine("============================");
                        sb.AppendLine();
                        sb.AppendLine(monsterData);
                    }
                }
                else
                {
                    sb.AppendLine("NO MONSTERS SELECTED FOR THIS ENCOUNTER");
                }

                // Footer with timestamp
                sb.AppendLine();
                sb.AppendLine("====================================");
                sb.AppendLine($"Generated: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC");
                sb.AppendLine("====================================");

                return Result<string>.Success(sb.ToString());
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Error formatting encounter: {ex.Message}", ReasonType.Failure);
            }
        }

        /// <summary>
        /// Gets formatted text representation of an encounter (simple version with non-nullable parameters)
        /// </summary>
        /// <param name="encounterDifficulty">Difficulty of the encounter</param>
        /// <param name="playerLevels">Player levels</param>
        /// <param name="encounterNarrativeContext">Narrative context</param>
        /// <param name="monsterHabitats">Monster habitats</param>
        /// <param name="pickedMonsters">Selected monsters for the encounter</param>
        /// <param name="expEncounterBudget">XP budget for the encounter</param>
        /// <param name="formatType">Type of formatting (Full, Summary, or List)</param>
        /// <returns>Formatted text representation of the encounter</returns>
        public static Result<string> GetFormattedEncounter(
            EncounterDifficultyEnum encounterDifficulty,
            IList<int> playerLevels,
            string encounterNarrativeContext,
            IList<MonsterHabitatEnum> monsterHabitats,
            IList<Monster5eDto> pickedMonsters,
            int expEncounterBudget,
            string formatType = "full")
        {
            // Convert to nullable and call the safe version
            return GetFormattedEncounterSafe(
                encounterDifficulty,
                playerLevels,
                encounterNarrativeContext,
                monsterHabitats,
                pickedMonsters,
                expEncounterBudget,
                formatType
            );
        }

        /// <summary>
        /// Helper method for text wrapping
        /// </summary>
        /// <param name="text">Text to wrap</param>
        /// <param name="maxLineLength">Maximum length of each line</param>
        /// <returns>Wrapped text with line breaks</returns>
        public static string WrapText(string text, int maxLineLength)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length <= maxLineLength)
                return text;

            var words = text.Split(' ');
            var lines = new List<string>();
            var currentLine = new StringBuilder();

            foreach (var word in words)
            {
                if (currentLine.Length + word.Length + 1 > maxLineLength)
                {
                    if (currentLine.Length > 0)
                    {
                        lines.Add(currentLine.ToString());
                        currentLine.Clear();
                    }
                }

                if (currentLine.Length > 0)
                    currentLine.Append(" ");
                currentLine.Append(word);
            }

            if (currentLine.Length > 0)
                lines.Add(currentLine.ToString());

            return string.Join(Environment.NewLine, lines);
        }

        /// <summary>
        /// Gets a brief summary of the encounter without full monster details
        /// </summary>
        /// <param name="encounterDifficulty">Difficulty of the encounter</param>
        /// <param name="playerLevels">Player levels</param>
        /// <param name="pickedMonsters">Selected monsters for the encounter</param>
        /// <param name="expEncounterBudget">XP budget for the encounter</param>
        /// <returns>Brief summary text of the encounter</returns>
        public static string GetEncounterSummary(
            EncounterDifficultyEnum encounterDifficulty,
            IList<int> playerLevels,
            IList<Dnd5eApiMonsterDTO> pickedMonsters,
            int expEncounterBudget)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"Encounter: {encounterDifficulty} difficulty for {playerLevels?.Count ?? 0} players");

            if (playerLevels?.Any() == true)
            {
                sb.AppendLine($"Party levels: {string.Join(", ", playerLevels.OrderBy(l => l))} (avg: {playerLevels.Average():F1})");
            }

            sb.AppendLine($"XP Budget: {expEncounterBudget:N0} XP");

            if (pickedMonsters?.Any() == true)
            {
                var totalXp = pickedMonsters.Sum(m => m.Xp);
                sb.AppendLine($"Monsters: {pickedMonsters.Count} total ({totalXp:N0} XP)");

                var monsterGroups = pickedMonsters
                    .GroupBy(m => m.Name)
                    .OrderByDescending(g => g.Count());

                foreach (var group in monsterGroups)
                {
                    var count = group.Count();
                    var firstMonster = group.First();
                    sb.AppendLine($"  - {(count > 1 ? $"{count}x " : "")}{group.Key} (CR {firstMonster.ChallengeRating})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        /// <summary>
        /// Validates encounter parameters
        /// </summary>
        /// <param name="playerLevels">Player levels to validate</param>
        /// <param name="expEncounterBudget">XP budget to validate</param>
        /// <returns>Result with success or failure message</returns>
        public static Result<bool> ValidateEncounterParameters(
            IList<int> playerLevels,
            int expEncounterBudget)
        {
            if (playerLevels == null || !playerLevels.Any())
            {
                return Result<bool>.Failure("Player levels cannot be null or empty", ReasonType.BadParameter);
            }

            if (playerLevels.Any(l => l < 1 || l > 20))
            {
                return Result<bool>.Failure("All player levels must be between 1 and 20", ReasonType.BadParameter);
            }

            if (expEncounterBudget <= 0)
            {
                return Result<bool>.Failure("XP budget must be greater than 0", ReasonType.BadParameter);
            }

            return Result<bool>.Success(true);
        }
    }
}