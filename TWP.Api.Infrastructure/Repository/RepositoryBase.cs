using Common.ResultPattern;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using TWP.Api.Core.DbEntities;
using TWP.Api.Infrastructure.Repository.Interfaces;

namespace TWP.Api.Infrastructure.Repository
{
    public abstract class RepositoryBase<TEntity> : IRepositoryBase<TEntity>
       where TEntity : DbEntity
    {
        protected readonly DataContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public RepositoryBase(DataContext context)
        {
            _context = context;
        }

        public virtual async Task<Result> AddAsync(TEntity entity)
            => await Safe.ExecuteAsync(async () =>
            {
                await _context.AddAsync(entity);
                await _context.SaveChangesAsync();
                return Result.Success();
            });

        public virtual async Task<Result> AddAsync(IEnumerable<TEntity> entities)
            => await Safe.ExecuteAsync(async () =>
            {
                await _context.AddRangeAsync(entities);
                await _context.SaveChangesAsync();
                return Result.Success();
            });

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null)
            => await Safe.ExecuteAsync(async () => await _dbSet.CountAsync(predicate));

        public virtual async Task<Result> DeleteAsync(TEntity entityToDelete)
            => await Safe.ExecuteAsync(async () =>
            {
                _context.Remove(entityToDelete);
                await _context.SaveChangesAsync();
                return Result.Success();
            });

        public virtual async Task<Result> DeleteAsync(Guid id)
            => await Safe.ExecuteAsync(async () =>
            {
                var result = await GetByIdAsync(id);
                if (result.IsFailure)
                    return result;
                return await DeleteAsync(result.Data);
            });

        public virtual async Task<Result> DeleteAsync(IEnumerable<TEntity> entities)
            => await Safe.ExecuteAsync(async () =>
            {
                _context.RemoveRange(entities);
                await _context.SaveChangesAsync();
                return Result.Success();
            });

        public virtual async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<Result<IQueryable<TEntity>>> FromSqlAsync(string sql, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<Result<TEntity[]>> GetAllAsync()
            => await Safe.ExecuteAsync(async () =>
            {
                var result = await _dbSet.Select(e => e).ToArrayAsync();
                if (result is null || !result.Any())
                    return Result<TEntity[]>.Failure(error: "No entity found.", ReasonType.NotFound);
                return Result<TEntity[]>.Success(result);
            });

        public virtual async Task<Result<TEntity>> GetByIdAsync(Guid id)
            => await Safe.ExecuteAsync(async () =>
            {
                var result = await _dbSet.FirstOrDefaultAsync(e => e.Id == id);
                if (result is null)
                    return Result<TEntity>.Failure(error: "Entity not found.", ReasonType.NotFound);
                return Result<TEntity>.Success(result);
            });

        public virtual async Task<Result<TEntity[]>> GetByIdsAsync(params object[] keyValues)
            => await Safe.ExecuteAsync(async () =>
            {
                var result = await _dbSet.Where(e => keyValues.Contains(e.Id)).ToArrayAsync();
                if (result is null || !result.Any())
                    return Result<TEntity[]>.Failure(error: "Entities not found.", ReasonType.NotFound);
                return Result<TEntity[]>.Success(result);
            });

        public virtual async Task<Result<TEntity>> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, bool disableTracking = true)
            => await Safe.ExecuteAsync(async () =>
            {
                var result = await _dbSet.FirstOrDefaultAsync(predicate);
                if (result is null)
                    return Result<TEntity>.Failure(error: "Entity not found.", ReasonType.NotFound);
                return Result<TEntity>.Success(result);
            });

        public virtual async Task<Result<IEnumerable<TEntity>>> GetMuliple(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, bool disableTracking = true)
        {
            throw new NotImplementedException();
        }

        public Task<Result<IEnumerable<TEntity>>> GetMulipleAsync(Expression<Func<TEntity, bool>> predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null, bool disableTracking = true)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<Result> UpdateAsync(TEntity entity)
         => await Safe.ExecuteAsync(async () =>
         {
             _context.Update(entity);
             await _context.SaveChangesAsync();
             return Result.Success();
         });

        public virtual async Task<Result> UpdateRangeAsync(IEnumerable<TEntity> entities)
            => await Safe.ExecuteAsync(async () =>
            {
                _context.UpdateRange(entities);
                await _context.SaveChangesAsync();
                return Result.Success();
            });
    }
}