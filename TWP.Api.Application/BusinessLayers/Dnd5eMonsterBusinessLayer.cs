using Common.ResultPattern;
using System.Threading.Tasks;
using TWP.Api.Application.BusinessLayers.Interfaces;
using TWP.Api.Infrastructure.CsvRepositories.Interfaces;

namespace TWP.Api.Application.BusinessLayers
{
    public class Dnd5eMonsterBusinessLayer : IDnd5eMonsterBusinessLayer
    {
        private readonly IDnd2024AllMonsterStatsCsvRepository _csvRepository;

        public Dnd5eMonsterBusinessLayer(IDnd2024AllMonsterStatsCsvRepository csvRepository)
        {
            _csvRepository = csvRepository;
        }

        public async Task<Result<string>> GetAllMonsterStatsCsv()
            => await Safe.ExecuteAsync(async () =>
            {
                var csv = _csvRepository.GetCsvString();
                return Result<string>.Success(csv);
            });
    }
} 