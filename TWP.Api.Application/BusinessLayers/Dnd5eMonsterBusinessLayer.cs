using Common.Randomizer;
using Common.ResultPattern;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Application.DataTransferObjects;
using TWP.Api.Application.Helpers.Mappers;
using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Core.DbEntities;
using TWP.Api.Infrastructure.CsvRepositories.Interfaces;
using TWP.Api.Infrastructure.Repository.Interfaces;

namespace TWP.Api.Application.BusinessLayers
{
    public class Dnd5eMonsterBusinessLayer : IDnd5eMonsterBusinessLayer
    {
        private readonly IDnd2024AllMonsterStatsCsvRepository _csvRepository;
        private readonly IMonster5eRepository _monster5ERepository;
        private readonly IMonsterBuildingGuidelineRepository _monsterBuildingGuidelineRepository;

        public Dnd5eMonsterBusinessLayer(IDnd2024AllMonsterStatsCsvRepository csvRepository, IMonster5eRepository monster5ERepository, IMonsterBuildingGuidelineRepository monsterBuildingGuidelineRepository)
        {
            _csvRepository = csvRepository;
            _monster5ERepository = monster5ERepository;
            _monsterBuildingGuidelineRepository = monsterBuildingGuidelineRepository;
        }

        public async Task<Result<Monster5eDto>> CreateOriginalDndMonster(float challengeRating)
            => await Safe.ExecuteAsync(async () =>
            {
                //TODO
                //Pick a base monster from dnd2024
                var baseMonsters = await _monster5ERepository.FindByCrAsync(challengeRating);
                if (baseMonsters.IsFailure)
                    return Result<Monster5eDto>.Failure("No Monsters found for the given CR", ReasonType.NotFound);
                var randomSelector = new RandomSelector<Monster5eDbEntity>();
                var baseMonster = randomSelector.SelectOneRandomly(baseMonsters.Data.ToArray());

                //Pick Role

                //Generate monster lore from base monster
                var baseLore = baseMonster.Lore;

                //Get Guide line
                //var guideLineForCr = _monsterBuildingGuidelineRepository.GetByCRAsync(baseMonster.Cr);

                //Pick Traits

                //Pick Action
                throw new NotImplementedException();
            });

        public async Task<Result<List<Monster5eDto>>> GetAll5eMonsters()
            => await Safe.ExecuteAsync(async () =>
            {
                var results = await _monster5ERepository.GetAllAsync();
                if (results.IsFailure)
                    return Result<List<Monster5eDto>>.Failure(results.Error!, results.ReasonType);
                if (results.Data is null || !results.Data.Any())
                    return Result<List<Monster5eDto>>.Failure("No data", ReasonType.NotFound);
                return Result<List<Monster5eDto>>.Success(results.Data.Select(m => m.ToDto()).ToList());
            });

        public async Task<Result<List<Dnd5eMonsterDto>>> GetAllMonsterStatsByCr(int cr)
            => await Safe.ExecuteAsync(async () =>
            {
                var results = _csvRepository.GetAllDnd5e2024MonsterStatsByCr(cr).Verify(r => r.IsNull());
                if(results.IsFailure)
                    return Result<List<Dnd5eMonsterDto>>.Failure(results.Error!, results.ReasonType);
                return results;
            });
    }
} 