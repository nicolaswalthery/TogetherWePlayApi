using TWP.Api.Infrastructure.CsvRepositories;

namespace TWP.Api.Infrastructure.CsvRepositories
{
    /// <summary>
    /// The CSV Repository retrieves the CSV data as a string.
    /// </summary>
    public class Dnd2024AllMonsterStatsCsvRepository : CsvRepositoryBase, IDnd2024AllMonsterStatsCsvRepository
    {
        public Dnd2024AllMonsterStatsCsvRepository() : base(folderName: "CsvFiles", fileName: "Dnd2024AllMonsterStats.csv")
        {
        }

        public string GetCsvString()
            => GetCsvByFileName();
    }
} 