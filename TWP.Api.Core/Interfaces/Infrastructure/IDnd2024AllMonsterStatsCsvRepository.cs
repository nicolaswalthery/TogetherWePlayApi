using TWP.Api.Core.DataTransferObjects;

namespace TWP.Api.Infrastructure.CsvRepositories.Interfaces
{
    public interface IDnd2024AllMonsterStatsCsvRepository
    {
        List<Dnd5eMonsterDto> GetAllDnd5e2024MonsterStats();
        public List<Dnd5eMonsterDto> GetAllDnd5e2024MonsterStatsByCr(int cr);
    }
} 