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
            => GetAllDnd5e2024MonsterStats().Where(m => ParseCrToDouble(m.CR) <= cr).ToList();

        private double ParseCrToDouble(string cr)
        {
            if (double.TryParse(cr, out var result))
                return result;

            var parts = cr.Split('/');
            if (parts.Length == 2 &&
                double.TryParse(parts[0], out var num) &&
                double.TryParse(parts[1], out var den) &&
                den != 0)
            {
                return num / den;
            }

            return 0;
        }

    }
} 