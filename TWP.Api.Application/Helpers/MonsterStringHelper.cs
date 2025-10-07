using System.Text;
using TWP.Api.Core.DbEntities;

namespace TWP.Api.Core.Helpers
{
    public static class MonsterStringHelper
    {
        public static string ToFullString(this Monster5eDbEntity monster)
        {
            if (monster == null)
                return "No monster data available.";

            var sb = new StringBuilder();

            // Basic Information
            sb.AppendLine("=== MONSTER INFORMATION ===");
            sb.AppendLine($"ID: {monster.Id}");
            sb.AppendLine($"Name: {monster.Name}");
            sb.AppendLine($"Alignment: {monster.Alignment}");
            sb.AppendLine($"Challenge Rating: {monster.ChallengeRating} (CR: {monster.Cr})");
            if (monster.CrInLair.HasValue)
                sb.AppendLine($"CR in Lair: {monster.CrInLair}");
            sb.AppendLine($"XP: {monster.Xp}");
            sb.AppendLine($"Initiative Bonus: {monster.InitiativeBonus}");
            sb.AppendLine($"Role: {monster.Role}");
            sb.AppendLine($"Size: {monster.CreatureSize}");

            // Defense
            sb.AppendLine("\n--- DEFENSE ---");
            sb.AppendLine($"Armor Class: {monster.ArmorClass}");
            if (monster.MinionArmorClass.HasValue)
                sb.AppendLine($"Minion Armor Class: {monster.MinionArmorClass}");
            sb.AppendLine($"Hit Points: {monster.HitPoints}");
            sb.AppendLine($"Hit Dice: {monster.HitDice}");

            // Movement
            sb.AppendLine("\n--- MOVEMENT ---");
            if (!string.IsNullOrEmpty(monster.Speed))
                sb.AppendLine($"Speed: {monster.Speed}");
            if (!string.IsNullOrEmpty(monster.Climb))
                sb.AppendLine($"Climb: {monster.Climb}");
            if (!string.IsNullOrEmpty(monster.Swim))
                sb.AppendLine($"Swim: {monster.Swim}");
            if (!string.IsNullOrEmpty(monster.Fly))
                sb.AppendLine($"Fly: {monster.Fly}");

            // Abilities
            sb.AppendLine("\n--- ABILITIES ---");
            if (monster.Strength.HasValue)
                sb.AppendLine($"Strength: {monster.Strength}");
            if (monster.Dexterity.HasValue)
                sb.AppendLine($"Dexterity: {monster.Dexterity}");
            if (monster.Constitution.HasValue)
                sb.AppendLine($"Constitution: {monster.Constitution}");
            if (monster.Intelligence.HasValue)
                sb.AppendLine($"Intelligence: {monster.Intelligence}");
            if (monster.Wisdom.HasValue)
                sb.AppendLine($"Wisdom: {monster.Wisdom}");
            if (monster.Charisma.HasValue)
                sb.AppendLine($"Charisma: {monster.Charisma}");

            // Saving Throws
            sb.AppendLine("\n--- SAVING THROWS ---");
            if (monster.StrSavingThrow.HasValue)
                sb.AppendLine($"Strength Save: {monster.StrSavingThrow}");
            if (monster.DexSavingThrow.HasValue)
                sb.AppendLine($"Dexterity Save: {monster.DexSavingThrow}");
            if (monster.ConSavingThrow.HasValue)
                sb.AppendLine($"Constitution Save: {monster.ConSavingThrow}");
            if (monster.IntSavingThrow.HasValue)
                sb.AppendLine($"Intelligence Save: {monster.IntSavingThrow}");
            if (monster.WisSavingThrow.HasValue)
                sb.AppendLine($"Wisdom Save: {monster.WisSavingThrow}");
            if (monster.ChaSavingThrow.HasValue)
                sb.AppendLine($"Charisma Save: {monster.ChaSavingThrow}");
            if (monster.ProficiencyBonus.HasValue)
                sb.AppendLine($"Proficiency Bonus: {monster.ProficiencyBonus}");

            // Skills & Immunities
            sb.AppendLine("\n--- SKILLS & RESISTANCES ---");
            if (!string.IsNullOrEmpty(monster.Skills))
            {
                sb.AppendLine($"Skills: {monster.Skills}");
                var skillsDict = monster.GetSkills();
                if (skillsDict != null)
                {
                    foreach (var skill in skillsDict)
                        sb.AppendLine($"  - {skill.Key}: {skill.Value}");
                }
            }
            if (!string.IsNullOrEmpty(monster.DamageImmunities))
                sb.AppendLine($"Damage Immunities: {monster.DamageImmunities}");
            if (!string.IsNullOrEmpty(monster.DamageResistances))
                sb.AppendLine($"Damage Resistances: {monster.DamageResistances}");
            if (!string.IsNullOrEmpty(monster.Senses))
                sb.AppendLine($"Senses: {monster.Senses}");
            if (!string.IsNullOrEmpty(monster.Languages))
                sb.AppendLine($"Languages: {monster.Languages}");

            // Equipment & Habitat
            sb.AppendLine("\n--- EQUIPMENT & HABITAT ---");
            if (!string.IsNullOrEmpty(monster.Equipments))
            {
                sb.AppendLine($"Equipment (raw): {monster.Equipments}");
                var equipmentList = monster.GetEquipments();
                if (equipmentList != null)
                {
                    foreach (var equipment in equipmentList)
                        sb.AppendLine($"  - {equipment}");
                }
            }
            if (!string.IsNullOrEmpty(monster.Habitats))
                sb.AppendLine($"Habitats: {monster.Habitats}");
            if (!string.IsNullOrEmpty(monster.CreatureType))
                sb.AppendLine($"Creature Type: {monster.CreatureType}");
            if (!string.IsNullOrEmpty(monster.CreatureSubType))
                sb.AppendLine($"Creature SubType: {monster.CreatureSubType}");
            if (!string.IsNullOrEmpty(monster.MonsterGroup))
                sb.AppendLine($"Monster Group: {monster.MonsterGroup}");

            // Lore
            sb.AppendLine("\n--- LORE ---");
            if (!string.IsNullOrEmpty(monster.Manner))
                sb.AppendLine($"Manner: {monster.Manner}");
            if (!string.IsNullOrEmpty(monster.Lore))
                sb.AppendLine($"Lore: {monster.Lore}");
            sb.AppendLine($"Page Source: {monster.PageSource}");
            sb.AppendLine($"Source: {monster.Source}");

            // Actions
            if (monster.Actions != null && monster.Actions.Any())
            {
                sb.AppendLine("\n=== ACTIONS ===");
                foreach (var action in monster.Actions)
                {
                    sb.AppendLine($"\n-- Action: {action.Name} --");
                    sb.AppendLine($"  Type: {action.Type}");
                    sb.AppendLine($"  Attack Type: {action.AttackType}");
                    sb.AppendLine($"  Description: {action.Description}");
                    if (!string.IsNullOrEmpty(action.ShortRange))
                        sb.AppendLine($"  Short Range: {action.ShortRange}");
                    if (!string.IsNullOrEmpty(action.LongRange))
                        sb.AppendLine($"  Long Range: {action.LongRange}");
                    if (action.AttackBonus.HasValue)
                        sb.AppendLine($"  Attack Bonus: {action.AttackBonus}");
                    if (action.DamageBonus.HasValue)
                        sb.AppendLine($"  Damage Bonus: {action.DamageBonus}");
                    if (action.DamageDice.HasValue)
                        sb.AppendLine($"  Damage Dice: {action.NumberDamageDice}d{action.DamageDice}");
                    if (action.DamageType.HasValue)
                        sb.AppendLine($"  Damage Type: {action.DamageType}");
                    if (action.LimitPerDay.HasValue)
                        sb.AppendLine($"  Limit Per Day: {action.LimitPerDay}");
                    if (action.IsProhibitedForMinion)
                        sb.AppendLine($"  Prohibited for Minion: Yes");
                    if (!string.IsNullOrEmpty(action.actionTrigger))
                        sb.AppendLine($"  Trigger: {action.actionTrigger}");
                    if (!string.IsNullOrEmpty(action.advantageCondition))
                        sb.AppendLine($"  Advantage Condition: {action.advantageCondition}");
                    if (!string.IsNullOrEmpty(action.disadvantageCondition))
                        sb.AppendLine($"  Disadvantage Condition: {action.disadvantageCondition}");
                }
            }

            // Traits
            if (monster.Traits != null && monster.Traits.Any())
            {
                sb.AppendLine("\n=== TRAITS ===");
                foreach (var trait in monster.Traits)
                {
                    sb.AppendLine($"\n-- Trait: {trait.Title} --");
                    sb.AppendLine($"  Description: {trait.Description}");
                    if (trait.AttackBonus.HasValue)
                        sb.AppendLine($"  Attack Bonus: {trait.AttackBonus}");
                    if (trait.DamageBonus.HasValue)
                        sb.AppendLine($"  Damage Bonus: {trait.DamageBonus}");
                    if (trait.DamageDice.HasValue)
                        sb.AppendLine($"  Damage Dice: {trait.NumberDamageDice}d{trait.DamageDice}");
                    if (trait.DamageType.HasValue)
                        sb.AppendLine($"  Damage Type: {trait.DamageType}");
                    if (!string.IsNullOrEmpty(trait.traitTrigger))
                        sb.AppendLine($"  Trigger: {trait.traitTrigger}");
                    if (!string.IsNullOrEmpty(trait.advantageCondition))
                        sb.AppendLine($"  Advantage Condition: {trait.advantageCondition}");
                    if (!string.IsNullOrEmpty(trait.disadvantageCondition))
                        sb.AppendLine($"  Disadvantage Condition: {trait.disadvantageCondition}");
                    if (trait.IsOptional)
                        sb.AppendLine($"  Optional: Yes");
                }
            }

            // Symbarum5e
            if (monster.Symbarum5e != null)
            {
                sb.AppendLine("\n=== SYMBARUM 5E ===");
                sb.AppendLine($"Shadow: {monster.Symbarum5e.Shadow}");
            }

            // Metadata
            sb.AppendLine("\n=== METADATA ===");

            return sb.ToString();
        }
    }
}