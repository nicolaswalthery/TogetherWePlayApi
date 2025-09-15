using Common.Extensions;
using Common.Randomizer;
using Common.ResultPattern;
using System.Text.Json;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Application.DataTransferObjects;
using TWP.Api.Application.Helpers;
using TWP.Api.Application.Helpers.Mappers;
using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Core.DbEntities;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.Helpers;
using TWP.Api.Infrastructure.Interops.Interfaces;
using TWP.Api.Infrastructure.Repository.Interfaces;

namespace TWP.Api.Application.BusinessLayers
{
    public class Dnd5eEncounterBusinessLayer : IDndEncounterBusinessLayer
    {
        private readonly IMonster5eRepository _monster5eRepository;
        private readonly IOpenAiInterops _openAiInterops;
        private readonly IMonsterBuildingGuidelineRepository _monsterBuildingGuidelineRepository;

        public Dnd5eEncounterBusinessLayer(IMonster5eRepository monster5eRepository, IOpenAiInterops openAiInterops, IMonsterBuildingGuidelineRepository monsterBuildingGuidelineRepository)
        {
            _monster5eRepository = monster5eRepository;
            _openAiInterops = openAiInterops;
            _monsterBuildingGuidelineRepository = monsterBuildingGuidelineRepository;
        }

        public async Task<Result<Dnd5eEncounterGeneratedDto>> EncounterRandomGenerator(
            EncounterDifficultyEnum encounterDifficulty,
            IList<int> playerLevels,
            string encounterNarrativeContext,
            MonsterHabitatEnum monsterHabitat,
            bool generateWithEncounterTemplate = false)
                => await Safe.ExecuteAsync(async () =>
                {
                    if (playerLevels.HasNoElement())
                        return Result<Dnd5eEncounterGeneratedDto>.Failure("No Elements", ReasonType.BadParameter);
                    var cr = playerLevels.Min();
                    var expEncounterBudget = ComputeExpBudget(encounterDifficulty, playerLevels);

                    var monsterDbEntities = await _monster5eRepository.FindByCrOrLessAsync(cr + 1);
                    if (monsterDbEntities.Data.HasNoElement())
                        return Result<Dnd5eEncounterGeneratedDto>.Failure("No Monsters found for the given CR or less", ReasonType.NotFound);

                    var monsters = monsterDbEntities.Data.Select(m => m.ToDto()).ToList();
                    var filtered = monsters.Where(m => m.Habitats != null && m.Habitats.Contains(monsterHabitat.ToString())).ToList();

                    var creatureType = new RandomSelector<Monster5eDto>().SelectOneRandomly(filtered.ToArray()).CreatureType;
                    var pickedMonsters = filtered.Where(m => m.CreatureType == creatureType).ToList();

                    (List<Monster5eDto> encounterMonsters, int expRemainingBudget, string templateUsed) encounterGenerated;
                    if (generateWithEncounterTemplate)
                        encounterGenerated = await GenerateEncounterWithTemplate(pickedMonsters!, expEncounterBudget, playerLevels.Count, playerLevels.Min(), encounterDifficulty);
                    else
                    {
                        encounterGenerated = GenerateEncounter(pickedMonsters!, expEncounterBudget, playerLevels.Count, playerLevels.Min());
                        if (encounterGenerated.expRemainingBudget != 0)
                        {
                            var originalMonsterBasedOnOneOfTheGeneratedencounter = await ReskinDndMonster(encounterGenerated.expRemainingBudget, encounterGenerated.encounterMonsters.FirstOrDefault().Name);
                            encounterGenerated.encounterMonsters.Add(originalMonsterBasedOnOneOfTheGeneratedencounter.Data);
                        }
                    }

                    var formattedEncounterData = EncounterFormatter.GetFormattedEncounterSafe(encounterDifficulty: encounterDifficulty, playerLevels: playerLevels, encounterNarrativeContext, monsterHabitat: monsterHabitat, pickedMonsters: encounterGenerated.encounterMonsters, expEncounterBudget: expEncounterBudget);

                    var openAiresult = await _openAiInterops.GetChatGptResponseAsync(
                        message: $"Create an Dnd5e Encounter using these data : Encounter data for the Master Game Master to use : {formattedEncounterData.Data} which were picked according to Difficulty : {encounterDifficulty.ToString()}, Number of players : {playerLevels.Count}, Party Level : {playerLevels.Min()}, NarrativeContext : {encounterNarrativeContext}, Monster Habitats : {monsterHabitat}",
                        systemPrompt: $"You are a Master Game Master (GM) tasked with creating a complete D&D 5e encounter. The encounter's difficulty, monster selection, habitat, and challenge rating ({cr}) have been given. Your role is to generate the following:\r\n\r\nEncounter Description:\r\nCreate an engaging narrative (narrative context : {encounterNarrativeContext}) that introduces the encounter. This should include:\r\n\r\nA description of the environment where the encounter takes place.\r\n\r\nEnsuring the monsters are integrated into the narrative which are those : Encounter data for the Master Game Master to use : {formattedEncounterData.Data}.\r\n\r\nAny immediate effects or environmental hazards the players should be aware of (e.g., traps, difficult terrain, weather conditions).\r\n\r\nMonster Stats & Tactics:\r\nProvide the stats for the Mud Mephit, including:\r\n\r\nHit Points (HP), Armor Class (AC), Damage output, and relevant abilities.\r\n\r\nSpecial abilities and spells the the monster that are given to you can use.\r\n\r\nTactical advice for the GM (you are writing for a \"noob GM,\" so make sure it is simple and clear). This should include:\r\n\r\nHow the the monster that are given to you behaves in combat.\r\n\r\nIdeal strategies or tactics the the monster that are given to you would employ.\r\n\r\nWeaknesses or vulnerabilities that the players can exploit.\r\n\r\nTreasure:\r\nBased on the CR and level of the encounter, generate treasure that fits with the encounter. This should include:\r\n\r\nGold or valuable items, ensuring the treasure is balanced for the party’s level.\r\n\r\nMagic items that are thematically appropriate for the the monster that are given to you and the habitat."
                        );
                    var finaleResult = new Dnd5eEncounterGeneratedDto() 
                                        { 
                                            Cr = cr, EncounterDifficulty = encounterDifficulty, 
                                            EncounterNarrativeContext = encounterNarrativeContext, 
                                            MonsterHabitat = monsterHabitat, 
                                            Monsters = encounterGenerated.encounterMonsters, 
                                            OpenAiResponse = openAiresult,
                                            FormattedEncounterData = formattedEncounterData.Data!
                                        };

                    return Result<Dnd5eEncounterGeneratedDto>.Success(finaleResult);
                });

        /// <summary>
        /// Reskin a Dnd Monster by changing its role, name, lore and actions using AI
        /// </summary>
        /// <param name="challengeRating">CR that the monster to reskin will have</param>
        /// <param name="experience">experience budget remaining after builder an encounter</param>
        /// <returns></returns>
        public async Task<Result<Monster5eDto>> ReskinDndMonster(int experience, string monsterName, CombatRoleEnum? combatRoleEnum = null)
            => await Safe.ExecuteAsync(async () =>
            {
                
                var monsterBuildingGuideLines = await _monsterBuildingGuidelineRepository.GetByExpAsync(experience);
                if (monsterBuildingGuideLines.Data is null)
                    return Result<Monster5eDto>.Failure("No Monster Building Guidelines found for the given experience", ReasonType.NotFound);

                if (monsterBuildingGuideLines.IsFailure)
                    return Result<Monster5eDto>.Failure("FetchingBuilding Guidelines faillure", ReasonType.Failure);

                var result = await _monster5eRepository.GetByNameAsync(monsterName);
                if (result.IsFailure)
                    return Result<Monster5eDto>.Failure("No Monsters found for the given CR", ReasonType.NotFound);
                var baseMonster = result.Data;

                CombatRoleEnum randomRole;
                if(combatRoleEnum is null)
                    do
                    {
                        randomRole = RoleDescriptionsHelper.GetOneRandomRole();
                    } while (baseMonster.Role.Value == randomRole);
                else
                    randomRole = combatRoleEnum.Value;
                var roleDescription = RoleDescriptionsHelper.GetRoleDescription(randomRole);

                var results = await _monster5eRepository.FindByCrAsync(monsterBuildingGuideLines.Data.CRNumeric);
                var monsterCarac = results.Data.Shuffle().FirstOrDefault();

                //Generate monster lore from base monster
                var originalLore = await _openAiInterops.GetChatGptResponseAsync($"Take the base lore of {baseMonster.Name} and use it to create a original lore for {baseMonster.Name} that has the role {randomRole} : {roleDescription}. Create what this {randomRole} variation has more, what set it appart !");
                var originalName = await _openAiInterops.GetChatGptResponseAsync($"Take the base name of {baseMonster.Name} and use it to create a original name for a monster that has the role {randomRole} : {roleDescription} and this lore {originalLore}");

                var originalManner = await _openAiInterops.GetChatGptResponseAsync($"Take the base lore of {baseMonster.Name} and use it to create a very short (10 to 15 words) manner description for {baseMonster.Name} that has the role {randomRole} : {roleDescription}.");

                var newMonster = new Monster5eDbEntity
                {
                    Actions = baseMonster.Actions,
                    Alignment = baseMonster.Alignment,
                    ArmorClass = monsterBuildingGuideLines.Data.ArmorClass,
                    ChallengeRating = monsterBuildingGuideLines.Data.CR,
                    Climb = baseMonster.Climb,
                    Constitution = monsterCarac.Constitution,
                    CreatureSize = baseMonster.CreatureSize,
                    CreatureSubType = baseMonster.CreatureSubType,
                    CreatureType = baseMonster.CreatureType,
                    Cr = monsterBuildingGuideLines.Data.CRNumeric,
                    Equipments = baseMonster.Equipments,
                    HitDice = monsterCarac.HitDice,
                    HitPoints = monsterBuildingGuideLines.Data.AverageHP,
                    InitiativeBonus = monsterBuildingGuideLines.Data.InitiativeBonus,
                    Intelligence = monsterCarac.Intelligence,
                    Languages = baseMonster.Languages,
                    Lore = JsonSerializer.Serialize(new { originalLore }),
                    Manner = originalManner,
                    MinionArmorClass = monsterCarac.MinionArmorClass,
                    MonsterGroup = baseMonster.MonsterGroup,
                    Name = originalName,
                    PageSource = 0,
                    ProficiencyBonus = monsterBuildingGuideLines.Data.ProficiencyBonus,
                    Role = randomRole,
                    Skills = monsterCarac.Skills,
                    Source = "AI Generated",
                    Speed = monsterCarac.Speed,
                    Strength = monsterCarac.Strength,
                    Swim = monsterCarac.Swim,
                    Traits = baseMonster.Traits,
                    Wisdom = monsterCarac.Wisdom,
                    Xp = monsterCarac.Xp,
                    Charisma = monsterCarac.Charisma,
                    DamageImmunities = baseMonster.DamageImmunities,
                    DamageResistances = baseMonster.DamageResistances,
                    Senses = baseMonster.Senses,
                    CrInLair = baseMonster.CrInLair,
                    DexSavingThrow = monsterCarac.DexSavingThrow,
                    StrSavingThrow = monsterCarac.StrSavingThrow,
                    ConSavingThrow = monsterCarac.ConSavingThrow,
                    IntSavingThrow = monsterCarac.IntSavingThrow,
                    WisSavingThrow = monsterCarac.WisSavingThrow,
                    ChaSavingThrow = monsterCarac.ChaSavingThrow,
                    Fly = baseMonster.Fly,
                    Dexterity = baseMonster.Dexterity,
                    Habitats = baseMonster.Habitats,
                    Symbarum5e = baseMonster.Symbarum5e
                };

                var monster5eRoleAdapterHelpers = new Monster5eRoleAdapterHelpers();
                var roleApplied = monster5eRoleAdapterHelpers.AdaptMonsterToRole(newMonster);

                if (randomRole != CombatRoleEnum.Minion)
                {
                    //Create Action related to the role of the monster
                    var actionType = EnumExtensions.GetRandomElementOfEnum<ActionTypeEnum>();
                    var attackType = EnumExtensions.GetRandomElementOfEnum<AttackTypeEnum>();

                    string roleBasedActions = String.Empty;
                    if (randomRole != CombatRoleEnum.Minion)
                        roleBasedActions = await _openAiInterops.ChatGptResponseAsync(CreateRoleActionPrompt(baseMonster, randomRole, originalLore, roleDescription, actionType, attackType));

                    var newAction = new ActionDto
                    {
                        Id = Guid.NewGuid(),
                        MonsterId = roleApplied.modifiedMonster.Id,
                        Name = $"{randomRole} Special Attack",
                        Type = actionType.ToString(),
                        AttackType = attackType.ToString(),
                        Description = roleBasedActions,
                        IsProhibitedForMinion = false
                    };

                    roleApplied.modifiedMonster.Actions.Add(newAction.ToDbEntity());
                }
                else //Minion cannot have actions
                    newMonster.Actions = null;

                await _monster5eRepository.Insert(newMonster);

                return Result<Monster5eDto>.Success(newMonster.ToDto());
            });

        /// <summary>
        /// Calculate the ideal CR for a given role based on party level and difficulty
        /// </summary>
        private float CalculateIdealCrForRole(CombatRoleEnum role, int partyLevel, EncounterDifficultyEnum difficulty)
        {
            var baseCr = (float)partyLevel;

            // Adjust based on difficulty
            var difficultyModifier = difficulty switch
            {
                EncounterDifficultyEnum.Low => -1f,
                EncounterDifficultyEnum.Moderate => 0f,
                EncounterDifficultyEnum.High => 1f,
                _ => 0f
            };

            // Adjust based on role
            var roleModifier = role switch
            {
                CombatRoleEnum.Solo => difficultyModifier + 2f,
                CombatRoleEnum.Brute => difficultyModifier + 0.5f,
                CombatRoleEnum.Leader => difficultyModifier + 0.5f,
                CombatRoleEnum.Artillery => difficultyModifier - 0.5f,
                CombatRoleEnum.Controller => difficultyModifier,
                CombatRoleEnum.Soldier => difficultyModifier,
                CombatRoleEnum.Support => difficultyModifier - 1f,
                CombatRoleEnum.Skirmisher => difficultyModifier - 0.5f,
                CombatRoleEnum.Ambusher => difficultyModifier,
                CombatRoleEnum.Minion => -2f,
                _ => difficultyModifier
            };

            return Math.Max(0.125f, baseCr + roleModifier);
        }

        /// <summary>
        /// Score how appropriate a monster is for a given role
        /// </summary>
        private int GetRoleAppropriatenessScore(Monster5eDto monster, CombatRoleEnum role, int partyLevel)
        {
            var score = 0;

            // Base score on CR difference from party level
            var crDifference = Math.Abs(monster.Cr - partyLevel);
            score += (int)(crDifference * 10);

            // Adjust based on role expectations
            switch (role)
            {
                case CombatRoleEnum.Solo:
                    // Solo monsters should be higher CR
                    if (monster.Cr > partyLevel + 1) score -= 20;
                    break;
                case CombatRoleEnum.Minion:
                    // Minions should be lower CR
                    if (monster.Cr < partyLevel - 2) score -= 20;
                    break;
                case CombatRoleEnum.Leader:
                case CombatRoleEnum.Brute:
                    // These should be at or slightly above party level
                    if (monster.Cr >= partyLevel && monster.Cr <= partyLevel + 1) score -= 15;
                    break;
                case CombatRoleEnum.Support:
                case CombatRoleEnum.Artillery:
                    // These should be slightly below party level
                    if (monster.Cr >= partyLevel - 1 && monster.Cr <= partyLevel) score -= 15;
                    break;
            }

            return score;
        }

        /// <summary>
        /// Generates an encounter using a template-based approach with predefined role compositions
        /// </summary>
        /// <param name="monsters">List of available monsters with roles assigned</param>
        /// <param name="expEncounterBudget">Total XP budget for the encounter</param>
        /// <param name="playerCount">Number of players in the party</param>
        /// <param name="partyLevel">Average level of the party</param>
        /// <param name="difficulty">Encounter difficulty</param>
        /// <param name="template">Optional specific template to use. If null, selects randomly based on difficulty</param>
        /// <returns>List of monsters for the encounter and remaining XP budget</returns>
        private async Task<(List<Monster5eDto> encounterMonsters, int expRemainingBudget, string templateUsed)> GenerateEncounterWithTemplate(
            List<Monster5eDto> monsters,
            int expEncounterBudget,
            int playerCount,
            int partyLevel,
            EncounterDifficultyEnum difficulty,
            EncounterTemplateHelpers.EncounterTemplate template = null)
        {
            // Select template if not provided
            template ??= EncounterTemplateHelpers.GetRandomTemplate(difficulty);

            var encounter = new List<Monster5eDto>();
            var remainingBudget = expEncounterBudget;

            // Apply template multiplier to budget (templates with synergies are harder)
            var templateMultiplier = EncounterTemplateHelpers.GetCRBudgetMultiplier(template);
            var adjustedBudget = (int)(expEncounterBudget / templateMultiplier);
            remainingBudget = adjustedBudget;

            // Group monsters by role
            var monstersByRole = monsters
                .Where(m => m.Xp <= adjustedBudget && m.Role != null)
                .GroupBy(m => Enum.Parse<CombatRoleEnum>(m.Role))
                .ToDictionary(g => g.Key, g => g.ToList());

            // Process each role in the template
            foreach (var roleComposition in template.Composition.OrderBy(c => c.IsRequired ? 0 : 1))
            {
                var targetCount = roleComposition.GetCountForPartySize(playerCount);

                // Check if we have monsters for this role
                if (!monstersByRole.ContainsKey(roleComposition.Role) || !monstersByRole[roleComposition.Role].Any())
                {
                    if (roleComposition.IsRequired)
                    {
                        // Reskin an existing monster to fulfill the required role
                        var reskinResult = await CreateReskinMonsterForRole(
                            roleComposition.Role,
                            monsters,
                            remainingBudget,
                            targetCount,
                            partyLevel,
                            difficulty);

                        if (reskinResult != null)
                        {
                            // Add the reskinned monster to our available monsters for this role
                            if (!monstersByRole.ContainsKey(roleComposition.Role))
                            {
                                monstersByRole[roleComposition.Role] = new List<Monster5eDto>();
                            }
                            monstersByRole[roleComposition.Role].Add(reskinResult);
                        }
                        else
                        {
                            continue; // Skip this role if reskin failed
                        }
                    }
                    else
                    {
                        continue; // Skip optional roles if not available
                    }
                }

                var availableMonstersForRole = monstersByRole[roleComposition.Role]
                    .Where(m => m.Xp <= remainingBudget)
                    .OrderBy(m => GetRoleAppropriatenessScore(m, roleComposition.Role, partyLevel))
                    .ToList();

                // Add monsters for this role
                var addedCount = 0;
                while (addedCount < targetCount && availableMonstersForRole.Any() && remainingBudget > 0)
                {
                    Monster5eDto selectedMonster = null;

                    if (roleComposition.Role == CombatRoleEnum.Solo)
                    {
                        // For solo monsters, pick the strongest that fits
                        selectedMonster = availableMonstersForRole
                            .Where(m => m.Xp <= remainingBudget * 0.7) // Solo should take most of the budget
                            .OrderByDescending(m => m.Xp)
                            .FirstOrDefault();
                    }
                    else if (roleComposition.Role == CombatRoleEnum.Minion)
                    {
                        // For minions, pick the weakest
                        selectedMonster = availableMonstersForRole
                            .Where(m => m.Xp <= remainingBudget / (targetCount - addedCount))
                            .OrderBy(m => m.Xp)
                            .FirstOrDefault();
                    }
                    else
                    {
                        // For other roles, pick based on appropriate CR for party level
                        var idealCr = CalculateIdealCrForRole(roleComposition.Role, partyLevel, difficulty);
                        selectedMonster = availableMonstersForRole
                            .Where(m => m.Xp <= remainingBudget / Math.Max(1, targetCount - addedCount))
                            .OrderBy(m => Math.Abs(m.Cr - idealCr))
                            .FirstOrDefault();
                    }

                    if (selectedMonster != null && selectedMonster.Xp <= remainingBudget)
                    {
                        encounter.Add(selectedMonster);
                        remainingBudget -= selectedMonster.Xp;
                        addedCount++;

                        // For variety, remove the selected monster from available pool if we need more
                        if (addedCount < targetCount && roleComposition.Role != CombatRoleEnum.Minion)
                        {
                            availableMonstersForRole.Remove(selectedMonster);
                        }
                    }
                    else
                    {
                        break; // Can't add more monsters of this role
                    }
                }
            }

            // If we have significant budget left and the encounter is too small, add appropriate monsters
            if (remainingBudget > expEncounterBudget * 0.3 && encounter.Count < playerCount * 2)
            {
                await FillRemainingBudgetAsync(encounter, remainingBudget, monsters, template, playerCount, partyLevel);
            }

            return (encounter, remainingBudget, template.Name);
        }

        /// <summary>
        /// Create a reskinned monster for a specific role when none are available
        /// </summary>
        private async Task<Monster5eDto> CreateReskinMonsterForRole(
            CombatRoleEnum neededRole,
            List<Monster5eDto> availableMonsters,
            int remainingBudget,
            int targetCount,
            int partyLevel,
            EncounterDifficultyEnum difficulty)
        {
            // Calculate appropriate XP for this role
            var idealCr = CalculateIdealCrForRole(neededRole, partyLevel, difficulty);
            var targetXp = remainingBudget / Math.Max(1, targetCount);

            // Special handling for Solo monsters - they should consume most of the budget
            if (neededRole == CombatRoleEnum.Solo)
            {
                targetXp = (int)(remainingBudget * 0.7);
            }
            // Special handling for Minions - they should be cheap
            else if (neededRole == CombatRoleEnum.Minion)
            {
                targetXp = Math.Min(50, remainingBudget / targetCount);
            }

            // Find the best candidate monster to reskin
            // Prefer monsters that are already in the encounter's creature type
            var candidateMonster = availableMonsters
                .Where(m => m.Xp > 0 && m.Name != null)
                .OrderBy(m => Math.Abs(m.Xp - targetXp))
                .FirstOrDefault();

            if (candidateMonster == null)
            {
                return null;
            }

            try
            {
                // Call ReskinDndMonster with the forced combat role
                var reskinResult = await ReskinDndMonster(
                    targetXp,
                    candidateMonster.Name,
                    neededRole);

                if (reskinResult.IsSuccess && reskinResult.Data != null)
                {
                    return reskinResult.Data;
                }
            }
            catch (Exception ex)
            {
                // Log the error if you have logging
                // For now, just return null to continue without the reskinned monster
            }

            return null;
        }

        /// <summary>
        /// Fill remaining budget with appropriate monsters based on template theme (async version)
        /// </summary>
        private async Task FillRemainingBudgetAsync(
            List<Monster5eDto> encounter,
            int remainingBudget,
            List<Monster5eDto> availableMonsters,
            EncounterTemplateHelpers.EncounterTemplate template,
            int playerCount,
            int partyLevel)
        {
            // Prioritize adding more of existing roles in the template
            var existingRoles = template.Composition.Where(c => c.IsRequired)
                                                    .OrderBy(c => c.Role == CombatRoleEnum.Minion ? 0 : 1) // Prefer minions for filling
                                                    .ToList();

            var currentBudget = remainingBudget;

            for (int roleIndex = 0; roleIndex < existingRoles.Count && currentBudget > 10; roleIndex++)
            {
                var roleComp = existingRoles[roleIndex];

                var candidates = availableMonsters.Where(m => m.Role == roleComp.Role.ToString() && m.Xp <= currentBudget)
                                                  .OrderBy(m => m.Xp).ToList();

                // If no candidates exist for this role, try to reskin
                if (!candidates.Any() && currentBudget > 50)
                {
                    var reskinned = await CreateReskinMonsterForRole(
                        roleComp.Role,
                        availableMonsters,
                        currentBudget,
                        1,
                        partyLevel,
                        EncounterDifficultyEnum.Moderate);

                    if (reskinned != null)
                    {
                        candidates.Add(reskinned);
                    }
                }

                for (int candidateIndex = 0;
                     candidateIndex < candidates.Count
                        && currentBudget > 10
                        && encounter.Count < playerCount * 3;
                     candidateIndex++)
                {
                    var candidate = candidates[candidateIndex];

                    if (candidate.Xp <= currentBudget && encounter.Count < playerCount * 3)
                    {
                        encounter.Add(candidate);
                        currentBudget -= candidate.Xp;
                    }
                }
            }
        }

        private static string CreateRoleActionPrompt(Monster5eDbEntity baseMonster, CombatRoleEnum role, string originalLore, string roleDescription, ActionTypeEnum actionType, AttackTypeEnum attackTypeEnum)
        {
            // Instructions spécifiques selon le rôle
            var roleSpecificGuidelines = GetRoleSpecificActionGuidelines(role, baseMonster.Cr);

            return $@"You are a D&D 5e game designer. You MUST respond with ONLY the action description text, no other formatting or explanation.

MONSTER CONTEXT:
- Name: {baseMonster.Name}
- Challenge Rating: {baseMonster.ChallengeRating}
- Combat Role: {role} ({roleDescription})
- Lore: {originalLore}
- Stats: STR {baseMonster.Strength}, DEX {baseMonster.Dexterity}, CON {baseMonster.Constitution}, INT {baseMonster.Intelligence}, WIS {baseMonster.Wisdom}, CHA {baseMonster.Charisma}
- Proficiency Bonus: {baseMonster.ProficiencyBonus}
- Existing Actions: {string.Join("; ", baseMonster.Actions.Select(a => a.Description))}

TASK: Create ONE special action that PERFECTLY embodies the {role} combat role.

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

        /// <summary>
        /// Compute the experience budget for a party
        /// </summary>
        /// <returns></returns>
        private int ComputeExpBudget(EncounterDifficultyEnum encounterDifficulty, IList<int> playerLevels)
        {
            if(playerLevels.Any(pl => pl <= 0 || pl > 20))
                return 0;

            // XP budget table as per the image (dnd2024 version)
            var xpTable = new Dictionary<int, (int Low, int Moderate, int High)>
            {
                {1, (50, 75, 100)},
                {2, (100, 150, 200)},
                {3, (150, 225, 400)},
                {4, (250, 375, 500)},
                {5, (500, 750, 1100)},
                {6, (600, 1000, 1400)},
                {7, (750, 1300, 1700)},
                {8, (1000, 1700, 2100)},
                {9, (1300, 2000, 2600)},
                {10, (1600, 2300, 3100)},
                {11, (1900, 2900, 4100)},
                {12, (2200, 3700, 4700)},
                {13, (2600, 4200, 5400)},
                {14, (2900, 4900, 6200)},
                {15, (3300, 5400, 7800)},
                {16, (3800, 6100, 9800)},
                {17, (4500, 7200, 11700)},
                {18, (5000, 8700, 14200)},
                {19, (5500, 10700, 17200)},
                {20, (6400, 13200, 22000)}
            };

            int total = 0;
            foreach (var level in playerLevels)
            {
                var xp = encounterDifficulty switch
                {
                    EncounterDifficultyEnum.Low => xpTable[level].Low,
                    EncounterDifficultyEnum.Moderate => xpTable[level].Moderate,
                    EncounterDifficultyEnum.High => xpTable[level].High,
                    _ => throw new ArgumentException("Unsupported difficulty")
                };
                total += xp;
            }
            return total;
        }

        /// <summary>
        /// Generates a single encounter by selecting monsters whose total XP does not exceed the given budget.
        /// </summary>
        /// <param name="monsters">List of available monsters.</param>
        /// <param name="expEncounterBudget">Total XP budget for the encounter.</param>
        /// <param name="playerNumber">Number of players in the party.</param>
        /// <param name="partyLevel">Level of the party.</param>
        /// <returns>List of monsters for the encounter.</returns>
        private (List<Monster5eDto> encounterMonsters, int expRemainingBudget, string templateUsed) GenerateEncounter(List<Monster5eDto> monsters, int expEncounterBudget, int playerNumber, int partyLevel)
        {
            var encounter = new List<Monster5eDto>();
            var remainingBudget = expEncounterBudget;
            var maxMonsters = playerNumber * 2;
            var maxDifferentMonster = 4;

            // Group monsters by Challenge Rating (CR), filtering monsters whose XP is within the budget.
            var selectedAvailableMonsters = monsters.Where(m => m.Xp <= expEncounterBudget)
                                        .OrderBy(_ => Guid.NewGuid()) // Shuffle types
                                        .ToList();
            bool breakLoop = false;
            for (var i = 0; i <= 100000 && !breakLoop && remainingBudget > 9 /* 10 is the minimum xp budget available for a dnd monster */ && encounter.Count < maxMonsters; i++)
            {
                var monster = selectedAvailableMonsters[i];

                // Add monster if it fits within the remaining budget
                if (monster.Xp <= remainingBudget && encounter.Count < maxMonsters)
                {
                    encounter.Add(monster);
                    remainingBudget -= monster.Xp;
                }
                else
                {
                    // If remaining budget allows, try to add a monster with the highest XP value fitting the remaining budget.
                    var lastMonster = selectedAvailableMonsters.Where(m => m.Xp <= remainingBudget && !encounter.Contains(m))
                                                               .OrderByDescending(m => m.Xp)
                                                               .FirstOrDefault();
                    if (lastMonster != null)
                    {
                        encounter.Add(lastMonster);
                        remainingBudget -= lastMonster.Xp;
                    }
                    else
                        breakLoop = true; // Exit if no suitable monster can be added
                }

                // Ensure that we include only one monster of the partyLevel + 1 CR in the encounter
                if (IsCrPlusOneAlreadyIncludedInEncounter(encounter, partyLevel))
                    selectedAvailableMonsters = selectedAvailableMonsters.Where(m => m.Cr <= (float)partyLevel).ToList();
            }

            return (encounter, remainingBudget, "None");
        }

        /// <summary>
        /// Checks if a monster with CR +1 has already been included in the encounter.
        /// </summary>
        /// <param name="encounter">List of monsters in the current encounter.</param>
        /// <param name="partyLevel">The level of the party.</param>
        /// <returns>True if a monster with CR + 1 is already in the encounter.</returns>
        private bool IsCrPlusOneAlreadyIncludedInEncounter(List<Monster5eDto> encounter, int partyLevel)
        {
            return encounter.Any(m => m.Cr == partyLevel + 1);
        }


        /// <summary>
        /// Creates a sci-fi themed adversary from a narrative description
        /// </summary>
        /// <param name="narrativeDescription">The narrative/descriptive text about the sci-fi adversary</param>
        /// <param name="forcedCombatRole">Optional: Force a specific combat role, otherwise it will be selected based on narrative</param>
        /// <returns>A list of sci-fi adversaries, one for each combat role</returns>
        public async Task<Result<List<Monster5eDto>>> CreateScifiAdversary(
            string narrativeDescription,
            CombatRoleEnum? forcedCombatRole = null)
                => await Safe.ExecuteAsync(async () =>
                {
                    if (string.IsNullOrWhiteSpace(narrativeDescription))
                        return Result<List<Monster5eDto>>.Failure("Narrative description cannot be empty", ReasonType.BadParameter);

                    // Step 1: Analyze narrative to determine appropriate CR and suggested roles
                    var analysisPrompt = $@"Analyze this sci-fi adversary description and provide a JSON response with the following structure:
            {{
                ""suggestedCR"": <number between 0.125 and 30>,
                ""threatLevel"": ""<low/medium/high/extreme>"",
                ""suggestedRoles"": [""<role1>"", ""<role2>""],
                ""keyTraits"": [""<trait1>"", ""<trait2>""],
                ""weaponType"": ""<energy/projectile/melee/mixed>"",
                ""techLevel"": <0, 1, 2, 3, 4, or 5>
            }}

            Tech Level Guidelines:
                - TL0: This level covers the entirety of civilized history until 
                        the early industrial era, stopping before the harnessing of 
                        electric power, everything from the discovery of the wheel 
                        to its use in manufacturing. 
                        Vehicles. Gliders or basic aeroforms. Both ground and 
                        aircraft are limited to archaic steam power. 
                        Weapons. All weapons rely on steam or chemical 
                        propellants with simple loading mechanisms. The blunderbuss 
                        and musket are examples. 
                        Medical. Natural healing. TL0 benefi ts more from 
                        discovered human knowledge about biology than the tools 
                        that were developed consequently. Surgery can cure most 
                        wounds, but recovery can last a while.
                         Similarity. Up to the mid-18th century.
                - TL1: 
                     At this level, machines come into their own. Internal 
                    combustion and steam power have been perfected. Electric 
                    power and road vehicles are changing the way cities are built. 
                    Vehicles. Ground vehicles are run off steam or internal 
                    combustion. Electrical power is in its infancy. The fact they 
                    are mass-produced is the real achievement. Aircraft are 
                    flown by manual controls and receive propulsion from 
                    propellers.  Weapons. Bolt action rifl es and revolvers. Cartridge
                    fed fi rearms are becoming more common. 
                    Medical. The implementation of the scientifi c method 
                    and laboratory research has resulted in vaccines. Drugs are 
                    becoming commonplace.
                     Similarity. 19th to early 20th Century.

                - TL2: 
                    At this level, almost every form of technology has integrated 
                    electronics and advanced computer control. Electrifi cation is 
                    now commonplace, though computers have yet to dominate 
                    civilization.
                     Vehicles. Ground vehicles now have electronics; some 
                    offer climate control. Aircraft now possess fl y-by-wire, 
                    vectored thrust, and vertical-take-off capacity. 
                    Weapons. Computer tracking and targeting. Infrared 
                    and thermal imaging is available, but not standard. Firearms 
                    haven’t changed but have grown more complicated with 
                    advanced reloading and higher fi ring rates. Advances in 
                    construction make them lighter with larger calibers. 
                    Medical. Computer diagnostic beds, MRIs, and X-Rays.
                     Similarity. Mid-late 20th century.
                - TL3: 
                    Refinements in the manipulation of magnetic fi elds and 
                    energy levels characterize this stage. Computers now 
                    control most of civilization and link citizens together.
                    Vehicles. Vertical take-off fan craft and wingless jets 
                    keep aircraft aloft, are much more stable, and can fl y rings 
                    around more primitive craft. Aircraft designs are no longer 
                    dominated by their massive aeroforms. Ground vehicles still 
                    use wheels, but now mass transit magnetic vehicles appear 
                    as an alternative. 
                    Weapons. There will always be bullets, but the rise 
                    of both railcannons and self-propelled projectiles offer 
                    alternatives. Laser weaponry in its infancy. Advanced 
                    magnetics. Prototype exo-armor appears. 
                    Medical. Most known diseases are curable. Healing 
                    time cut to one-third with medical attention. Nanotech 
                    healing isolated in the laboratory.
                     Similarity. Early-mid 21st century.

                - TL4: 
                    At this level, alternate energy and advanced in nuclear 
                    power has created an energy surplus. Nanotechnology is 
                    ubiquitous. Consumer space travel is now frequent.
                     Vehicles. Robots appear beyond the role of “dumb 
                    tool.” Exo-armor is mass-produced. Wheeled traffi c 
                    virtually nonexistent or, if it exists, can traverse any terrain. 
                    Ramjets shrink and provide massive thrust in small packages, 
                    revolutionizing transportation outside of magnetic-traffi c. 
                    Weapons. Laser weapons “tunable.” Plasma weaponry. 
                    Bolt weapons are outdated.
                    Medical. Nanotechnology can heal any wounds and 
                    even regenerate limbs.
                - TL5: 
                    Any sufficiently advanced technology would be indistinguishable from magic.
                     Vehicles. Common antigravity replaces all previous 
                    transportation. 
                    Weapons. Disruptors, vapor rifl es, disintegrator 
                    weaponry. 
                    Medical. Complete body reconstruction.

                    Application of tech levels
                         The tech level can affect the diffi culty and cost of crafting, 
                        repairing, and modifying technology. It can also change its 
                        rarity.
                         TL 0 and TL 1. Common. All items with no listed TL 
                        are TL0.
                        TL 2. Uncommon
                         TL 3. Rare
                         TL 4. Very Rare
                         TL 5. Legendary
                         If setting a game at a higher TL, you can shift the rarity 
                        down to make items more common. Certain items (like 
                        exo-armor) may be rarer than their listed tech level. They 
                        may also count as multiple items. Tech levels can also apply 
                        in other ways depending on the device in question. See the 
                        item descriptions for details. 

                    Description: {narrativeDescription}

                    Base your CR suggestion on:
                    - Low (CR 0.125-4): Basic troops, drones, minor threats
                    - Medium (CR 5-10): Elite units, specialists, significant threats
                    - High (CR 11-16): Commanders, heavy units, major threats
                    - Extreme (CR 17+): Legendary units, boss-level threats

                    Base CR on both threat description and tech level.
                    Valid roles: Brute, Soldier, Controller, Skirmisher, Ambusher, Artillery, Minion, Solo, Support, Leader";

                    var analysisJson = await _openAiInterops.GetChatGptResponseAsync(
                        analysisPrompt,
                        temperature: 0.3,
                        maxTokens: 500,
                        responseFormat: OpenAIResponseFormatEnum.Json
                    );

                    // Parse the analysis
                    var analysis = JsonSerializer.Deserialize<ScifiAdversaryAnalysis>(analysisJson);
                    var suggestedCr = analysis?.SuggestedCR ?? 1.0f;

                    // Step 2: Get a base monster from DB with similar CR
                    var baseMonsterResult = await _monster5eRepository.FindByCrAsync(suggestedCr);
                    if (baseMonsterResult.IsFailure || !baseMonsterResult.Data.Any())
                    {
                        // Fallback to nearest CR
                        var allMonsters = await _monster5eRepository.GetAllAsync();
                        if (allMonsters.IsFailure || !allMonsters.Data.Any())
                            return Result<List<Monster5eDto>>.Failure("No base monsters available", ReasonType.NotFound);

                        baseMonsterResult = Result<List<Monster5eDbEntity>>.Success(
                            allMonsters.Data.OrderBy(m => Math.Abs(m.Cr - suggestedCr)).Take(5).ToList()
                        );
                    }

                    // Select a random base monster
                    var baseMonster = new RandomSelector<Monster5eDbEntity>()
                        .SelectOneRandomly(baseMonsterResult.Data.ToArray());

                    // Step 3: Determine which roles to create
                    var rolesToCreate = new List<CombatRoleEnum>();
                    if (forcedCombatRole.HasValue)
                    {
                        rolesToCreate.Add(forcedCombatRole.Value);
                    }
                    else
                    {
                        // Create variants for suggested roles from analysis
                        var suggestedRoles = analysis?.SuggestedRoles ?? new List<string> { "Soldier", "Artillery" };
                        foreach (var roleStr in suggestedRoles.Take(3)) // Limit to 3 variants
                        {
                            if (Enum.TryParse<CombatRoleEnum>(roleStr, true, out var role))
                                rolesToCreate.Add(role);
                        }

                        // Ensure at least one role
                        if (!rolesToCreate.Any())
                            rolesToCreate.Add(CombatRoleEnum.Soldier);
                    }

                    // Step 4: Create sci-fi variants for each role
                    var scifiAdversaries = new List<Monster5eDto>();

                    foreach (var role in rolesToCreate)
                    {
                        var scifiVariant = await CreateSingleScifiVariant(
                            baseMonster,
                            narrativeDescription,
                            role,
                            analysis
                        );

                        if (scifiVariant.IsSuccess && scifiVariant.Data != null)
                            scifiAdversaries.Add(scifiVariant.Data);
                    }

                    if (!scifiAdversaries.Any())
                        return Result<List<Monster5eDto>>.Failure("Failed to create any sci-fi variants", ReasonType.Failure);

                    return Result<List<Monster5eDto>>.Success(scifiAdversaries);
                });

        /// <summary>
        /// Creates a single sci-fi variant for a specific combat role
        /// </summary>
        private async Task<Result<Monster5eDto>> CreateSingleScifiVariant(
            Monster5eDbEntity baseMonster,
            string narrativeDescription,
            CombatRoleEnum role,
            ScifiAdversaryAnalysis analysis)
                => await Safe.ExecuteAsync(async () =>
                {
                    var roleDescription = RoleDescriptionsHelper.GetRoleDescription(role);

                    // Step 1: Generate sci-fi lore based on narrative and role
                    var lorePrompt = $@"Create a compelling sci-fi lore for an adversary based on:
                    Original narrative: {narrativeDescription}
                    Combat role: {role} - {roleDescription}
                    Tech level: {analysis?.TechLevel ?? "standard"}
                    Weapon preference: {analysis?.WeaponType ?? "kinetic"}
            
                    Write 2-3 sentences of lore that:
                    - Explains their origin/faction/purpose
                    - Describes their technology or augmentations
                    - Hints at their combat tactics related to their {role} role
                    Keep it concise and evocative.";

                            var scifiLore = await _openAiInterops.GetChatGptResponseAsync(lorePrompt, temperature: 0.7, maxTokens: 200);

                            // Step 2: Generate sci-fi name
                            var namePrompt = $@"Create a sci-fi adversary name based on:
                            Lore: {scifiLore}
                            Role: {role}
                            Tech level: {analysis?.TechLevel ?? "standard"}
            
                            Examples of good sci-fi names:
                            - Shock Trooper (Soldier)
                            - Stealth Drone MK-7 (Ambusher)
                            - Commander (Controller)
                            - Heavy Mech Unit (Brute)
                            - Sniper Bot X-99 (Artillery)
            
                            Provide only the name, nothing else.";

                    var scifiName = await _openAiInterops.GetChatGptResponseAsync(namePrompt, temperature: 0.8, maxTokens: 50);

                    // Step 3: Generate manner/behavior description
                    var mannerPrompt = $@"Write a very short (10-15 words) behavior/manner description for:
                                        {scifiName} - a {role} with this lore: {scifiLore} Focus on how they act in combat. 
                                        Be concise and evocative.";

                    var scifiManner = await _openAiInterops.GetChatGptResponseAsync(mannerPrompt, temperature: 0.6, maxTokens: 50);

                    // Step 4: Create the new monster entity
                    var newMonster = new Monster5eDbEntity
                    {
                        // Copy base stats
                        Actions = new List<ActionDbEntity>(), // Will add sci-fi actions later
                        Alignment = AlignmentEnum.Unaligned, // Most sci-fi adversaries are unaligned
                        ArmorClass = baseMonster.ArmorClass,
                        ChallengeRating = baseMonster.ChallengeRating,
                        Climb = baseMonster.Climb,
                        Constitution = baseMonster.Constitution,
                        CreatureSize = baseMonster.CreatureSize,
                        CreatureSubType = "Construct", // Or "Alien" based on narrative
                        CreatureType = DetermineScifiCreatureType(analysis),
                        Cr = baseMonster.Cr,
                        Equipments = JsonSerializer.Serialize(new List<string> { "Energy Weapon", "Tech Armor" }),
                        HitDice = baseMonster.HitDice,
                        HitPoints = baseMonster.HitPoints,
                        InitiativeBonus = baseMonster.InitiativeBonus,
                        Intelligence = baseMonster.Intelligence,
                        Languages = "Binary, Common",
                        Lore = JsonSerializer.Serialize(new { scifiLore }),
                        Manner = scifiManner.Trim(),
                        MinionArmorClass = baseMonster.MinionArmorClass,
                        MonsterGroup = "Sci-Fi Adversaries",
                        Name = scifiName.Trim(),
                        PageSource = 0,
                        ProficiencyBonus = baseMonster.ProficiencyBonus,
                        Role = role,
                        Skills = baseMonster.Skills,
                        Source = "AI Generated - Sci-Fi",
                        Speed = baseMonster.Speed,
                        Strength = baseMonster.Strength,
                        Swim = baseMonster.Swim,
                        Traits = new List<TraitDbEntity>(),
                        Wisdom = baseMonster.Wisdom,
                        Xp = baseMonster.Xp,
                        Charisma = baseMonster.Charisma,
                        DamageImmunities = "poison", // Most sci-fi constructs/robots are immune to poison
                        DamageResistances = DetermineScifiResistances(analysis),
                        Senses = "darkvision 60 ft., passive Perception " + (10 + ((baseMonster.Wisdom ?? 10) - 10) / 2),
                        CrInLair = baseMonster.CrInLair,
                        DexSavingThrow = baseMonster.DexSavingThrow,
                        StrSavingThrow = baseMonster.StrSavingThrow,
                        ConSavingThrow = baseMonster.ConSavingThrow,
                        IntSavingThrow = baseMonster.IntSavingThrow,
                        WisSavingThrow = baseMonster.WisSavingThrow,
                        ChaSavingThrow = baseMonster.ChaSavingThrow,
                        Fly = baseMonster.Fly,
                        Dexterity = baseMonster.Dexterity,
                        Habitats = "Urban, Spacecraft, Alien World",
                        Symbarum5e = null
                    };

                    // Step 5: Apply role adaptations
                    var roleAdapter = new Monster5eRoleAdapterHelpers();
                    var (roleResult, modifiedMonster) = roleAdapter.AdaptMonsterToRole(newMonster);

                    // Step 6: Generate sci-fi themed actions for the role
                    var scifiActions = await GenerateScifiActions(modifiedMonster, role, scifiLore, analysis);
                    foreach (var action in scifiActions)
                    {
                        modifiedMonster.Actions.Add(action);
                    }

                    // Step 7: Add sci-fi traits
                    var scifiTraits = await GenerateScifiTraits(modifiedMonster, role, analysis);
                    foreach (var trait in scifiTraits)
                    {
                        modifiedMonster.Traits.Add(trait);
                    }

                    // Save to database
                    await _monster5eRepository.Insert(modifiedMonster);

                    return Result<Monster5eDto>.Success(modifiedMonster.ToDto());
                });

        /// <summary>
        /// Generate sci-fi themed actions based on role
        /// </summary>
        private async Task<List<ActionDbEntity>> GenerateScifiActions(
            Monster5eDbEntity monster,
            CombatRoleEnum role,
            string lore,
            ScifiAdversaryAnalysis analysis)
        {
            var actions = new List<ActionDbEntity>();
            var weaponType = analysis?.WeaponType ?? "energy";

            // Generate primary attack
            var primaryActionPrompt = $@"Create a sci-fi weapon attack for a {role} adversary.
    Weapon type: {weaponType}
    Challenge Rating: {monster.ChallengeRating}
    Lore context: {lore}
    
    Respond in D&D 5e format. Include attack bonus, damage, and special effects.
    Example: 'Plasma Rifle. Ranged Weapon Attack: +8 to hit, range 150/600 ft., one target. Hit: 14 (2d8 + 5) radiant damage, and the target must succeed on a DC 15 Constitution saving throw or be blinded until the end of their next turn.'
    
    Make it thematic for sci-fi and appropriate for the {role} role.";

            var primaryAction = await _openAiInterops.GetChatGptResponseAsync(
                primaryActionPrompt,
                temperature: 0.6,
                maxTokens: 200
            );

            actions.Add(new ActionDbEntity
            {
                Id = Guid.NewGuid(),
                MonsterId = monster.Id,
                Name = $"{weaponType.ToCapitalize()} Strike",
                Type = ActionTypeEnum.Action,
                AttackType = weaponType == "melee" ? AttackTypeEnum.Melee : AttackTypeEnum.Ranged,
                Description = primaryAction,
                IsProhibitedForMinion = false
            });

            // Generate role-specific special action
            if (role != CombatRoleEnum.Minion)
            {
                var specialActionPrompt = $@"Create a special sci-fi ability for a {role} that fits this description:
        {RoleDescriptionsHelper.GetRoleDescription(role)}
        
        This should be a unique technological or alien ability that reinforces their {role} combat role.
        Tech level: {analysis?.TechLevel ?? "standard"}
        
        Format as a D&D 5e action with clear mechanics. Make it feel futuristic and cool.";

                var specialAction = await _openAiInterops.GetChatGptResponseAsync(
                    specialActionPrompt,
                    temperature: 0.7,
                    maxTokens: 250
                );

                actions.Add(new ActionDbEntity
                {
                    Id = Guid.NewGuid(),
                    MonsterId = monster.Id,
                    Name = $"{role} Protocol",
                    Type = role == CombatRoleEnum.Solo ? ActionTypeEnum.Legendary : ActionTypeEnum.Action,
                    AttackType = AttackTypeEnum.None,
                    Description = specialAction,
                    IsProhibitedForMinion = true,
                    LimitPerDay = role == CombatRoleEnum.Solo ? null : 1
                });
            }

            return actions;
        }



        ///TODO : Refine traits based on role and tech level

        /// <summary>
        /// Generate sci-fi themed traits
        /// </summary>
        private async Task<List<TraitDbEntity>> GenerateScifiTraits(
            Monster5eDbEntity monster,
            CombatRoleEnum role,
            ScifiAdversaryAnalysis analysis)
        {
            var traits = new List<TraitDbEntity>();

            // Tech Shield trait (common for sci-fi)
            traits.Add(new TraitDbEntity
            {
                Id = Guid.NewGuid(),
                MonsterId = monster.Id,
                Title = "Energy Shielding",
                Description = $"The {monster.Name} has advantage on saving throws against spells and other magical effects. When it takes damage, it can use its reaction to gain resistance to that damage type until the start of its next turn (recharge 5-6).",
                IsOptional = false
            });

            // Role-specific trait
            var roleTraitPrompt = $@"Create a passive sci-fi trait for a {role} adversary.
    This trait should reinforce their {role} combat role and feel technological.
    Keep it to 1-2 sentences. Include mechanical benefits.
    
    Example: 'Targeting Matrix. The unit has advantage on attack rolls against creatures it damaged on its previous turn.'";

            var roleTraitDesc = await _openAiInterops.GetChatGptResponseAsync(
                roleTraitPrompt,
                temperature: 0.6,
                maxTokens: 150
            );

            traits.Add(new TraitDbEntity
            {
                Id = Guid.NewGuid(),
                MonsterId = monster.Id,
                Title = $"{role} Enhancement",
                Description = roleTraitDesc,
                IsOptional = false
            });

            return traits;
        }



        ///TODO : Refine traits based on role and tech level

        /// <summary>
        /// Helper method to determine creature type based on analysis
        /// </summary>
        private string DetermineScifiCreatureType(ScifiAdversaryAnalysis analysis)
        {
            if (analysis == null) return "Construct";

            return analysis.TechLevel switch
            {
                "transcendent" => "Aberration", // For super-advanced alien tech
                "primitive" => "Humanoid", // For low-tech adversaries
                _ => "Construct" // Default for robots/drones/mechs
            };
        }



        ///TODO : Refine traits based on role and tech level

        /// <summary>
        /// Helper method to determine damage resistances based on tech level
        /// </summary>
        private string DetermineScifiResistances(ScifiAdversaryAnalysis analysis)
        {
            if (analysis == null) return "bludgeoning, piercing, slashing from nonmagical attacks";

            return analysis.TechLevel switch
            {
                "transcendent" => "bludgeoning, piercing, slashing from nonmagical attacks; radiant, necrotic",
                "advanced" => "bludgeoning, piercing, slashing from nonmagical attacks; fire",
                "standard" => "bludgeoning, piercing, slashing from nonmagical attacks",
                "primitive" => null,
                _ => "bludgeoning, piercing, slashing from nonmagical attacks"
            };
        }

    }
}
