using Common.ResultPattern;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using TWP.Api.Core.DbEntities;

namespace TWP.Api.Core.Interface.Infrastructure
{
    public interface IRepositoryBase<TEntity> where TEntity : DbEntity
    {
        #region CREATE
        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        Task<Result> AddAsync(TEntity entity);
        /// <summary>
        /// Inserts a range of entities.
        /// </summary>
        /// <param name="entities">The entities to insert.</param>
        Task<Result> AddAsync(IEnumerable<TEntity> entities);
        #endregion
        #region READ
        /// <summary>
        /// Finds an entity with the given primary key values.
        /// </summary>
        /// <param name="keyValues">The values of the primary key.</param>
        /// <returns>The found entity or null.</returns>
        Task<Result<TEntity[]>> GetByIdsAsync(params object[] keyValues);

        /// <summary>
        /// Get one entity by its identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Result<TEntity>> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets the first or default entity based on a predicate, orderby and children inclusions.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="include">Navigation properties separated by a comma.</param>
        /// <param name="disableTracking">A boolean to disable entities changing tracking.</param>
        /// <returns>The first element satisfying the condition.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        Task<Result<TEntity>> GetFirstOrDefaultAsync(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true
        );
        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <returns>The all dataset.</returns>
        Task<Result<TEntity[]>> GetAllAsync();
        /// <summary>
        /// Gets the entities based on a predicate, orderby and children inclusions.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="include">A function to include navigation properties</param>
        /// <param name="disableTracking">A boolean to disable entities changing tracking.</param>
        /// <returns>A list of elements satisfying the condition.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        Task<Result<IEnumerable<TEntity>>> GetMulipleAsync(
            Expression<Func<TEntity, bool>> predicate = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> include = null,
            bool disableTracking = true
        );
        /// <summary>
        /// Uses raw SQL queries to fetch the specified entity data.
        /// </summary>
        /// <param name="sql">The raw SQL.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A list of elements satisfying the condition specified by raw SQL.</returns>
        Task<Result<IQueryable<TEntity>>> FromSqlAsync(string sql, params object[] parameters);
        #endregion
        #region UPDATE
        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">The entity.</param>
        Task<Result> UpdateAsync(TEntity entity);
        /// <summary>
        /// Updates the specified entities.
        /// </summary>
        /// <param name="entities">The entities.</param>
        Task<Result> UpdateRangeAsync(IEnumerable<TEntity> entities);
        #endregion
        #region DELETE
        /// <summary>
        /// Deletes the entity by the specified primary key.
        /// </summary>
        /// <param name="id">The primary key value.</param>
        Task<Result> DeleteAsync(Guid id);
        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        Task<Result> DeleteAsync(TEntity entityToDelete);
        /// <summary>
        /// Deletes the specified entities.
        /// </summary>
        /// <param name="entities">The entities to delete.</param>
        Task<Result> DeleteAsync(IEnumerable<TEntity> entities);
        #endregion
        #region OTHER
        /// <summary>
        /// Gets the count based on a predicate.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>The number of rows.</returns>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate = null);
        /// <summary>
        /// Check if an element exists for a condition.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A boolean</returns>
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
        #endregion
    }
}
