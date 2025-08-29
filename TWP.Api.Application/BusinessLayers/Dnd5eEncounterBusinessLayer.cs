using Common.Extensions;
using Common.Randomizer;
using Common.ResultPattern;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Application.DataTransferObjects;
using TWP.Api.Application.Helpers;
using TWP.Api.Application.Helpers.Mappers;
using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.CsvRepositories.Interfaces;
using TWP.Api.Infrastructure.Interops.Interfaces;
using TWP.Api.Infrastructure.JsonRepositories.Interfaces;

namespace TWP.Api.Application.BusinessLayers
{
    public class Dnd5eEncounterBusinessLayer : IDndEncounterBusinessLayer
    {
        private readonly IMonsterApiInterops _monsterApiInterops;
        private readonly IDnd2024AllMonsterStatsCsvRepository _dnd2024AllMonsterStatsCsvRepository;
        private readonly IDnd5eEncounterDataJsonRepository _dnd5ERelationBetweenXpAndCrJsonRepository;
        private readonly IOpenAiInterops _openAiInterops;

        public Dnd5eEncounterBusinessLayer(IMonsterApiInterops monsterApiInterops, IDnd2024AllMonsterStatsCsvRepository dnd2024AllMonsterStatsCsvRepository, IDnd5eEncounterDataJsonRepository dnd5ERelationBetweenXpAndCrJsonRepository, IOpenAiInterops openAiInterops)
        {
            _monsterApiInterops = monsterApiInterops;
            _dnd2024AllMonsterStatsCsvRepository = dnd2024AllMonsterStatsCsvRepository;
            _dnd5ERelationBetweenXpAndCrJsonRepository = dnd5ERelationBetweenXpAndCrJsonRepository;
            _openAiInterops = openAiInterops;
        }



        public async Task<Result<Dnd5eEncounterGeneratedDto>> EncounterRandomGenerator(
            EncounterDifficultyEnum encounterDifficulty,
            IList<int> playerLevels,
            string encounterNarrativeContext,
            IList<MonsterHabitatEnum> monsterHabitats)
                => await Safe.ExecuteAsync(async () =>
                {
                    if(playerLevels.HasNoElement())
                        return Result<Dnd5eEncounterGeneratedDto>.Failure("No Elements", ReasonType.BadParameter);
                    var cr = playerLevels.Min();
                    var expEncounterBudget = ComputeExpBudget(encounterDifficulty, playerLevels);

                    var monsterApiResponseDto = await _monsterApiInterops.GetMonstersByChallengeRatingOrlessAsync(cr + 1);
                    if (monsterApiResponseDto == null || monsterApiResponseDto.Results.HasNoElement())
                        return Result<Dnd5eEncounterGeneratedDto>.Failure("No Monsters found for the given CR", ReasonType.NotFound);
                    
                    var monsters = new List<Dnd5eApiMonsterDTO>();
                    foreach (var monsterIndex in monsterApiResponseDto.Results.Select(r => r.Index))
                        monsters.Add(await _monsterApiInterops.GetMonsterByIndexAsync(monsterIndex));

                    var encounterGenerated = GenerateEncounter(monsters!, expEncounterBudget, playerLevels.Count, playerLevels.Min());

                    var formattedEncounterData = EncounterFormatter.GetFormattedEncounterSafe(encounterDifficulty: encounterDifficulty, playerLevels: playerLevels, encounterNarrativeContext, monsterHabitats: monsterHabitats, pickedMonsters: encounterGenerated, expEncounterBudget: expEncounterBudget);

                    var openAiresult = await _openAiInterops.GetChatGptResponseAsync(
                        message: $"Create an Dnd5e Encounter using these data : Encounter data for the Master Game Master to use : {formattedEncounterData.Data} which were picked according to Difficulty : {encounterDifficulty.ToString()}, Number of players : {playerLevels.Count}, Party Level : {playerLevels.Min()}, NarrativeContext : {encounterNarrativeContext}, Monster Habitats : {string.Join(",", monsterHabitats.Select(h => h.ToString()))}",
                        systemPrompt: $"You are a Master Game Master (GM) tasked with creating a complete D&D 5e encounter. The encounter's difficulty, monster selection, habitat, and challenge rating ({cr}) have been given. Your role is to generate the following:\r\n\r\nEncounter Description:\r\nCreate an engaging narrative (narrative context : {encounterNarrativeContext}) that introduces the encounter. This should include:\r\n\r\nA description of the environment where the encounter takes place.\r\n\r\nEnsuring the monsters are integrated into the narrative which are those : Encounter data for the Master Game Master to use : {formattedEncounterData.Data}.\r\n\r\nAny immediate effects or environmental hazards the players should be aware of (e.g., traps, difficult terrain, weather conditions).\r\n\r\nMonster Stats & Tactics:\r\nProvide the stats for the Mud Mephit, including:\r\n\r\nHit Points (HP), Armor Class (AC), Damage output, and relevant abilities.\r\n\r\nSpecial abilities and spells the the monster that are given to you can use.\r\n\r\nTactical advice for the GM (you are writing for a \"noob GM,\" so make sure it is simple and clear). This should include:\r\n\r\nHow the the monster that are given to you behaves in combat.\r\n\r\nIdeal strategies or tactics the the monster that are given to you would employ.\r\n\r\nWeaknesses or vulnerabilities that the players can exploit.\r\n\r\nTreasure:\r\nBased on the CR and level of the encounter, generate treasure that fits with the encounter. This should include:\r\n\r\nGold or valuable items, ensuring the treasure is balanced for the party’s level.\r\n\r\nMagic items that are thematically appropriate for the the monster that are given to you and the habitat."
                        );
                    var finaleResult = new Dnd5eEncounterGeneratedDto() 
                                        { 
                                            Cr = cr, EncounterDifficulty = encounterDifficulty, 
                                            EncounterNarrativeContext = encounterNarrativeContext, 
                                            MonsterHabitats = monsterHabitats, 
                                            Monsters = encounterGenerated, 
                                            OpenAiResponse = openAiresult,
                                            FormattedEncounterData = formattedEncounterData.Data!
                    };

                    return Result<Dnd5eEncounterGeneratedDto>.Success(finaleResult);
                });
  

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
        private List<Dnd5eApiMonsterDTO> GenerateEncounter(List<Dnd5eApiMonsterDTO> monsters, int expEncounterBudget, int playerNumber, int partyLevel)
        {
            var encounter = new List<Dnd5eApiMonsterDTO>();
            var remainingBudget = expEncounterBudget;
            var maxMonsters = playerNumber * 2;
            var maxDifferentMonster = 4;

            // Group monsters by Challenge Rating (CR), filtering monsters whose XP is within the budget.
            var selectedAvailableMonsters = monsters.Where(m => m.Xp.HasValue && m.Xp <= expEncounterBudget)
                                        .OrderBy(_ => Guid.NewGuid()) // Shuffle types
                                        .ToList();

            for (var i = 0; i < selectedAvailableMonsters.Count && remainingBudget > 9 /* 10 is the minimum xp budget available for a dnd monster */ && encounter.Count < maxMonsters; i++)
            {
                var monster = selectedAvailableMonsters[i];

                // Add monster if it fits within the remaining budget
                if (monster.Xp <= remainingBudget && encounter.Count < maxMonsters)
                {
                    encounter.Add(monster);
                    remainingBudget -= monster.Xp.Value;
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
                        remainingBudget -= lastMonster.Xp.Value;
                    }
                }

                // Ensure that we include only one monster of the partyLevel + 1 CR in the encounter
                if (IsCrPlusOneAlreadyIncludedInEncounter(encounter, partyLevel))
                    selectedAvailableMonsters = selectedAvailableMonsters.Where(m => m.ChallengeRating <= (decimal)partyLevel).ToList();
            }

            return encounter;
        }

        /// <summary>
        /// Checks if a monster with CR +1 has already been included in the encounter.
        /// </summary>
        /// <param name="encounter">List of monsters in the current encounter.</param>
        /// <param name="partyLevel">The level of the party.</param>
        /// <returns>True if a monster with CR + 1 is already in the encounter.</returns>
        private bool IsCrPlusOneAlreadyIncludedInEncounter(List<Dnd5eApiMonsterDTO> encounter, int partyLevel)
        {
            return encounter.Any(m => m.ChallengeRating == partyLevel + 1);
        }

    }
}
