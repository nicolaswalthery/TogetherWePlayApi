using Common.Extensions;
using Common.ResultPattern;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Application.Helpers;
using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Core.Enums;
using TWP.Api.Infrastructure.CsvRepositories.Interfaces;
using TWP.Api.Infrastructure.DataTransferObjects;
using TWP.Api.Infrastructure.JsonRepositories.Interfaces;

namespace TWP.Api.Application.BusinessLayers
{
    public class Dnd5eEncounterBusinessLayer : IDndEncounterBusinessLayer
    {
        private readonly IDnd2024AllMonsterStatsCsvRepository _dnd2024AllMonsterStatsCsvRepository;
        private readonly IMonsterActivitiesJsonRepository _monsterActivitiesJsonRepository;
        private readonly ISomethingHappenJsonRepository _somethingHappenJsonRepository;

        public Dnd5eEncounterBusinessLayer(IDnd2024AllMonsterStatsCsvRepository dnd2024AllMonsterStatsCsvRepository, IMonsterActivitiesJsonRepository monsterActivitiesJsonRepository, ISomethingHappenJsonRepository somethingHappenJsonRepository)
        {
            _dnd2024AllMonsterStatsCsvRepository = dnd2024AllMonsterStatsCsvRepository;
            _monsterActivitiesJsonRepository = monsterActivitiesJsonRepository;
            _somethingHappenJsonRepository = somethingHappenJsonRepository;
        }

        public async Task<Result<List<Dnd5eMonsterDto>>> EncounterRandomGenerator(EncounterDifficultyEnum encounterDifficulty, IList<int> playerLevels, string EncounterNarrativeContext)
            => await Safe.ExecuteAsync(async () =>
            {
                if(playerLevels.HasNoElement())
                    return Result<List<Dnd5eMonsterDto>>.Failure("No Elements", ReasonType.BadParameter);

                var expEncounterBudget = ComputeExpBudget(encounterDifficulty, playerLevels);
               
                var result = _dnd2024AllMonsterStatsCsvRepository.GetAllDnd5e2024MonsterStatsByCr(playerLevels.Min()).Verify(data => data.IsNull());
                if (result.IsFailure)
                    return Result<List<Dnd5eMonsterDto>>.Failure(result.Error!, result.ReasonType);

                //Introduice ChatGPT to create 

                return result;
            });

        /// <summary>
        /// Compute the experience budget for a party
        /// </summary>
        /// <returns></returns>
        private int ComputeExpBudget(EncounterDifficultyEnum encounterDifficulty, IList<int> playerLevels)
        {
            if(playerLevels.Any(pl => pl <= 0 || pl > 20))
                return 0;

            // XP budget table as per the image
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
    }
}
