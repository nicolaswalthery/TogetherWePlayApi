using TWP.Api.Infrastructure.Helpers;
using TWP.Api.Infrastructure.CsvRepositories.Interfaces;
using TWP.Api.Core.DataTransferObjects;

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
    }
} 