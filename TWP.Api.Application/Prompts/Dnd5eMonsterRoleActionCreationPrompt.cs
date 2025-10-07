using TWP.Api.Application.Helpers;
using TWP.Api.Core.DbEntities;
using TWP.Api.Core.Enums;

namespace TWP.Api.Application.Prompts
{
    public static class Dnd5eMonsterRoleActionCreationPrompt
    {
        /// <summary>
        /// This prompt create a new Monster Action representing its role in combat. 
        /// </summary>
        /// <param name="baseMonster">Monster</param>
        /// <param name="monsterBuildingGuidelineDbEntity">Guidelines</param>
        /// <param name="role">Combat role</param>
        /// <param name="originalLore">New monster's lore</param>
        /// <param name="roleDescription">New monster's role description</param>
        /// <param name="actionType">Action Type of the Action to be created</param>
        /// <param name="attackTypeEnum">Attack Type of the Action to be created</param>
        /// <returns></returns>
        public static string RoleActionCreationPrompt(this Monster5eDbEntity baseMonster, MonsterBuildingGuidelineDbEntity monsterBuildingGuidelineDbEntity, CombatRoleEnum role, string originalLore, string roleDescription, ActionTypeEnum actionType, AttackTypeEnum attackTypeEnum)
        {
            var roleSpecificGuidelines = GetRoleSpecificActionGuidelines(role, baseMonster.Cr);

            var actions = new List<string>();
            foreach (var action in baseMonster.Actions)
                actions.Add($"{action.Name} : damage output = {action.NumberDamageDice}{action.DamageDice}+{action.DamageBonus} and damage type {action.DamageType}");

            return $@"You are a D&D 5e game designer. You MUST respond with ONLY the action description text, no other formatting or explanation.
                    MONSTER CONTEXT:
                    - Name: {baseMonster.Name}
                    - Challenge Rating: {baseMonster.ChallengeRating}
                    - Combat Role: {role} ({roleDescription})
                    - Lore: {originalLore}
                    - Stats: STR {baseMonster.Strength}, DEX {baseMonster.Dexterity}, CON {baseMonster.Constitution}, INT {baseMonster.Intelligence}, WIS {baseMonster.Wisdom}, CHA {baseMonster.Charisma}
                    - Proficiency Bonus: {baseMonster.ProficiencyBonus}
                    - Existing Actions: {string.Join(" | ", baseMonster.Actions.Select(a => a.Description))}

                    TASK: Create ONE special action that PERFECTLY embodies the {role} combat role and respecting the guidelines for the monster of CR {baseMonster.ChallengeRating} below :
                    Usual Attack Bonus :{monsterBuildingGuidelineDbEntity.AttackBonus}
                    Usual Damage per round :{monsterBuildingGuidelineDbEntity.DamagePerRound} 
                    Usual Multi-attack count :{monsterBuildingGuidelineDbEntity.MultiAttackCount}
                    Usual Save DC :{monsterBuildingGuidelineDbEntity.SaveDC}
                    Total Damage Average :{monsterBuildingGuidelineDbEntity.TotalDamageAvg}

                    Keep in mind that the monster alredy does these actions and already deal damages : 
                    {string.Join('/', actions)} 
                    So adapt the dame output of the special action that PERFECTLY embodies the {role} combat role to not go above the usual Damage per round :{monsterBuildingGuidelineDbEntity.DamagePerRound} 

                    ROLE-SPECIFIC REQUIREMENTS FOR {role.ToString().ToUpper()}:
                    {roleSpecificGuidelines}

                    GENERAL REQUIREMENTS:
                    - Action Type: {actionType} (Movement/Action/Bonus/Reaction/Legendary/Lair)
                    - Attack Type: {attackTypeEnum} (None/Melee/Ranged/MeleeOrRanged)
                    - Include proper to-hit bonuses, damage dice, and save DCs based on CR {baseMonster.ChallengeRating}
                    - Follow EXACT D&D 5e formatting conventions
                    - Output ONLY the mechanical description, nothing else

                    Now create the action description:";
        }


        private static string GetRoleSpecificActionGuidelines(CombatRoleEnum role, float cr)
        {
            var monster5eRoleAdapterHelpers = new Monster5eRoleAdapterHelpers();
            var baseStats = monster5eRoleAdapterHelpers.GetStatsForCR(cr);
            var minionDamage = monster5eRoleAdapterHelpers.GetMinionStatistics().First(ms => ms.Cr == cr).Damage;

            return role switch
            {
                CombatRoleEnum.Brute => $@"
                    BRUTE ACTION GUIDELINES:
                    - MUST deal high damage: {(int)(baseStats.DmgPerRound * 1.5)} damage or more
                    - Include knockback, stun, or prone effects (DC {baseStats.SaveDC + 1} STR save)
                    - If melee: Add cleave effect hitting multiple targets OR extra damage on a charge
                    - If reaction: Retaliation damage when hit
                    - Example effects: Slam (knock prone), Devastating Charge (+2d6 damage if moved 20ft), Cleaving Swing (hit all within 5ft)
                    - Emphasize raw power over finesse",

                CombatRoleEnum.Soldier => $@"
                    SOLDIER ACTION GUIDELINES:
                    - Focus on defense and protecting allies
                    - Include one of: Shield Wall (+2 AC to adjacent allies), Defensive Stance (disadvantage on attacks against it), Intercept (reaction to block attack on ally)
                    - If attack: Include grapple, restrain, or marking effect (target has disadvantage attacking others)
                    - Save DC: {baseStats.SaveDC} STR or DEX
                    - Example effects: Shield Bash (push 10ft + prone), Defensive Strike (attack + AC bonus), Guardian's Mark (disadvantage if target attacks others)",

                CombatRoleEnum.Controller => $@"
                    CONTROLLER ACTION GUIDELINES:
                    - MUST affect multiple targets or large area (15ft+ radius/cone)
                    - Include movement restriction: Restrained, Grappled, Speed reduced to 0, Difficult terrain
                    - Duration effects: Last until end of next turn minimum
                    - Save DC: {baseStats.SaveDC + 2} (use INT or WIS)
                    - Example effects: Web Spray (15ft cone, restrained), Mind Fog (20ft radius, confused), Gravity Well (pull all within 20ft)
                    - Prioritize battlefield manipulation over damage",

                CombatRoleEnum.Skirmisher => $@"
                    SKIRMISHER ACTION GUIDELINES:
                    - MUST include movement without opportunity attacks
                    - Hit-and-run mechanics: Attack + disengage, or attack + move half speed
                    - If bonus action: Dash or Disengage with added benefit
                    - If reaction: Counter-movement when enemy approaches
                    - Damage: Moderate ({baseStats.DmgPerRound})
                    - Example effects: Nimble Strike (attack + move 15ft no OA), Evasive Maneuvers (bonus action dash + AC bonus), Skirmishing Attack (move before and after attack)",

                CombatRoleEnum.Ambusher => $@"
                    AMBUSHER ACTION GUIDELINES:
                    - MUST have advantage or auto-crit against surprised/unaware targets
                    - First round bonus: Extra {(int)(baseStats.DmgPerRound * 2)} damage
                    - Include stealth synergy: Become hidden, invisible, or teleport
                    - If reaction: Trigger on specific condition for surprise attack
                    - Save DC: {baseStats.SaveDC + 1} DEX
                    - Example effects: Assassinate (auto-crit if target hasn't acted), Vanishing Strike (attack + turn invisible), Ambush Surge (double damage if hidden)",

                CombatRoleEnum.Artillery => $@"
                    ARTILLERY ACTION GUIDELINES:
                    - Range MUST be 60ft minimum, preferably 120ft+
                    - Include area effect: 10ft radius explosion, line 60ft x 5ft, or multi-target
                    - Damage: High ({(int)(baseStats.DmgPerRound * 1.3)}) but requires setup or has recharge
                    - If bonus action: Aim for advantage or extra damage next turn
                    - Save DC: {baseStats.SaveDC} DEX
                    - Example effects: Explosive Shot (20ft radius, half on save), Volley (hit all in 10ft radius), Charged Blast (recharge 5-6, triple damage)",

                CombatRoleEnum.Minion => $@"
                    MINION ACTION GUIDELINES:
                    - Simple, single-target attack
                    - Low damage: {minionDamage} fixed damage (no roll)
                    - Include pack tactics: Advantage if ally within 5ft of target
                    - Group synergy: Extra effect if 3+ minions attack same target
                    - No complex mechanics or saves
                    - Example effects: Swarm Attack (advantage with allies), Mob Rush (+1 damage per adjacent minion), Overwhelm (target speed -10ft if hit by 3+ minions)",

                CombatRoleEnum.Solo => $@"
                    SOLO ACTION GUIDELINES:
                    - MUST be legendary action OR have multiple effects in one action
                    - Affect entire battlefield: All enemies within 30ft+ OR multiple attacks
                    - Include one: Frightening Presence (WIS DC {baseStats.SaveDC + 2}), Area damage, Multi-attack with different effects
                    - Damage: Very high ({(int)(baseStats.DmgPerRound * 2)})
                    - Recharge mechanic (5-6) for devastating abilities
                    - Example effects: Legendary Multiattack (3 different attacks), Terrifying Roar (30ft frightened + damage), Storm of Blows (attack all within reach)",

                CombatRoleEnum.Support => $@"
                    SUPPORT ACTION GUIDELINES:
                    - MUST benefit allies, not harm enemies (or minimal damage)
                    - Healing: {Math.Max(baseStats.DmgPerRound / 2, 10)} HP to one or {Math.Max(baseStats.DmgPerRound / 4, 5)} to multiple
                    - Include one: Remove condition, Grant temporary HP, Give advantage, Bonus to saves/AC
                    - Range: 30ft minimum for ally effects
                    - Duration: Until start of next turn minimum
                    - Example effects: Healing Word (bonus action, 30ft), Inspiring Presence (all allies advantage on next attack), Protective Ward (ally gets +{baseStats.ProfBonus} AC)",

                CombatRoleEnum.Leader => $@"
                    LEADER ACTION GUIDELINES:
                    - MUST grant actions or benefits to allies
                    - Command effects: Allow ally to attack, move without OA, or take reaction
                    - Tactical benefits: Reposition allies, grant advantage, bonus to attacks (+{baseStats.ProfBonus})
                    - Area: Affects all allies within 30ft who can hear/see
                    - Save bonuses: Allies get +{baseStats.ProfBonus} to saves
                    - Example effects: Battle Command (2 allies make immediate attack), Tactical Genius (all allies can shift 10ft), Rallying Cry (allies gain temp HP + save bonus)",

                _ => "Create an action appropriate for this monster's CR and stats."
            };
        }

    }
}
