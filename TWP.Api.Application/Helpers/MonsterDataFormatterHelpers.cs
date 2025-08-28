using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TWP.Api.Core.DataTransferObjects;

namespace TWP.Api.Application.Helpers
{
    /// <summary>
    /// Static helper class for formatting D&D 5e monster data
    /// </summary>
    public static class MonsterDataFormatterHelpers
    {
        /// <summary>
        /// Formats a list of monsters with full details
        /// </summary>
        /// <param name="monsters">List of monsters to format</param>
        /// <returns>Formatted string with detailed monster information</returns>
        public static string FormatMonstersList(IList<Dnd5eApiMonsterDTO> monsters)
        {
            if (monsters == null || !monsters.Any())
                return "No monsters to display.";

            var sb = new StringBuilder();
            var monsterCount = 0;

            foreach (var monster in monsters.Where(m => m != null))
            {
                monsterCount++;
                if (monsterCount > 1)
                {
                    sb.AppendLine();
                    sb.AppendLine("------------------------------------");
                    sb.AppendLine();
                }

                AppendMonsterDetails(sb, monster, true);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Formats a list of monsters with summary information
        /// </summary>
        /// <param name="monsters">List of monsters to format</param>
        /// <returns>Formatted string with summary monster information</returns>
        public static string FormatMonstersSummary(IList<Dnd5eApiMonsterDTO> monsters)
        {
            if (monsters == null || !monsters.Any())
                return "No monsters to display.";

            var sb = new StringBuilder();
            var monsterCount = 0;

            foreach (var monster in monsters.Where(m => m != null))
            {
                monsterCount++;
                if (monsterCount > 1)
                {
                    sb.AppendLine();
                    sb.AppendLine("------------------------------------");
                    sb.AppendLine();
                }

                AppendMonsterDetails(sb, monster, false);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Appends monster details to StringBuilder
        /// </summary>
        /// <param name="sb">StringBuilder to append to</param>
        /// <param name="monster">Monster to format</param>
        /// <param name="fullDetails">Whether to include full details or just summary</param>
        private static void AppendMonsterDetails(StringBuilder sb, Dnd5eApiMonsterDTO monster, bool fullDetails)
        {
            // Monster Name and Type
            sb.AppendLine($"【 {monster.Name?.ToUpper() ?? "UNKNOWN"} 】");
            sb.AppendLine($"{monster.Size ?? "?"} {monster.Type ?? "?"}, {monster.Alignment ?? "unaligned"}");
            sb.AppendLine();

            // Basic Stats
            sb.AppendLine("BASIC STATISTICS:");

            // Armor Class
            if (monster.ArmorClass?.Any() == true)
            {
                var acInfo = monster.ArmorClass.First();
                sb.AppendLine($"  Armor Class: {acInfo.Value}" +
                    (!string.IsNullOrWhiteSpace(acInfo.Type) ? $" ({acInfo.Type})" : ""));
            }

            // Hit Points
            sb.AppendLine($"  Hit Points: {monster.HitPoints} " +
                (!string.IsNullOrWhiteSpace(monster.HitPointsRoll) ? $"({monster.HitPointsRoll})" :
                 !string.IsNullOrWhiteSpace(monster.HitDice) ? $"({monster.HitDice})" : ""));

            // Speed
            if (monster.Speed != null)
            {
                var speeds = new List<string>();
                if (!string.IsNullOrWhiteSpace(monster.Speed.Walk))
                    speeds.Add($"{monster.Speed.Walk}");
                if (!string.IsNullOrWhiteSpace(monster.Speed.Swim))
                    speeds.Add($"swim {monster.Speed.Swim}");

                if (speeds.Any())
                    sb.AppendLine($"  Speed: {string.Join(", ", speeds)}");
            }

            // Challenge Rating and XP
            sb.AppendLine($"  Challenge Rating: {monster.ChallengeRating} ({monster.Xp:N0} XP)");
            sb.AppendLine();

            // Ability Scores
            sb.AppendLine("ABILITIES:");
            sb.AppendLine($"  STR: {monster.Strength} ({GetModifierString(monster.Strength)})  " +
                         $"DEX: {monster.Dexterity} ({GetModifierString(monster.Dexterity)})  " +
                         $"CON: {monster.Constitution} ({GetModifierString(monster.Constitution)})");
            sb.AppendLine($"  INT: {monster.Intelligence} ({GetModifierString(monster.Intelligence)})  " +
                         $"WIS: {monster.Wisdom} ({GetModifierString(monster.Wisdom)})  " +
                         $"CHA: {monster.Charisma} ({GetModifierString(monster.Charisma)})");
            sb.AppendLine();

            if (fullDetails)
            {
                // Proficiencies
                if (monster.Proficiencies?.Any() == true)
                {
                    var savingThrows = monster.Proficiencies
                        .Where(p => p.ProficiencyDetail?.Name?.Contains("Saving Throw") == true)
                        .ToList();

                    var skills = monster.Proficiencies
                        .Where(p => p.ProficiencyDetail?.Name?.Contains("Skill") == true)
                        .ToList();

                    if (savingThrows.Any())
                    {
                        sb.AppendLine("SAVING THROWS:");
                        foreach (var save in savingThrows)
                        {
                            var name = save.ProficiencyDetail.Name.Replace("Saving Throw: ", "");
                            sb.AppendLine($"  {name}: +{save.Value}");
                        }
                        sb.AppendLine();
                    }

                    if (skills.Any())
                    {
                        sb.AppendLine("SKILLS:");
                        foreach (var skill in skills)
                        {
                            var name = skill.ProficiencyDetail.Name.Replace("Skill: ", "");
                            sb.AppendLine($"  {name}: +{skill.Value}");
                        }
                        sb.AppendLine();
                    }
                }

                // Damage Immunities/Resistances/Vulnerabilities
                if (monster.DamageVulnerabilities?.Any() == true)
                {
                    sb.AppendLine($"Damage Vulnerabilities: {string.Join(", ", monster.DamageVulnerabilities)}");
                }
                if (monster.DamageResistances?.Any() == true)
                {
                    sb.AppendLine($"Damage Resistances: {string.Join(", ", monster.DamageResistances)}");
                }
                if (monster.DamageImmunities?.Any() == true)
                {
                    sb.AppendLine($"Damage Immunities: {string.Join(", ", monster.DamageImmunities)}");
                }
                if (monster.ConditionImmunities?.Any() == true)
                {
                    sb.AppendLine($"Condition Immunities: {string.Join(", ", monster.ConditionImmunities)}");
                }

                // Senses
                if (monster.Senses != null)
                {
                    var sensesList = new List<string>();
                    if (!string.IsNullOrWhiteSpace(monster.Senses.Darkvision))
                        sensesList.Add($"darkvision {monster.Senses.Darkvision}");
                    sensesList.Add($"passive Perception {monster.Senses.PassivePerception}");

                    sb.AppendLine($"Senses: {string.Join(", ", sensesList)}");
                }

                // Languages
                if (!string.IsNullOrWhiteSpace(monster.Languages))
                {
                    sb.AppendLine($"Languages: {monster.Languages}");
                }

                if (monster.DamageVulnerabilities?.Any() == true ||
                    monster.DamageResistances?.Any() == true ||
                    monster.DamageImmunities?.Any() == true ||
                    monster.ConditionImmunities?.Any() == true ||
                    monster.Senses != null ||
                    !string.IsNullOrWhiteSpace(monster.Languages))
                {
                    sb.AppendLine();
                }

                // Special Abilities
                if (monster.SpecialAbilities?.Any() == true)
                {
                    sb.AppendLine("SPECIAL ABILITIES:");
                    foreach (var ability in monster.SpecialAbilities)
                    {
                        sb.AppendLine($"  • {ability.Name}");
                        if (!string.IsNullOrWhiteSpace(ability.Desc))
                        {
                            var wrapped = WrapAndIndent(ability.Desc, 76, 4);
                            sb.AppendLine(wrapped);
                        }
                    }
                    sb.AppendLine();
                }

                // Actions
                if (monster.Actions?.Any() == true)
                {
                    sb.AppendLine("ACTIONS:");
                    foreach (var action in monster.Actions)
                    {
                        sb.AppendLine($"  • {action.Name}");

                        if (action.AttackBonus.HasValue)
                        {
                            sb.AppendLine($"    Attack Bonus: +{action.AttackBonus}");
                        }

                        if (!string.IsNullOrWhiteSpace(action.Desc))
                        {
                            var wrapped = WrapAndIndent(action.Desc, 76, 4);
                            sb.AppendLine(wrapped);
                        }

                        if (action.DC != null)
                        {
                            sb.AppendLine($"    DC {action.DC.DCValue} {action.DC.DCType?.Name ?? "check"}");
                        }
                    }
                    sb.AppendLine();
                }

                // Legendary Actions
                if (monster.LegendaryActions?.Any() == true)
                {
                    sb.AppendLine("LEGENDARY ACTIONS:");
                    sb.AppendLine("  The creature can take 3 legendary actions, choosing from the options below.");
                    sb.AppendLine("  Only one legendary action option can be used at a time and only at the end");
                    sb.AppendLine("  of another creature's turn. The creature regains spent legendary actions at");
                    sb.AppendLine("  the start of its turn.");
                    sb.AppendLine();

                    foreach (var legendary in monster.LegendaryActions)
                    {
                        sb.AppendLine($"  • {legendary.Name}");
                        if (!string.IsNullOrWhiteSpace(legendary.Desc))
                        {
                            var wrapped = WrapAndIndent(legendary.Desc, 76, 4);
                            sb.AppendLine(wrapped);
                        }
                    }
                    sb.AppendLine();
                }

                // Reactions (if any)
                if (monster.Reactions?.Any() == true)
                {
                    sb.AppendLine("REACTIONS:");
                    foreach (var reaction in monster.Reactions)
                    {
                        sb.AppendLine($"  • {reaction}");
                    }
                    sb.AppendLine();
                }
            }
        }

        /// <summary>
        /// Calculates ability modifier from ability score
        /// </summary>
        /// <param name="abilityScore">Ability score value</param>
        /// <returns>Modifier string with + or - prefix</returns>
        private static string GetModifierString(int abilityScore)
        {
            var modifier = (abilityScore - 10) / 2;
            return modifier >= 0 ? $"+{modifier}" : modifier.ToString();
        }

        /// <summary>
        /// Wraps text and adds indentation
        /// </summary>
        /// <param name="text">Text to wrap</param>
        /// <param name="maxLineLength">Maximum line length</param>
        /// <param name="indentSpaces">Number of spaces to indent</param>
        /// <returns>Wrapped and indented text</returns>
        private static string WrapAndIndent(string text, int maxLineLength, int indentSpaces)
        {
            if (string.IsNullOrWhiteSpace(text))
                return "";

            var indent = new string(' ', indentSpaces);
            var words = text.Split(' ');
            var lines = new List<string>();
            var currentLine = new StringBuilder();

            foreach (var word in words)
            {
                if (currentLine.Length + word.Length + 1 > maxLineLength)
                {
                    if (currentLine.Length > 0)
                    {
                        lines.Add(indent + currentLine.ToString());
                        currentLine.Clear();
                    }
                }

                if (currentLine.Length > 0)
                    currentLine.Append(" ");
                currentLine.Append(word);
            }

            if (currentLine.Length > 0)
                lines.Add(indent + currentLine.ToString());

            return string.Join(Environment.NewLine, lines);
        }
    }
}