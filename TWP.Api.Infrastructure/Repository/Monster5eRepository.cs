using Common.ResultPattern;
using Microsoft.EntityFrameworkCore;
using TWP.Api.Core.DbEntities;

namespace TWP.Api.Infrastructure.Repository.Interfaces
{
    public class Monster5eRepository : RepositoryBase<Monster5eDbEntity>, IMonster5eRepository
    {
        public Monster5eRepository(DataContext context) : base(context)
        {
        }

        public async Task<Result<List<Monster5eDbEntity>>> FindByCrOrLessAsync(int challengeRating)
            => await Safe.ExecuteAsync(async () =>
            {
                var result = await _context.Monsters.Where(m => m.Cr <= challengeRating).Include(e => e.Traits)
                                                    .Include(e => e.Actions)
                                                    .Include(e => e.Symbarum5e)
                                                    .ToListAsync();
                return Result<List<Monster5eDbEntity>>.Success(result);
            });

        public async Task<Result<List<Monster5eDbEntity>>> FindByCrAsync(float challengeRating)
            => await Safe.ExecuteAsync(async () =>
            {
                var result = await _context.Monsters.Where(m => m.Cr == challengeRating).Include(e => e.Traits)
                                                    .Include(e => e.Actions)
                                                    .Include(e => e.Symbarum5e)
                                                    .ToListAsync();
                return Result<List<Monster5eDbEntity>>.Success(result);
            });

        public async Task<Result<List<Monster5eDbEntity>>> GetAllAsync()
            => await Safe.ExecuteAsync(async () =>
            {
                var result = await _context.Monsters.Include(e => e.Traits)
                                                    .Include(e => e.Actions)
                                                    .Include(e => e.Symbarum5e)
                                                    .ToListAsync();
                return Result<List<Monster5eDbEntity>>.Success(result);
            });

        public async Task<Result> Insert(Monster5eDbEntity monster5eDbEntity)
            => await Safe.ExecuteAsync(async () =>
            {
                base._context.Add(monster5eDbEntity);
                await base._context.SaveChangesAsync();
                return Result.Success();
            });

        public async Task<Result> InsertMany(List<Monster5eDbEntity> monster5EDbEntities)
            => await Safe.ExecuteAsync(async () =>
            {
                await base._context.AddRangeAsync(monster5EDbEntities);
                await base._context.SaveChangesAsync();
                return Result.Success();
            });

        public async Task<Result> UpdateMany(List<Monster5eDbEntity> monster5EDbEntities)
            => await Safe.ExecuteAsync(async () =>
            {
                await base.UpdateRangeAsync(monster5EDbEntities);
                return Result.Success();
            });

        public async Task<Result> Update(Monster5eDbEntity monster5e)
            => await Safe.ExecuteAsync(async () =>
            {
                await base.UpdateAsync(monster5e);
                return Result.Success();
            });

        public async Task<Result<List<TraitDbEntity>>> GetAllTraits(List<TraitDbEntity> monster5EDbEntities)
            => await Safe.ExecuteAsync(async () =>
            {
                var result = await _context.Monsters.Include(e => e.Traits).ToListAsync();
                var traits = result.SelectMany(m => m.Traits).ToList();
                return Result<List<TraitDbEntity>>.Success(traits);
            });

        public async Task<Result<List<ActionDbEntity>>> GetAllActions(List<TraitDbEntity> monster5EDbEntities)
            => await Safe.ExecuteAsync(async () =>
            {
                var result = await _context.Monsters.Include(e => e.Actions).ToListAsync();
                var actions = result.SelectMany(m => m.Actions).ToList();
                return Result<List<ActionDbEntity>>.Success(actions);
            });

        public async Task<Result<Monster5eDbEntity>> GetByNameAsync(string monsterName)
            => await Safe.ExecuteAsync(async () =>
            {
                var result = await _context.Monsters.Include(e => e.Traits)
                                                    .Include(e => e.Actions)
                                                    .Include(e => e.Symbarum5e)
                                                    .FirstOrDefaultAsync(m => m.Name.ToLower() == monsterName.ToLower());
                return Result<Monster5eDbEntity>.Success(result);
            });
    }
}
