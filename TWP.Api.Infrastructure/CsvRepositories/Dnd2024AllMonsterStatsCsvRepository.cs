using TWP.Api.Core.DataTransferObjects;
using TWP.Api.Infrastructure.CsvRepositories.Interfaces;
using TWP.Api.Infrastructure.Helpers;

namespace TWP.Api.Infrastructure.CsvRepositories
{
    /// <summary>
    /// The CSV Repository retrieves the CSV data as a string.
    /// </summary>
    public class Dnd2024AllMonsterStatsCsvRepository : CsvRepositoryBase, IDnd2024AllMonsterStatsCsvRepository
    {
        public Dnd2024AllMonsterStatsCsvRepository() : base(fileName: "Dnd2024AllMonsterStats.csv")
        {
        }

        public List<Dnd5eMonsterDto> GetAllDnd5e2024MonsterStats()
            => GetCsvPath().MapCsvTo<Dnd5eMonsterDto>(true);

        public List<Dnd5eMonsterDto> GetAllDnd5e2024MonsterStatsByCr(int cr)
            => GetAllDnd5e2024MonsterStats().Where(m => m.SanitizedCr <= cr).ToList();

    }
} 