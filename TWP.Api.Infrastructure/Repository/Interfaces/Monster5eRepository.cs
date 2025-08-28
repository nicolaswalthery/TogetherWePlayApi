using Common.ResultPattern;
using Microsoft.EntityFrameworkCore;
using TWP.Api.Core.DbEntities;

namespace TWP.Api.Infrastructure.Repository.Interfaces
{
    public class Monster5eRepository : RepositoryBase<Monster5eDbEntity>
    {
        public Monster5eRepository(DataContext context) : base(context)
        {
        }

        public async Task<Result<List<Monster5eDbEntity>>> GetAllAsync()
            => await Safe.ExecuteAsync(async () =>
            {
                var result = await _context.Monsters.Include(e => e.Traits)
                                                    .Include(e => e.Actions)
                                                    .Include(e => e.Symbarum5e)
                                                    .ToListAsync();
                return Result<List<Monster5eDbEntity>>.Success(result);
            });

    }
}
