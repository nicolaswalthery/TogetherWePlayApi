using Common.Extensions;
using Common.Randomizer;
using Common.ResultPattern;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Application.Helpers.Mappers;
using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.CsvRepositories.Interfaces;
using TWP.Api.Infrastructure.JsonRepositories.Interfaces;

namespace TWP.Api.Application.BusinessLayers
{
    public class Dnd5eEncounterBusinessLayer : IDndEncounterBusinessLayer
    {
        private readonly IDnd2024AllMonsterStatsCsvRepository _dnd2024AllMonsterStatsCsvRepository;
        private readonly IDnd5eEncounterDataJsonRepository _dnd5ERelationBetweenXpAndCrJsonRepository;

        public Dnd5eEncounterBusinessLayer(IDnd2024AllMonsterStatsCsvRepository dnd2024AllMonsterStatsCsvRepository, IDnd5eEncounterDataJsonRepository dnd5ERelationBetweenXpAndCrJsonRepository)
        {
            _dnd2024AllMonsterStatsCsvRepository = dnd2024AllMonsterStatsCsvRepository;
            _dnd5ERelationBetweenXpAndCrJsonRepository = dnd5ERelationBetweenXpAndCrJsonRepository;
        }

        public async Task<Result<List<Dnd5eMonsterDto>>> EncounterRandomGenerator(
            EncounterDifficultyEnum encounterDifficulty,
            IList<int> playerLevels,
            string encounterNarrativeContext,
            IList<MonsterHabitatEnum> monsterHabitats)
            => await Safe.ExecuteAsync(async () =>
            {
                if(playerLevels.HasNoElement())
                    return Result<List<Dnd5eMonsterDto>>.Failure("No Elements", ReasonType.BadParameter);

                var result = _dnd2024AllMonsterStatsCsvRepository.GetAllDnd5e2024MonsterStatsByCr(playerLevels.Min()).Where(m => m.Name.IsNotNullOrEmptyOrWhiteSpace()).ToList().Verify(data => data.IsNull());
                if (result.IsFailure)
                    return Result<List<Dnd5eMonsterDto>>.Failure(result.Error!, result.ReasonType);

                //Fix the fact that the csv do not have XP data for each monster. This code is not intended to stay.
                var resultCrRelatedToXp = _dnd5ERelationBetweenXpAndCrJsonRepository.GetAll();
                foreach (var datum in result.Data!.Where(d => d.XP.IsNullOrEmptyOrWhiteSpace()))
                    datum.XP = resultCrRelatedToXp.First(r => r.CR == datum.CR).XP.ToString();

                var filteredMonsters = monsterHabitats.Contains(MonsterHabitatEnum.Any) ? result.Data : result.Data!.Where(d => monsterHabitats.Contains(d.Habitat.ToEnum())).ToList();

                var expEncounterBudget = ComputeExpBudget(encounterDifficulty, playerLevels);

                var encounterGenerated = GenerateEncounter(filteredMonsters, expEncounterBudget, playerLevels.Count);
                
                //Introduice ChatGPT to create 

                return Result<List<Dnd5eMonsterDto>>.Success(encounterGenerated);
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
        /// <returns>List of monsters for the encounter.</returns>
        private List<Dnd5eMonsterDto> GenerateEncounter(IList<Dnd5eMonsterDto> monsters, int expEncounterBudget, int playerNumber)
        {
            var encounter = new List<Dnd5eMonsterDto>();
            int remainingBudget = expEncounterBudget;
            int maxMonsters = playerNumber * 2;

            var availableShuffledMonsters = monsters.Where(m => m.SanitizedXp <= expEncounterBudget).ToList().Shuffle();
            for(var i = 0; i < availableShuffledMonsters.ToArray().Length && remainingBudget > 9/*10 is the minimum xp budget available for a dnd monster*/ && encounter.Count < maxMonsters; i++)
            {
                var monster = availableShuffledMonsters[i];
                if (monster.SanitizedXp <= remainingBudget && encounter.Count < maxMonsters)
                {
                    encounter.Add(monster);
                    remainingBudget -= monster.SanitizedXp;
                }
                else  // If we have not reached the max number of monsters and have remaining budget, try to add one last monster that fits the budget
                {
                    var lastMonster = monsters.Where(m => m.SanitizedXp <= remainingBudget && !encounter.Contains(m))
                                              .OrderByDescending(m => m.SanitizedXp).FirstOrDefault();
                    if (lastMonster != null)
                    {
                        encounter.Add(lastMonster);
                        remainingBudget -= lastMonster.SanitizedXp;
                    }
                    else
                    {
                        remainingBudget = 0;
                    }
                }
            }

            return encounter;
        }
    }
}
