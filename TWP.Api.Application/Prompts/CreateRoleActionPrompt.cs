using TWP.Api.Core.DbEntities;
using TWP.Api.Core.Enums;

namespace TWP.Api.Application.Prompts;

public static class CreateRoleActionPrompt
{
    public static string GetPrompt(Monster5eDbEntity baseMonster, CombatRoleEnum role, string originalLore, string roleDescription, ActionTypeEnum actionType, AttackTypeEnum attackTypeEnum)
            => $@"You are a D&D 5e game designer. You MUST respond with ONLY the action description text, no other formatting or explanation.

                MONSTER CONTEXT:
                - Name: {baseMonster.Name}
                - Challenge Rating: {baseMonster.ChallengeRating}
                - Combat Role: {role} ({roleDescription})
                - Lore: {originalLore}
                - Stats: STR {baseMonster.Strength}, DEX {baseMonster.Dexterity}, CON {baseMonster.Constitution}, INT {baseMonster.Intelligence}, WIS {baseMonster.Wisdom}, CHA {baseMonster.Charisma}
                - Existing Actions: {string.Join("; ", baseMonster.Actions.Select(a => a.Description))}

                TASK: Create ONE special action description that perfectly represents this monster's combat role.

                REQUIREMENTS:
                - Write a complete D&D 5e mechanical description
                - Include attack type, to-hit bonus, reach/range, and damage as appropriate
                - Include any saving throws, DCs, and conditions
                - Follow standard D&D 5e formatting conventions
                - Make the action strongly reflect the monster's combat role
                - Output ONLY the description text, nothing else
                - The Action must be of type : {actionType}
                - The action is of attack type : {attackTypeEnum} which can be None, Melee, Ranged, MeleeOrRanged. If None, the action is not an attack. If Melee, the action is a melee attack. If Ranged, the action is a ranged attack. If MeleeOrRanged, the action can be either a melee or ranged attack.

                EXAMPLE OUTPUT:
                Melee Weapon Attack: +8 to hit, reach 10 ft., one target. Hit: 14 (2d8 + 5) bludgeoning damage plus 9 (2d8) thunder damage, and the target must succeed on a DC 15 Strength saving throw or be pushed 10 feet away and knocked prone.

                Now create the action description:";
}