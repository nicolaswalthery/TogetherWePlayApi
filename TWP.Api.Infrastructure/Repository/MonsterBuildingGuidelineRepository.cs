using Common.ResultPattern;
using Microsoft.EntityFrameworkCore;
using TWP.Api.Core.DbEntities;

namespace TWP.Api.Infrastructure.Repository.Interfaces
{
    /// <summary>
    /// Repository implementation for Monster Building Guidelines
    /// </summary>
    public class MonsterBuildingGuidelineRepository : RepositoryBase<MonsterBuildingGuidelineDbEntity>, IMonsterBuildingGuidelineRepository
    {
        public MonsterBuildingGuidelineRepository(DataContext context) : base(context)
        {
        }

        public async Task<Result<MonsterBuildingGuidelineDbEntity>> GetByExpAsync(int exp)
            => await Safe.ExecuteAsync(async () =>
            {
                var guidelines = await _context.MonsterBuildingGuidelines.Where(g => g.Exp <= exp && g.IsActive).ToListAsync();

                if (guidelines == null)
                    return Result<MonsterBuildingGuidelineDbEntity>.Failure($"No guideline found for Exp {exp}", ReasonType.NotFound);
                var result = guidelines.OrderByDescending(gl => gl.Exp).First();
                return Result<MonsterBuildingGuidelineDbEntity>.Success(result);
            });

        public async Task<Result<MonsterBuildingGuidelineDbEntity>> GetByExpAsync(float cr)
        => await Safe.ExecuteAsync(async () =>
            {
                var guideline = await _context.Set<MonsterBuildingGuidelineDbEntity>().FirstOrDefaultAsync(g => g.CRNumeric == cr && g.IsActive);

                if (guideline == null)
                    return Result<MonsterBuildingGuidelineDbEntity>.Failure($"No guideline found for CR {cr}", ReasonType.NotFound);

                return Result<MonsterBuildingGuidelineDbEntity>.Success(guideline);
            });

        public async Task<Result<MonsterBuildingGuidelineDbEntity>> GetByNumericCRAsync(float crNumeric)
        {
            return await Safe.ExecuteAsync(async () =>
            {
                var guideline = await _context.Set<MonsterBuildingGuidelineDbEntity>()
                    .FirstOrDefaultAsync(g => Math.Abs(g.CRNumeric - crNumeric) < 0.001 && g.IsActive);

                if (guideline == null)
                    return Result<MonsterBuildingGuidelineDbEntity>.Failure($"No guideline found for CR value {crNumeric}", ReasonType.NotFound);

                return Result<MonsterBuildingGuidelineDbEntity>.Success(guideline);
            });
        }

        public async Task<Result<List<MonsterBuildingGuidelineDbEntity>>> GetAllActiveAsync()
        {
            return await Safe.ExecuteAsync(async () =>
            {
                var guidelines = await _context.Set<MonsterBuildingGuidelineDbEntity>()
                    .Where(g => g.IsActive)
                    .OrderBy(g => g.CRNumeric)
                    .ToListAsync();

                return Result<List<MonsterBuildingGuidelineDbEntity>>.Success(guidelines);
            });
        }

        public async Task<Result<List<MonsterBuildingGuidelineDbEntity>>> GetRangeAsync(float minCR, float maxCR)
        {
            return await Safe.ExecuteAsync(async () =>
            {
                var guidelines = await _context.Set<MonsterBuildingGuidelineDbEntity>()
                    .Where(g => g.IsActive && g.CRNumeric >= minCR && g.CRNumeric <= maxCR)
                    .OrderBy(g => g.CRNumeric)
                    .ToListAsync();

                return Result<List<MonsterBuildingGuidelineDbEntity>>.Success(guidelines);
            });
        }

        public async Task<Result<MonsterBuildingGuidelineDbEntity>> GetNearestAsync(float crNumeric)
        {
            return await Safe.ExecuteAsync(async () =>
            {
                var guideline = await _context.Set<MonsterBuildingGuidelineDbEntity>()
                    .Where(g => g.IsActive)
                    .OrderBy(g => Math.Abs(g.CRNumeric - crNumeric))
                    .FirstOrDefaultAsync();

                if (guideline == null)
                    return Result<MonsterBuildingGuidelineDbEntity>.Failure("No guidelines found", ReasonType.NotFound);

                return Result<MonsterBuildingGuidelineDbEntity>.Success(guideline);
            });
        }

        public async Task<Result> InsertOrUpdateAsync(MonsterBuildingGuidelineDbEntity guideline)
        {
            return await Safe.ExecuteAsync(async () =>
            {
                var existing = await _context.Set<MonsterBuildingGuidelineDbEntity>()
                    .FirstOrDefaultAsync(g => g.CR == guideline.CR);

                if (existing != null)
                {
                    // Update existing
                    existing.ProficiencyBonus = guideline.ProficiencyBonus;
                    existing.ArmorClass = guideline.ArmorClass;
                    existing.MinHP = guideline.MinHP;
                    existing.MaxHP = guideline.MaxHP;
                    existing.AverageHP = guideline.AverageHP;
                    existing.HPRange = guideline.HPRange;
                    existing.ToFHP = guideline.ToFHP;
                    existing.AttackBonus = guideline.AttackBonus;
                    existing.MultiAttackCount = guideline.MultiAttackCount;
                    existing.AverageDamagePerRound = guideline.AverageDamagePerRound;
                    existing.TotalDamageAvg = guideline.TotalDamageAvg;
                    existing.TotalDamageLegendaryAvg = guideline.TotalDamageLegendaryAvg;
                    existing.DamagePerRound = guideline.DamagePerRound;
                    existing.DamagePerRoundAlt = guideline.DamagePerRoundAlt;
                    existing.SaveDC = guideline.SaveDC;
                    existing.InitiativeBonus = guideline.InitiativeBonus;
                    existing.ExperiencePoints = guideline.ExperiencePoints;
                    existing.ExampleMonsters = guideline.ExampleMonsters;
                    existing.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    // Insert new
                    await _context.Set<MonsterBuildingGuidelineDbEntity>().AddAsync(guideline);
                }

                await _context.SaveChangesAsync();
                return Result.Success();
            });
        }

        public async Task<Result> SeedDefaultGuidelinesAsync()
        {
            return await Safe.ExecuteAsync(async () =>
            {
                // Check if guidelines already exist
                var existingCount = await _context.Set<MonsterBuildingGuidelineDbEntity>().CountAsync();
                if (existingCount > 0)
                {
                    return Result.Success(); // Already seeded
                }

                var guidelines = GetDefaultGuidelines();
                await _context.Set<MonsterBuildingGuidelineDbEntity>().AddRangeAsync(guidelines);
                await _context.SaveChangesAsync();

                return Result.Success();
            });
        }

        private List<MonsterBuildingGuidelineDbEntity> GetDefaultGuidelines()
        {
            return new List<MonsterBuildingGuidelineDbEntity>
            {
                CreateGuideline("0", 0f, 2, 11, 1, 13, 4, "1-13", "0 (0-1)", 2, 0, 1, 0, 6, 3, 0, 11, 1, 10, "Commoner, Frog"),
                CreateGuideline("1/8", 0.125f, 2, 13, 5, 17, 9, "5-17", "9 (7-11)", 3, 0, 4, 6, 7, 8, 5, 11, 1, 25, "Bandit, Giant Rat"),
                CreateGuideline("1/4", 0.25f, 2, 13, 8, 22, 14, "8-22", "13 (10-16)", 4, 2, 6, 7, 8, 9, 11, 11, 2, 50, "Goblin Warrior, Wolf"),
                CreateGuideline("1/2", 0.5f, 2, 13, 11, 33, 20, "11-33", "22 (17-28)", 4, 2, 8, 9, 11, 11, 11, 11, 2, 100, "Black Bear, Hobgoblin Warrior"),
                CreateGuideline("1", 1f, 2, 13, 19, 52, 29, "19-52", "33 (25-41)", 4, 2, 10, 12, 15, 12, 12, 12, 2, 200, "Ghoul, Bugbear Warrior"),
                CreateGuideline("2", 2f, 2, 13, 22, 85, 46, "22-85", "45 (34-56)", 5, 2, 17, 18, 23, 12, 12, 12, 2, 450, "Gelatinous Cube, Ogre"),
                CreateGuideline("3", 3f, 2, 14, 45, 90, 63, "45-90", "65 (49-81)", 5, 2, 23, 24, 30, 12, 12, 12, 2, 700, "Basilisk, Wight"),
                CreateGuideline("4", 4f, 2, 15, 41, 95, 71, "41-95", "84 (64-106)", 6, 2, 28, 30, 38, 13, 13, 13, 2, 1100, "Ettin, Ghost"),
                CreateGuideline("5", 5f, 3, 15, 60, 147, 99, "60-147", "95 (71-119)", 7, 3, 36, 36, 45, 14, 14, 14, 3, 1800, "Troll, Flesh Golem"),
                CreateGuideline("6", 6f, 3, 16, 78, 152, 109, "78-152", "112 (84-140)", 7, 3, 47, 42, 53, 14, 14, 14, 3, 2300, "Chimera, Wyvern"),
                CreateGuideline("7", 7f, 3, 16, 98, 168, 128, "98-168", "130 (98-162)", 8, 3, 51, 48, 60, 15, 15, 15, 3, 2900, "Mind Flayer, Stone Giant"),
                CreateGuideline("8", 8f, 3, 16, 95, 194, 135, "95-194", "136 (102-170)", 8, 3, 57, 54, 68, 15, 15, 15, 4, 3900, "Frost Giant, Hydra"),
                CreateGuideline("9", 9f, 4, 17, 123, 200, 158, "123-200", "145 (109-181)", 9, 3, 60, 60, 75, 16, 16, 16, 4, 5000, "Fire Giant, Treant"),
                CreateGuideline("10", 10f, 4, 17, 136, 229, 171, "136-229", "155 (116-194)", 9, 3, 65, 66, 83, 16, 16, 16, 5, 5900, "Aboleth, Stone Golem"),
                CreateGuideline("11", 11f, 4, 17, 143, 248, 194, "143-248", "165 (124-206)", 10, 3, 80, 72, 90, 17, 17, 17, 5, 7200, "Behir, Remorhaz"),
                CreateGuideline("12", 12f, 4, 17, 169, 240, 190, "169-240", "175 (131-219)", 10, 3, 89, 78, 98, 17, 17, 17, 8, 8400, "Archmage, Erinyes"),
                CreateGuideline("13", 13f, 5, 18, 172, 230, 200, "172-230", "184 (138-230)", 10, 3, 96, 84, 105, 17, 17, 17, 10, 10000, "Beholder, Vampire"),
                CreateGuideline("14", 14f, 5, 18, 184, 228, 201, "184-228", "196 (147-245)", 10, 3, 105, 90, 113, 17, 17, 17, 11, 11500, "Adult Black Dragon, Ice Devil"),
                CreateGuideline("15", 15f, 5, 18, 187, 256, 216, "187-256", "210 (158-263)", 11, 3, 110, 96, 120, 18, 18, 18, 12, 13000, "Mummy Lord, Purple Worm"),
                CreateGuideline("16", 16f, 5, 19, 212, 264, 240, "212-264", "229 (172-286)", 12, 3, 115, 102, 128, 18, 18, 18, 12, 15000, "Marilith, Planetar"),
                CreateGuideline("17", 17f, 6, 19, 198, 356, 265, "198-356", "246 (185-308)", 12, 3, 120, 108, 135, 19, 19, 19, 13, 18000, "Death Knight, Dracolich"),
                CreateGuideline("18", 18f, 6, 20, 180, 180, 180, "180", "266 (200-333)", 12, 3, 130, 114, 143, 19, 19, 19, 14, 20000, "Demilich"),
                CreateGuideline("19", 19f, 6, 20, 300, 300, 300, "300", "285 (214-356)", 14, 3, 140, 120, 150, 20, 20, 20, 14, 22000, "Balor"),
                CreateGuideline("20", 20f, 6, 20, 323, 337, 331, "323-337", "300 (225-375)", 14, 3, 146, 132, 165, 21, 21, 21, 14, 25000, "Pit Fiend"),
                CreateGuideline("21", 21f, 7, 21, 297, 367, 336, "297-367", "325 (244-406)", 15, 3, 160, 144, 180, 22, 22, 22, 15, 33000, "Lich, Solar"),
                CreateGuideline("22", 22f, 7, 21, 370, 507, 431, "370-507", "350 (263-438)", 15, 3, 170, 156, 195, 23, 23, 23, 15, 41000, "Elemental Cataclysm"),
                CreateGuideline("23", 23f, 7, 22, 346, 481, 445, "346-481", "375 (281-469)", 16, 3, 180, 168, 210, 23, 23, 23, 15, 50000, "Kraken"),
                CreateGuideline("24", 24f, 7, 22, 546, 546, 546, "546", "400 (300-500)", 16, 3, 190, 180, 225, 24, 24, 24, 16, 62000, "Ancient Red Dragon"),
                CreateGuideline("25", 25f, 8, 22, 553, 553, 553, "553", "430 (323-538)", 17, 3, 200, 192, 240, 24, 24, 24, 18, 75000, null),
                CreateGuideline("26", 26f, 8, 23, 0, 0, 0, "-", "460 (345-575)", 18, 4, 240, 204, 255, 25, 25, 25, 18, 90000, null),
                CreateGuideline("27", 27f, 8, 23, 0, 0, 0, "-", "490 (368-613)", 18, 4, 258, 216, 270, 25, 25, 25, 17, 105000, null),
                CreateGuideline("28", 28f, 8, 24, 0, 0, 0, "-", "540 (405-675)", 19, 4, 276, 228, 285, 26, 26, 26, 17, 120000, null),
                CreateGuideline("29", 29f, 9, 24, 0, 0, 0, "-", "600 (450-750)", 19, 4, 294, 240, 300, 26, 26, 26, 18, 135000, null),
                CreateGuideline("30", 30f, 9, 25, 697, 697, 697, "697", "666 (500-833)", 19, 4, 312, 252, 315, 26, 26, 26, 18, 155000, "Tarrasque")
            };
        }

        private MonsterBuildingGuidelineDbEntity CreateGuideline(
            string cr, float crNumeric, int profBonus, int ac,
            int minHp, int maxHp, int avgHp, string hpRange, string tofHp,
            int atkBonus, int multiAtk, int avgDmg, int tdAvg, int tdLegAvg,
            int dmgRound, int dmgRoundAlt, int saveDc, int init, int xp,
            string? examples)
        {
            return new MonsterBuildingGuidelineDbEntity
            {
                Id = Guid.NewGuid(),
                CR = cr,
                CRNumeric = crNumeric,
                ProficiencyBonus = profBonus,
                ArmorClass = ac,
                MinHP = minHp,
                MaxHP = maxHp,
                AverageHP = avgHp,
                HPRange = hpRange,
                ToFHP = tofHp,
                AttackBonus = atkBonus,
                MultiAttackCount = multiAtk,
                AverageDamagePerRound = avgDmg,
                TotalDamageAvg = tdAvg,
                TotalDamageLegendaryAvg = tdLegAvg,
                DamagePerRound = dmgRound,
                DamagePerRoundAlt = dmgRoundAlt,
                SaveDC = saveDc,
                InitiativeBonus = init,
                ExperiencePoints = xp,
                ExampleMonsters = examples,
                IsOfficial = true,
                Source = "D&D 5e 2024 - Alphastream.org",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
        }
    }
}