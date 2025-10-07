using Common.Extensions;
using Common.Randomizer;
using Common.ResultPattern;
using System.Text.Json;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Application.DataTransferObjects;
using TWP.Api.Application.Helpers;
using TWP.Api.Application.Helpers.Mappers;
using TWP.Api.Application.Interfaces.Services;
using TWP.Api.Application.Prompts;
using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Core.DbEntities;
using TWP.Api.Core.Enums;
using TWP.Api.Core.Interface.Infrastructure;

namespace TWP.Api.Application.BusinessLayers
{
    public class Dnd5eEncounterBusinessLayer : IDndEncounterBusinessLayer
    {
        private readonly IMonster5eRepository _monster5eRepository;
        private readonly IOpenAiServices _openAiInterops;
        private readonly IMonsterBuildingGuidelineRepository _monsterBuildingGuidelineRepository;

        public Dnd5eEncounterBusinessLayer(IMonster5eRepository monster5eRepository, IOpenAiServices openAiInterops, IMonsterBuildingGuidelineRepository monsterBuildingGuidelineRepository)
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

                    var monsterDbEntities = await _monster5eRepository.FindByCrOrLessAsync(cr+1);
                    if (monsterDbEntities.Data.HasNoElement())
                        return Result<Dnd5eEncounterGeneratedDto>.Failure("No Monsters found for the given CR or less", ReasonType.NotFound);

                    var monsters = monsterDbEntities.Data.Where(m => m.Source.Contains("Monster Manual 2024")).Select(m => m.ToDto()).ToList();
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

                var originalLoreTask = _openAiInterops.GetChatGptResponseAsync($"Take the base lore of {baseMonster.Name} and use it to create an original lore for {baseMonster.Name} that has the role {randomRole}: {roleDescription}. Describe what this {randomRole} variant adds and what sets it apart.");
                var originalMannerTask = _openAiInterops.GetChatGptResponseAsync($"Take the base lore of {baseMonster.Name} and create a very short (10–15 words) manner description for {baseMonster.Name} with the role {randomRole}: {roleDescription}.");
                var originalNameTask = originalLoreTask
                    .ContinueWith(t =>
                        _openAiInterops.GetChatGptResponseAsync(
                            $"Take the base name of {baseMonster.Name} and create an original name for a monster with the role {randomRole}: {roleDescription} and this lore: {t.Result}"
                        ),
                        TaskContinuationOptions.OnlyOnRanToCompletion | TaskContinuationOptions.ExecuteSynchronously
                    ).Unwrap();

                await Task.WhenAll(originalMannerTask, originalNameTask);

                var originalLore = await originalLoreTask;   
                var originalManner = await originalMannerTask;
                var originalName = await originalNameTask;

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

                    var guidline = await _monsterBuildingGuidelineRepository.GetByNumericCRAsync(monsterBuildingGuideLines.Data.CRNumeric);
                    if(guidline.IsFailure)
                        return Result<Monster5eDto>.Failure("No guidlines found for the given CR", ReasonType.NotFound);

                    string roleBasedActions = String.Empty;
                    if (randomRole != CombatRoleEnum.Minion)
                        roleBasedActions = await _openAiInterops.ChatGptResponseAsync(baseMonster.RoleActionCreationPrompt(guidline.Data!, randomRole, originalLore, roleDescription, actionType, attackType));

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
        public async Task<Result<List<Monster5eDto>>> CreateScifiAdversaries(
            string narrativeDescription,
            CombatRoleEnum? forcedCombatRole = null)
                => await Safe.ExecuteAsync(async () =>
                {
                    if (string.IsNullOrWhiteSpace(narrativeDescription))
                        return Result<List<Monster5eDto>>.Failure("Narrative description cannot be empty", ReasonType.BadParameter);

                    var analysisJson = await _openAiInterops.GetChatGptResponseAsync(
                        narrativeDescription.GetAnalyzeNarrativeToDetermineAppropriateCrAndSuggestedRolePrompt(),
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
                    var baseMonster = new RandomSelector<Monster5eDbEntity>().SelectOneRandomly(baseMonsterResult.Data.ToArray());

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
                    Tech level: {analysis.TechLevel}
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
                    Tech level: {analysis.TechLevel}
            
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

                    var creatureTypePrompt = $@"Figure out the creature main type for:
                                        {scifiName} with this lore: {scifiLore} {narrativeDescription} Focus on what they are in this lore. 
                                        Just give the main type, nothing more. Ex: Aberration - Créatures étranges et alien (mind flayers, beholders)
                                        Beast - Animaux naturels non-magiques (ours, loups, aigles)
                                        Celestial - Êtres des plans supérieurs (anges, pégases)
                                        Construct - Créatures artificielles animées (golems, modrons)
                                        Dragon - Dragons vrais et créatures draconiques (wyverns, drakes)
                                        Elemental - Êtres des plans élémentaires (élémentaires de feu, génies)
                                        Fey - Créatures du Feywild (pixies, dryades, satyres)
                                        Fiend - Êtres des plans inférieurs (démons, diables)
                                        Giant - Géants et créatures apparentées (ogres, trolls)
                                        Humanoid - Créatures bipèdes civilisées (humains, elfes, orcs, gobelins)
                                        Monstrosity - Monstres naturels mais inhabituels (griffons, minotaures)
                                        Ooze - Créatures gélatineuses (gelées, vases)
                                        Plant - Végétation animée (treants, blights)
                                        Undead - Morts-vivants (zombies, vampires, liches).";

                    var creatureType = await _openAiInterops.GetChatGptResponseAsync(creatureTypePrompt, temperature: 0.6, maxTokens: 50);

                    var creatureSubTypePrompt = $@"Figure out the creature subtype for:
                                        {scifiName} with this lore: {scifiLore} {narrativeDescription} Focus on what they are in this lore. 
                                        Just give the subtype, nothing more. Ex: Specific Name of the race or the species.";

                    var creatureSubType = await _openAiInterops.GetChatGptResponseAsync(creatureSubTypePrompt, temperature: 0.6, maxTokens: 50);

                    var damageImmunitiesPrompt = $@"Figure out the creature's immunities if any for:
                                        {scifiName} - with this lore: {scifiLore} {narrativeDescription} Focus on what they are in this lore. 
                                        Just give the list of immunities, nothing more. List of Immunities : Blinded, Charmed, Deafened, Exhaustion, Frightened, Grappled, Incapacitated, Invisible, Paralyzed, Petrified, Poisoned, Prone, Restrained, Stunned, Unconscious. 
                                        Keep in mind that the monster might have no immunities. Pick the ones that make the most sense according to the lore.";

                    var damageImmunities = await _openAiInterops.GetChatGptResponseAsync(damageImmunitiesPrompt, temperature: 0.6, maxTokens: 50);

                    var sensoryCapabilitiesPrompt = $@"Figure out the creature's sensory capabilities, if any, for:
                                                    {scifiName} - with this lore: {scifiLore} {narrativeDescription} Focus on what they are in this lore. 
                                                    Just give the list of senses, nothing more. List of Senses: Blindsight, Darkvision, Tremorsense, Truesight, Low-Light Vision, Thermal Vision, Echolocation, Radar Sense, Cybernetic Vision, Psionic Sense. 
                                                    Keep in mind that the creature might have only normal sight or no special senses at all. Pick the ones that make the most sense according to the lore.";

                    var sensoryCapabilities = await _openAiInterops.GetChatGptResponseAsync(sensoryCapabilitiesPrompt, temperature: 0.6, maxTokens: 50);

                    var habitatPrompt = $@"Figure out the creature's natural habitats, if any, for:
                                {scifiName} - with this lore: {scifiLore} {narrativeDescription} Focus on where this creature would most likely be found based on the lore. 
                                Just give the list of habitats, nothing more. List of Habitats: Any, Arctic, Coastal, Desert, Forest, Grassland, Hill, Mountain, Swamp, Underground, Underwater, Urban.
                                Keep in mind that the creature might thrive in multiple or only one specific habitat. Pick the ones that make the most sense according to the lore.";

                    var habitats = await _openAiInterops.GetChatGptResponseAsync(habitatPrompt, temperature: 0.6, maxTokens: 50);

                    var damageResistancesPrompt = $@"Figure out the creature's damage resistances, if any, for:
                    {scifiName} - with this lore: {scifiLore} {narrativeDescription} Focus on what they are in this lore. 
                    Just give the list of resistances, nothing more. List of Resistances: Acid, Bludgeoning, Cold, Fire, Force, Lightning, Necrotic, Piercing, Poison, Psychic, Radiant, Slashing, Thunder.
                    Keep in mind that the creature might have no resistances. Pick the ones that make the most sense according to the lore.";

                    var damageResistances = await _openAiInterops.GetChatGptResponseAsync(damageResistancesPrompt, temperature: 0.6, maxTokens: 50);


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
                        CreatureSubType = creatureSubType,
                        CreatureType = creatureType,
                        Cr = baseMonster.Cr,
                        Equipments = null,
                        HitDice = baseMonster.HitDice,
                        HitPoints = baseMonster.HitPoints,
                        InitiativeBonus = baseMonster.InitiativeBonus,
                        Intelligence = baseMonster.Intelligence,
                        Languages = "Common",
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
                        DamageImmunities = damageImmunities,
                        DamageResistances = damageResistances,
                        Senses = sensoryCapabilities,
                        CrInLair = baseMonster.CrInLair,
                        DexSavingThrow = baseMonster.DexSavingThrow,
                        StrSavingThrow = baseMonster.StrSavingThrow,
                        ConSavingThrow = baseMonster.ConSavingThrow,
                        IntSavingThrow = baseMonster.IntSavingThrow,
                        WisSavingThrow = baseMonster.WisSavingThrow,
                        ChaSavingThrow = baseMonster.ChaSavingThrow,
                        Fly = baseMonster.Fly,
                        Dexterity = baseMonster.Dexterity,
                        Habitats = habitats,
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
                    var scifiTraits = await GenerateScifiTraits(modifiedMonster, role, scifiLore, analysis);
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

            var primaryAction = await _openAiInterops.GetChatGptResponseAsync(
                $"Create the main action of this monster. {monster.GetDnd5eMonsterActionCreationPrompt()}",
                temperature: 0.6,
                maxTokens: 1000,
                responseFormat: OpenAIResponseFormatEnum.Json);

            // Parse the analysis
            var primaryActionDbEntity = JsonSerializer.Deserialize<ActionDbEntity>(primaryAction);

            actions.Add(primaryActionDbEntity);

            // Generate role-specific special action
            if (role != CombatRoleEnum.Minion)
            {
                var roleAction = await _openAiInterops.GetChatGptResponseAsync(
                    $"Create the role specific action of this monster which have the combat role : {role}. {monster.GetDnd5eMonsterActionCreationPrompt()}",
                    temperature: 0.7,
                    maxTokens: 1000,
                    responseFormat: OpenAIResponseFormatEnum.Json
                    );

                var roleActionDbEntity = JsonSerializer.Deserialize<ActionDbEntity>(roleAction);

                actions.Add(roleActionDbEntity);
            }

            return actions;
        }

        /// <summary>
        /// Generate sci-fi themed traits based on role
        /// </summary>
        private async Task<List<TraitDbEntity>> GenerateScifiTraits(
            Monster5eDbEntity monster,
            CombatRoleEnum role,
            string lore,
            ScifiAdversaryAnalysis analysis)
        {
            var traits = new List<TraitDbEntity>();

            // Generate primary defensive/survival trait
            var primaryTrait = await _openAiInterops.GetChatGptResponseAsync(
                $"Create the main defensive or survival trait of this monster. {monster.GetDnd5eMonsterTraitCreationPrompt()}",
                temperature: 0.6,
                maxTokens: 1000,
                responseFormat: OpenAIResponseFormatEnum.Json);

            var primaryTraitDbEntity = JsonSerializer.Deserialize<TraitDbEntity>(primaryTrait, new JsonSerializerOptions{  PropertyNameCaseInsensitive = true });
            traits.Add(primaryTraitDbEntity);

            // Generate role-specific trait
            if (role != CombatRoleEnum.Minion)
            {
                var roleSpecificPrompt = role switch
                {
                    CombatRoleEnum.Brute =>
                        $"Create an aggressive trait (damage bonus when wounded, cleave attacks, or brutal critical) for this {role} monster. {monster.GetDnd5eMonsterTraitCreationPrompt()}",

                    CombatRoleEnum.Soldier =>
                        $"Create a defensive trait (armor, shields, damage reduction, or formation tactics) for this {role} monster. {monster.GetDnd5eMonsterTraitCreationPrompt()}",

                    CombatRoleEnum.Controller =>
                        $"Create a control trait (area denial, debuff aura, or movement restriction) for this {role} monster. {monster.GetDnd5eMonsterTraitCreationPrompt()}",

                    CombatRoleEnum.Skirmisher =>
                        $"Create a mobility trait (hit-and-run, enhanced movement, or escape ability) for this {role} monster. {monster.GetDnd5eMonsterTraitCreationPrompt()}",

                    CombatRoleEnum.Ambusher =>
                        $"Create a stealth or surprise trait (first strike, camouflage, or ambush bonus) for this {role} monster. {monster.GetDnd5eMonsterTraitCreationPrompt()}",

                    CombatRoleEnum.Artillery =>
                        $"Create a ranged combat trait (extended range, piercing shots, or targeting systems) for this {role} monster. {monster.GetDnd5eMonsterTraitCreationPrompt()}",

                    CombatRoleEnum.Solo =>
                        $"Create a legendary trait (multiple reactions, legendary resistance, or action recovery) for this {role} boss monster. {monster.GetDnd5eMonsterTraitCreationPrompt()}",

                    CombatRoleEnum.Support =>
                        $"Create a support trait (healing allies, providing buffs, or shielding) for this {role} monster. {monster.GetDnd5eMonsterTraitCreationPrompt()}",

                    CombatRoleEnum.Leader =>
                        $"Create a leadership trait (command aura, tactical coordination, or morale boost) for this {role} monster. {monster.GetDnd5eMonsterTraitCreationPrompt()}",

                    _ => $"Create a tactical trait appropriate for this {role} monster. {monster.GetDnd5eMonsterTraitCreationPrompt()}"
                };

                var roleTrait = await _openAiInterops.GetChatGptResponseAsync(
                    roleSpecificPrompt,
                    temperature: 0.7,
                    maxTokens: 1000,
                    responseFormat: OpenAIResponseFormatEnum.Json);

                var roleTraitDbEntity = JsonSerializer.Deserialize<TraitDbEntity>(roleTrait, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                traits.Add(roleTraitDbEntity);
            }

            // Add additional traits based on role and CR
            if (monster.Cr >= 10 && role != CombatRoleEnum.Minion)
            {
                // Solo monsters get extra traits
                if (role == CombatRoleEnum.Solo)
                {
                    var legendaryTrait = await _openAiInterops.GetChatGptResponseAsync(
                        $"Create a powerful legendary resistance or recovery trait for this CR {monster.Cr} solo boss. {monster.GetDnd5eMonsterTraitCreationPrompt()}",
                        temperature: 0.8,
                        maxTokens: 250,
                        responseFormat: OpenAIResponseFormatEnum.Json);

                    var legendaryTraitDbEntity = JsonSerializer.Deserialize<TraitDbEntity>(legendaryTrait);
                    traits.Add(legendaryTraitDbEntity);
                }

                // Leaders get command traits
                if (role == CombatRoleEnum.Leader)
                {
                    var commandTrait = await _openAiInterops.GetChatGptResponseAsync(
                        $"Create a tactical command trait that affects allies for this CR {monster.Cr} leader. {monster.GetDnd5eMonsterTraitCreationPrompt()}",
                        temperature: 0.7,
                        maxTokens: 250,
                        responseFormat: OpenAIResponseFormatEnum.Json);

                    var commandTraitDbEntity = JsonSerializer.Deserialize<TraitDbEntity>(commandTrait);
                    traits.Add(commandTraitDbEntity);
                }

                // High CR non-minions get an optional special trait
                var specialTrait = await _openAiInterops.GetChatGptResponseAsync(
                    $"Create a unique trait befitting a CR {monster.Cr} {role} monster. This should be thematic and balanced. {monster.GetDnd5eMonsterTraitCreationPrompt()}",
                    temperature: 0.8,
                    maxTokens: 250,
                    responseFormat: OpenAIResponseFormatEnum.Json);

                var specialTraitDbEntity = JsonSerializer.Deserialize<TraitDbEntity>(specialTrait);
                specialTraitDbEntity.IsOptional = true;
                traits.Add(specialTraitDbEntity);
            }

            return traits;
        }
    }
}
