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
            MonsterHabitatEnum monsterHabitat)
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

                    var encounterGenerated = GenerateEncounter(pickedMonsters!, expEncounterBudget, playerLevels.Count, playerLevels.Min());
                    if(encounterGenerated.expRemainingBudget != 0)
                    {
                        var originalMonsterBasedOnOneOfTheGeneratedencounter = await ReskinDndMonster(encounterGenerated.expRemainingBudget, encounterGenerated.encounterMonsters.FirstOrDefault().Name);
                        encounterGenerated.encounterMonsters.Add(originalMonsterBasedOnOneOfTheGeneratedencounter.Data);
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
        public async Task<Result<Monster5eDto>> ReskinDndMonster(int experience, string monsterName)
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
                do
                {
                    randomRole = RoleDescriptionsHelper.GetOneRandomRole();
                } while (baseMonster.Role.Value == randomRole);
                var roleDescription = RoleDescriptionsHelper.GetRoleDescription(randomRole);

                var results = await _monster5eRepository.FindByCrAsync(monsterBuildingGuideLines.Data.CRNumeric);
                var monsterCarac = results.Data.Shuffle().FirstOrDefault();

                //Generate monster lore from base monster
                var originalLore = await _openAiInterops.GetChatGptResponseAsync($"Take the base lore of {baseMonster.Name} and use it to create a original lore for {baseMonster.Name} that has the role {randomRole} : {roleDescription}. Create what this {randomRole} variation has more, what set it appart !");
                var originalName = await _openAiInterops.GetChatGptResponseAsync($"Take the base name of {baseMonster.Name} and use it to create a original name for a monster that has the role {randomRole} : {roleDescription} and this lore {originalLore}");

                var originalManner = await _openAiInterops.GetChatGptResponseAsync($"Take the base lore of {baseMonster.Name} and use it to create a very short (10 to 15 words) manner description for {baseMonster.Name} that has the role {randomRole} : {roleDescription}.");

                //TODO -> Improve
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

                    newMonster.Actions.Add(newAction.ToDbEntity());
                }
                else //Minion cannot have actions
                    newMonster.Actions = null;

                await _monster5eRepository.Insert(newMonster);

                return Result<Monster5eDto>.Success(newMonster.ToDto());
            });

        private static string CreateRoleActionPrompt(Monster5eDbEntity baseMonster, CombatRoleEnum role, string originalLore, string roleDescription, ActionTypeEnum actionType, AttackTypeEnum attackTypeEnum)
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
        private (List<Monster5eDto> encounterMonsters, int expRemainingBudget) GenerateEncounter(List<Monster5eDto> monsters, int expEncounterBudget, int playerNumber, int partyLevel)
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

            return (encounter, remainingBudget);
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

    }
}
