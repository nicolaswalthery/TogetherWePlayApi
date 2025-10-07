using System.Text;
using TWP.Api.Core.DataTransferObjects;

namespace TWP.Api.Application.Helpers
{
    /// <summary>
    /// Static helper class for formatting D&D 5e monster data from Monster5eDto
    /// </summary>
    public static class MonsterFormatterHelper
    {
        /// <summary>
        /// Formats a single monster with full details
        /// </summary>
        public static string FormatMonster(Monster5eDto monster, bool fullDetails = true)
        {
            if (monster == null)
                return "No monster to display.";

            var sb = new StringBuilder();
            AppendMonsterDetails(sb, monster, fullDetails);
            return sb.ToString();
        }

        /// <summary>
        /// Formats a list of monsters with full details
        /// </summary>
        public static string FormatMonstersList(IList<Monster5eDto> monsters)
        {
            if (monsters == null || !monsters.Any())
                return "No monsters to display.";

            var sb = new StringBuilder();
            var isFirst = true;

            foreach (var monster in monsters.Where(m => m != null))
            {
                if (!isFirst)
                {
                    sb.AppendLine();
                    sb.AppendLine("════════════════════════════════════════════════════════════════════════");
                    sb.AppendLine();
                }
                isFirst = false;

                AppendMonsterDetails(sb, monster, true);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Formats a list of monsters with summary information
        /// </summary>
        public static string FormatMonstersSummary(IList<Monster5eDto> monsters)
        {
            if (monsters == null || !monsters.Any())
                return "No monsters to display.";

            var sb = new StringBuilder();
            sb.AppendLine("MONSTER SUMMARY");
            sb.AppendLine("═══════════════");
            sb.AppendLine();

            foreach (var monster in monsters.Where(m => m != null))
            {
                AppendMonsterSummary(sb, monster);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Creates a table format for multiple monsters
        /// </summary>
        public static string FormatMonstersTable(IList<Monster5eDto> monsters)
        {
            if (monsters == null || !monsters.Any())
                return "No monsters to display.";

            var sb = new StringBuilder();

            // Header
            sb.AppendLine("┌────────────────────────┬──────┬─────┬────────┬───────────────────────┐");
            sb.AppendLine("│ Name                   │ CR   │ AC  │ HP     │ Type                  │");
            sb.AppendLine("├────────────────────────┼──────┼─────┼────────┼───────────────────────┤");

            foreach (var monster in monsters.Where(m => m != null))
            {
                var name = TruncateString(monster.Name, 22);
                var cr = TruncateString(monster.ChallengeRating, 4);
                var ac = TruncateString(monster.ArmorClass.ToString(), 3);
                var hp = TruncateString(monster.HitPoints.ToString(), 6);
                var type = TruncateString($"{monster.CreatureSize} {monster.CreatureType ?? "?"}", 21);

                sb.AppendLine($"│ {name} │ {cr} │ {ac} │ {hp} │ {type} │");
            }

            sb.AppendLine("└────────────────────────┴──────┴─────┴────────┴───────────────────────┘");

            return sb.ToString();
        }

        /// <summary>
        /// Appends detailed monster information
        /// </summary>
        private static void AppendMonsterDetails(StringBuilder sb, Monster5eDto monster, bool fullDetails)
        {
            // Header with name
            AppendMonsterHeader(sb, monster);

            // Basic information
            AppendBasicInfo(sb, monster);

            // Ability scores
            AppendAbilityScores(sb, monster);

            if (fullDetails)
            {
                // Saving throws
                AppendSavingThrows(sb, monster);

                // Skills
                AppendSkills(sb, monster);

                // Defenses
                AppendDefenses(sb, monster);

                // Senses and Languages
                AppendSensesAndLanguages(sb, monster);

                // Equipment and Habitat
                AppendEquipmentAndHabitat(sb, monster);

                // Lore
                AppendLore(sb, monster);

                // Traits
                AppendTraits(sb, monster);

                // Actions (categorized by type)
                AppendActions(sb, monster);

                // Symbarum 5e specific info
                AppendSymbarum5eInfo(sb, monster);
            }
        }

        /// <summary>
        /// Appends a summary line for a monster
        /// </summary>
        private static void AppendMonsterSummary(StringBuilder sb, Monster5eDto monster)
        {
            sb.AppendLine($"• {monster.Name} - CR {monster.ChallengeRating} ({monster.Xp:N0} XP)");
            sb.AppendLine($"  {monster.CreatureSize} {monster.CreatureType ?? "creature"}, {monster.Alignment ?? "unaligned"}");
            sb.AppendLine($"  AC {monster.ArmorClass}, HP {monster.HitPoints} ({monster.HitDice}), Speed {FormatSpeed(monster)}");
            if (!string.IsNullOrWhiteSpace(monster.Role))
                sb.AppendLine($"  Role: {monster.Role}");
            sb.AppendLine();
        }

        private static void AppendMonsterHeader(StringBuilder sb, Monster5eDto monster)
        {
            var name = monster.Name.ToUpper();
            var border = new string('═', Math.Min(name.Length + 4, 76));

            sb.AppendLine(border);
            sb.AppendLine($"║ {name} ║");
            sb.AppendLine(border);
            sb.AppendLine();
        }

        private static void AppendBasicInfo(StringBuilder sb, Monster5eDto monster)
        {
            // Type line
            var typeInfo = new List<string>();
            typeInfo.Add(monster.CreatureSize);

            if (!string.IsNullOrWhiteSpace(monster.CreatureType))
            {
                typeInfo.Add(monster.CreatureType);
                if (!string.IsNullOrWhiteSpace(monster.CreatureSubType))
                    typeInfo.Add($"({monster.CreatureSubType})");
            }

            typeInfo.Add(monster.Alignment ?? "unaligned");

            sb.AppendLine(string.Join(" ", typeInfo));

            if (!string.IsNullOrWhiteSpace(monster.Role))
                sb.AppendLine($"Combat Role: {monster.Role}");

            if (!string.IsNullOrWhiteSpace(monster.MonsterGroup))
                sb.AppendLine($"Group: {monster.MonsterGroup}");

            sb.AppendLine();
            sb.AppendLine("──────────────────────────────────────");

            // Armor Class
            sb.Append($"Armor Class: {monster.ArmorClass}");
            if (monster.MinionArmorClass.HasValue)
                sb.Append($" (Minion AC: {monster.MinionArmorClass})");
            sb.AppendLine();

            // Hit Points
            sb.AppendLine($"Hit Points: {monster.HitPoints} ({monster.HitDice})");

            // Initiative
            sb.AppendLine($"Initiative Bonus: {FormatModifier(monster.InitiativeBonus)}");

            // Speed
            sb.AppendLine($"Speed: {FormatSpeed(monster)}");

            // Challenge Rating
            sb.Append($"Challenge: {monster.ChallengeRating} ({monster.Xp:N0} XP)");
            if (monster.CrInLair.HasValue)
                sb.Append($" | In Lair: CR {monster.CrInLair}");
            sb.AppendLine();

            if (monster.ProficiencyBonus.HasValue)
                sb.AppendLine($"Proficiency Bonus: {FormatModifier(monster.ProficiencyBonus.Value)}");

            sb.AppendLine("──────────────────────────────────────");
            sb.AppendLine();
        }

        private static string FormatSpeed(Monster5eDto monster)
        {
            var speeds = new List<string>();

            if (!string.IsNullOrWhiteSpace(monster.Speed))
                speeds.Add($"{monster.Speed} ft.");
            if (!string.IsNullOrWhiteSpace(monster.Climb))
                speeds.Add($"climb {monster.Climb} ft.");
            if (!string.IsNullOrWhiteSpace(monster.Fly))
                speeds.Add($"fly {monster.Fly} ft.");
            if (!string.IsNullOrWhiteSpace(monster.Swim))
                speeds.Add($"swim {monster.Swim} ft.");

            return speeds.Any() ? string.Join(", ", speeds) : "0 ft.";
        }

        private static void AppendAbilityScores(StringBuilder sb, Monster5eDto monster)
        {
            sb.AppendLine("STR     DEX     CON     INT     WIS     CHA");

            // Scores
            sb.AppendLine($"{FormatAbilityScore(monster.Strength)}  " +
                         $"{FormatAbilityScore(monster.Dexterity)}  " +
                         $"{FormatAbilityScore(monster.Constitution)}  " +
                         $"{FormatAbilityScore(monster.Intelligence)}  " +
                         $"{FormatAbilityScore(monster.Wisdom)}  " +
                         $"{FormatAbilityScore(monster.Charisma)}");

            // Modifiers
            sb.AppendLine($"{FormatAbilityModifier(monster.Strength)}   " +
                         $"{FormatAbilityModifier(monster.Dexterity)}   " +
                         $"{FormatAbilityModifier(monster.Constitution)}   " +
                         $"{FormatAbilityModifier(monster.Intelligence)}   " +
                         $"{FormatAbilityModifier(monster.Wisdom)}   " +
                         $"{FormatAbilityModifier(monster.Charisma)}");

            sb.AppendLine("──────────────────────────────────────");
            sb.AppendLine();
        }

        private static string FormatAbilityScore(int? score)
        {
            return score.HasValue ? score.Value.ToString().PadLeft(3).PadRight(5) : " -- ";
        }

        private static string FormatAbilityModifier(int? score)
        {
            if (!score.HasValue)
                return " --- ";

            var modifier = (score.Value - 10) / 2;
            return FormatModifier(modifier).PadRight(6);
        }

        private static string FormatModifier(int modifier)
        {
            return modifier >= 0 ? $"+{modifier}" : modifier.ToString();
        }

        private static void AppendSavingThrows(StringBuilder sb, Monster5eDto monster)
        {
            var saves = new List<string>();

            if (monster.StrSavingThrow.HasValue)
                saves.Add($"Str {FormatModifier(monster.StrSavingThrow.Value)}");
            if (monster.DexSavingThrow.HasValue)
                saves.Add($"Dex {FormatModifier(monster.DexSavingThrow.Value)}");
            if (monster.ConSavingThrow.HasValue)
                saves.Add($"Con {FormatModifier(monster.ConSavingThrow.Value)}");
            if (monster.IntSavingThrow.HasValue)
                saves.Add($"Int {FormatModifier(monster.IntSavingThrow.Value)}");
            if (monster.WisSavingThrow.HasValue)
                saves.Add($"Wis {FormatModifier(monster.WisSavingThrow.Value)}");
            if (monster.ChaSavingThrow.HasValue)
                saves.Add($"Cha {FormatModifier(monster.ChaSavingThrow.Value)}");

            if (saves.Any())
            {
                sb.AppendLine($"Saving Throws: {string.Join(", ", saves)}");
            }
        }

        private static void AppendSkills(StringBuilder sb, Monster5eDto monster)
        {
            if (monster.Skills == null || !monster.Skills.Any())
                return;

            var skills = new List<string>();
            foreach (var skill in monster.Skills)
            {
                var value = skill.Value?.ToString() ?? "0";
                if (int.TryParse(value, out var skillMod))
                {
                    skills.Add($"{skill.Key} {FormatModifier(skillMod)}");
                }
                else
                {
                    skills.Add($"{skill.Key} {value}");
                }
            }

            if (skills.Any())
            {
                sb.AppendLine($"Skills: {string.Join(", ", skills)}");
            }
        }

        private static void AppendDefenses(StringBuilder sb, Monster5eDto monster)
        {
            if (!string.IsNullOrWhiteSpace(monster.DamageResistances))
            {
                sb.AppendLine($"Damage Resistances: {monster.DamageResistances}");
            }

            if (!string.IsNullOrWhiteSpace(monster.DamageImmunities))
            {
                sb.AppendLine($"Damage Immunities: {monster.DamageImmunities}");
            }
        }

        private static void AppendSensesAndLanguages(StringBuilder sb, Monster5eDto monster)
        {
            if (!string.IsNullOrWhiteSpace(monster.Senses))
            {
                sb.AppendLine($"Senses: {monster.Senses}");
            }

            if (!string.IsNullOrWhiteSpace(monster.Languages))
            {
                sb.AppendLine($"Languages: {monster.Languages}");
            }
        }

        private static void AppendEquipmentAndHabitat(StringBuilder sb, Monster5eDto monster)
        {
            if (monster.Equipments != null && monster.Equipments.Any())
            {
                sb.AppendLine($"Equipment: {string.Join(", ", monster.Equipments)}");
            }

            if (!string.IsNullOrWhiteSpace(monster.Habitats))
            {
                sb.AppendLine($"Habitats: {monster.Habitats}");
            }

            if (HasAnyDefenseOrSenseInfo(monster))
            {
                sb.AppendLine("──────────────────────────────────────");
                sb.AppendLine();
            }
        }

        private static void AppendLore(StringBuilder sb, Monster5eDto monster)
        {
            if (!string.IsNullOrWhiteSpace(monster.Manner) || !string.IsNullOrWhiteSpace(monster.Lore))
            {
                sb.AppendLine("LORE & BACKGROUND");
                sb.AppendLine("─────────────────");

                if (!string.IsNullOrWhiteSpace(monster.Manner))
                {
                    sb.AppendLine($"Manner: {monster.Manner}");
                }

                if (!string.IsNullOrWhiteSpace(monster.Lore))
                {
                    WrapAndPrint(sb, monster.Lore, 0);
                }

                sb.AppendLine();
            }
        }

        private static void AppendTraits(StringBuilder sb, Monster5eDto monster)
        {
            if (monster.Traits == null || !monster.Traits.Any())
                return;

            sb.AppendLine("TRAITS");
            sb.AppendLine("──────");

            foreach (var trait in monster.Traits)
            {
                sb.AppendLine($"• {trait.Title}{(trait.IsOptional ? " (Optional)" : "")}");

                if (!string.IsNullOrWhiteSpace(trait.Description))
                {
                    WrapAndPrint(sb, trait.Description, 2);
                }

                // Damage info if present
                if (trait.NumberDamageDice.HasValue && !string.IsNullOrWhiteSpace(trait.DamageDice))
                {
                    sb.Append($"  Damage: {trait.NumberDamageDice}{trait.DamageDice}");
                    if (trait.DamageBonus.HasValue && trait.DamageBonus != 0)
                        sb.Append($" {FormatModifier(trait.DamageBonus.Value)}");
                    if (!string.IsNullOrWhiteSpace(trait.DamageType))
                        sb.Append($" {trait.DamageType}");
                    sb.AppendLine();
                }

                // Attack bonus
                if (trait.AttackBonus.HasValue)
                {
                    sb.AppendLine($"  Attack Bonus: {FormatModifier(trait.AttackBonus.Value)}");
                }

                // Trigger
                if (!string.IsNullOrWhiteSpace(trait.TraitTrigger))
                {
                    sb.AppendLine($"  Trigger: {trait.TraitTrigger}");
                }

                // Advantage/Disadvantage conditions
                if (!string.IsNullOrWhiteSpace(trait.AdvantageCondition))
                {
                    sb.AppendLine($"  Advantage: {trait.AdvantageCondition}");
                }
                if (!string.IsNullOrWhiteSpace(trait.DisadvantageCondition))
                {
                    sb.AppendLine($"  Disadvantage: {trait.DisadvantageCondition}");
                }

                sb.AppendLine();
            }
        }

        private static void AppendActions(StringBuilder sb, Monster5eDto monster)
        {
            if (monster.Actions == null || !monster.Actions.Any())
                return;

            // Group actions by type
            var actionGroups = monster.Actions.GroupBy(a => a.Type).OrderBy(g => GetActionTypeOrder(g.Key));

            foreach (var group in actionGroups)
            {
                var sectionTitle = FormatActionTypeTitle(group.Key);
                sb.AppendLine(sectionTitle);
                sb.AppendLine(new string('─', sectionTitle.Length));

                foreach (var action in group)
                {
                    AppendSingleAction(sb, action);
                }

                sb.AppendLine();
            }
        }

        private static void AppendSingleAction(StringBuilder sb, ActionDto action)
        {
            sb.AppendLine($"• {action.Name}");

            // Attack type and bonus
            if (!string.IsNullOrWhiteSpace(action.AttackType))
            {
                sb.Append($"  {action.AttackType} Attack:");
                if (action.AttackBonus.HasValue)
                    sb.Append($" {FormatModifier(action.AttackBonus.Value)} to hit,");

                // Range
                if (!string.IsNullOrWhiteSpace(action.ShortRange))
                {
                    sb.Append($" reach {action.ShortRange} ft.");
                    if (!string.IsNullOrWhiteSpace(action.LongRange))
                        sb.Append($"/{action.LongRange} ft.");
                }
                sb.AppendLine();
            }

            // Damage
            if (action.NumberDamageDice.HasValue && !string.IsNullOrWhiteSpace(action.DamageDice))
            {
                sb.Append($"  Hit: {action.NumberDamageDice}{action.DamageDice}");
                if (action.DamageBonus.HasValue && action.DamageBonus != 0)
                    sb.Append($" {FormatModifier(action.DamageBonus.Value)}");
                if (!string.IsNullOrWhiteSpace(action.DamageType))
                    sb.Append($" {action.DamageType} damage");
                sb.AppendLine();
            }

            // Description
            if (!string.IsNullOrWhiteSpace(action.Description))
            {
                WrapAndPrint(sb, action.Description, 2);
            }

            // Usage limit
            if (action.LimitPerDay.HasValue)
            {
                sb.AppendLine($"  Usage: {action.LimitPerDay}/Day");
            }

            // Trigger
            if (!string.IsNullOrWhiteSpace(action.ActionTrigger))
            {
                sb.AppendLine($"  Trigger: {action.ActionTrigger}");
            }

            // Advantage/Disadvantage
            if (!string.IsNullOrWhiteSpace(action.AdvantageCondition))
            {
                sb.AppendLine($"  Advantage when: {action.AdvantageCondition}");
            }
            if (!string.IsNullOrWhiteSpace(action.DisadvantageCondition))
            {
                sb.AppendLine($"  Disadvantage when: {action.DisadvantageCondition}");
            }

            // Minion restriction
            if (action.IsProhibitedForMinion)
            {
                sb.AppendLine($"  (Not available to minions)");
            }

            sb.AppendLine();
        }

        private static void AppendSymbarum5eInfo(StringBuilder sb, Monster5eDto monster)
        {
            if (monster.Symbarum5e == null)
                return;

            sb.AppendLine("SYMBAROUM 5E");
            sb.AppendLine("────────────");
            sb.AppendLine($"Shadow: {monster.Symbarum5e.Shadow}");
            sb.AppendLine();
        }

        private static string FormatActionTypeTitle(string actionType)
        {
            return actionType?.ToUpper() switch
            {
                "ACTION" => "ACTIONS",
                "BONUS" => "BONUS ACTIONS",
                "REACTION" => "REACTIONS",
                "LEGENDARY" => "LEGENDARY ACTIONS",
                "LAIR" => "LAIR ACTIONS",
                "MYTHIC" => "MYTHIC ACTIONS",
                _ => actionType?.ToUpper() ?? "ACTIONS"
            };
        }

        private static int GetActionTypeOrder(string actionType)
        {
            return actionType?.ToUpper() switch
            {
                "ACTION" => 1,
                "BONUS" => 2,
                "REACTION" => 3,
                "LEGENDARY" => 4,
                "LAIR" => 5,
                "MYTHIC" => 6,
                _ => 99
            };
        }

        private static bool HasAnyDefenseOrSenseInfo(Monster5eDto monster)
        {
            return !string.IsNullOrWhiteSpace(monster.DamageResistances) ||
                   !string.IsNullOrWhiteSpace(monster.DamageImmunities) ||
                   !string.IsNullOrWhiteSpace(monster.Senses) ||
                   !string.IsNullOrWhiteSpace(monster.Languages) ||
                   (monster.Equipments != null && monster.Equipments.Any()) ||
                   !string.IsNullOrWhiteSpace(monster.Habitats);
        }

        private static void WrapAndPrint(StringBuilder sb, string text, int indent, int maxWidth = 76)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            var indentStr = new string(' ', indent);
            var effectiveWidth = maxWidth - indent;
            var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var currentLine = new StringBuilder();

            foreach (var word in words)
            {
                if (currentLine.Length > 0 && currentLine.Length + word.Length + 1 > effectiveWidth)
                {
                    sb.AppendLine(indentStr + currentLine.ToString());
                    currentLine.Clear();
                }

                if (currentLine.Length > 0)
                    currentLine.Append(' ');
                currentLine.Append(word);
            }

            if (currentLine.Length > 0)
                sb.AppendLine(indentStr + currentLine.ToString());
        }

        private static string TruncateString(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value))
                return new string(' ', maxLength);

            if (value.Length <= maxLength)
                return value.PadRight(maxLength);

            return value.Substring(0, maxLength - 3) + "...";
        }

        /// <summary>
        /// Gets a brief stat block for quick reference
        /// </summary>
        public static string GetStatBlock(Monster5eDto monster)
        {
            if (monster == null)
                return "No monster data";

            var sb = new StringBuilder();
            sb.AppendLine($"{monster.Name} (CR {monster.ChallengeRating})");
            sb.AppendLine($"AC {monster.ArmorClass} | HP {monster.HitPoints} | Speed {FormatSpeed(monster)}");

            // Ability scores in a compact line
            sb.Append("STR ");
            AppendAbilityCompact(sb, monster.Strength);
            sb.Append(" DEX ");
            AppendAbilityCompact(sb, monster.Dexterity);
            sb.Append(" CON ");
            AppendAbilityCompact(sb, monster.Constitution);
            sb.Append(" INT ");
            AppendAbilityCompact(sb, monster.Intelligence);
            sb.Append(" WIS ");
            AppendAbilityCompact(sb, monster.Wisdom);
            sb.Append(" CHA ");
            AppendAbilityCompact(sb, monster.Charisma);
            sb.AppendLine();

            return sb.ToString();
        }

        private static void AppendAbilityCompact(StringBuilder sb, int? score)
        {
            if (!score.HasValue)
            {
                sb.Append("--");
            }
            else
            {
                var modifier = (score.Value - 10) / 2;
                sb.Append($"{score}({FormatModifier(modifier)})");
            }
        }

        /// <summary>
        /// Exports monster as markdown
        /// </summary>
        public static string ExportAsMarkdown(Monster5eDto monster)
        {
            if (monster == null)
                return "No monster to export.";

            var sb = new StringBuilder();

            // Header
            sb.AppendLine($"# {monster.Name}");
            sb.AppendLine($"*{monster.CreatureSize} {monster.CreatureType ?? "creature"}, {monster.Alignment ?? "unaligned"}*");
            sb.AppendLine();

            // Basic stats
            sb.AppendLine("---");
            sb.AppendLine($"**Armor Class** {monster.ArmorClass}");
            sb.AppendLine($"**Hit Points** {monster.HitPoints} ({monster.HitDice})");
            sb.AppendLine($"**Speed** {FormatSpeed(monster)}");
            sb.AppendLine();

            // Abilities table
            sb.AppendLine("|STR|DEX|CON|INT|WIS|CHA|");
            sb.AppendLine("|:---:|:---:|:---:|:---:|:---:|:---:|");
            sb.AppendLine($"|{FormatAbilityForTable(monster.Strength)}|{FormatAbilityForTable(monster.Dexterity)}|" +
                         $"{FormatAbilityForTable(monster.Constitution)}|{FormatAbilityForTable(monster.Intelligence)}|" +
                         $"{FormatAbilityForTable(monster.Wisdom)}|{FormatAbilityForTable(monster.Charisma)}|");
            sb.AppendLine();

            // Saving throws
            var saves = new List<string>();
            if (monster.StrSavingThrow.HasValue) saves.Add($"Str {FormatModifier(monster.StrSavingThrow.Value)}");
            if (monster.DexSavingThrow.HasValue) saves.Add($"Dex {FormatModifier(monster.DexSavingThrow.Value)}");
            if (monster.ConSavingThrow.HasValue) saves.Add($"Con {FormatModifier(monster.ConSavingThrow.Value)}");
            if (monster.IntSavingThrow.HasValue) saves.Add($"Int {FormatModifier(monster.IntSavingThrow.Value)}");
            if (monster.WisSavingThrow.HasValue) saves.Add($"Wis {FormatModifier(monster.WisSavingThrow.Value)}");
            if (monster.ChaSavingThrow.HasValue) saves.Add($"Cha {FormatModifier(monster.ChaSavingThrow.Value)}");

            if (saves.Any())
                sb.AppendLine($"**Saving Throws** {string.Join(", ", saves)}  ");

            // Skills
            if (monster.Skills != null && monster.Skills.Any())
            {
                var skills = monster.Skills.Select(s => $"{s.Key} {FormatModifier(Convert.ToInt32(s.Value))}");
                sb.AppendLine($"**Skills** {string.Join(", ", skills)}  ");
            }

            // Defenses
            if (!string.IsNullOrWhiteSpace(monster.DamageResistances))
                sb.AppendLine($"**Damage Resistances** {monster.DamageResistances}  ");
            if (!string.IsNullOrWhiteSpace(monster.DamageImmunities))
                sb.AppendLine($"**Damage Immunities** {monster.DamageImmunities}  ");

            // Senses and Languages
            if (!string.IsNullOrWhiteSpace(monster.Senses))
                sb.AppendLine($"**Senses** {monster.Senses}  ");
            if (!string.IsNullOrWhiteSpace(monster.Languages))
                sb.AppendLine($"**Languages** {monster.Languages}  ");

            // Challenge
            sb.AppendLine($"**Challenge** {monster.ChallengeRating} ({monster.Xp:N0} XP)");
            if (monster.ProficiencyBonus.HasValue)
                sb.AppendLine($"**Proficiency Bonus** {FormatModifier(monster.ProficiencyBonus.Value)}");
            sb.AppendLine();

            // Traits
            if (monster.Traits != null && monster.Traits.Any())
            {
                sb.AppendLine("## Traits");
                foreach (var trait in monster.Traits)
                {
                    sb.AppendLine($"**{trait.Title}{(trait.IsOptional ? " (Optional)" : "")}**. {trait.Description}");
                    sb.AppendLine();
                }
            }

            // Actions by type
            if (monster.Actions != null && monster.Actions.Any())
            {
                var actionGroups = monster.Actions.GroupBy(a => a.Type).OrderBy(g => GetActionTypeOrder(g.Key));
                foreach (var group in actionGroups)
                {
                    sb.AppendLine($"## {FormatActionTypeTitle(group.Key)}");
                    foreach (var action in group)
                    {
                        sb.AppendLine($"**{action.Name}**. {action.Description}");
                        sb.AppendLine();
                    }
                }
            }

            // Source
            if (!string.IsNullOrWhiteSpace(monster.Source))
            {
                sb.AppendLine("---");
                sb.AppendLine($"*Source: {monster.Source}");
                if (monster.PageSource > 0)
                    sb.Append($", page {monster.PageSource}");
                sb.AppendLine("*");
            }

            return sb.ToString();
        }

        private static string FormatAbilityForTable(int? score)
        {
            if (!score.HasValue)
                return "—";

            var modifier = (score.Value - 10) / 2;
            return $"{score} ({FormatModifier(modifier)})";
        }
    }
}