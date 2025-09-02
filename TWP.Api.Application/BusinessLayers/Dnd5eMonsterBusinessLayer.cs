using Common.ResultPattern;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Application.Helpers.Mappers;
using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Infrastructure.CsvRepositories.Interfaces;
using TWP.Api.Infrastructure.Repository.Interfaces;

namespace TWP.Api.Application.BusinessLayers
{
    public class Dnd5eMonsterBusinessLayer : IDnd5eMonsterBusinessLayer
    {
        private readonly IDnd2024AllMonsterStatsCsvRepository _csvRepository;
        private readonly IMonster5eRepository _monster5ERepository;

        public Dnd5eMonsterBusinessLayer(IDnd2024AllMonsterStatsCsvRepository csvRepository, IMonster5eRepository monster5ERepository)
        {
            _csvRepository = csvRepository;
            _monster5ERepository = monster5ERepository;
        }

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