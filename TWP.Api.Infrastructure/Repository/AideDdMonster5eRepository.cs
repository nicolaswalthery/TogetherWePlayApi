using Common.Helpers.DndHelpers;
using Common.ResultPattern;
using Microsoft.EntityFrameworkCore;
using TWP.Api.Core.DbEntities;

namespace TWP.Api.Infrastructure.Repository.Interfaces
{
    public class AideDdMonster5eRepository : RepositoryBase<MonsterAideddDbEntity>, IAideDdMonster5eRepository
    {
        public AideDdMonster5eRepository(DataContext context) : base(context)
        {
        }

        public async Task<Result<List<MonsterAideddDbEntity>>> GetAllAsync()
            => await Safe.ExecuteAsync(async () =>
            {
                var result = await _context.AideddMonsters.ToListAsync();
                return Result<List<MonsterAideddDbEntity>>.Success(result);
            });

        public async Task<Result<List<MonsterAideddDbEntity>>> FindByCrOrLessAsync(double cr)
            => await Safe.ExecuteAsync(async () =>
            {

                var result = await _context.AideddMonsters.ToListAsync();
                var filtered = result.Where(m =>
                {
                    try
                    {
                        var monsterCr = m.CR.ConvertToDoubleChallengeRating();
                        return monsterCr <= cr;
                    }
                    catch
                    {
                        return false;
                    }
                }).ToList();
                return Result<List<MonsterAideddDbEntity>>.Success(filtered);
            });

    }
}
