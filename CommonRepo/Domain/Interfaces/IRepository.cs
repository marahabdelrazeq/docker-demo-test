
using CommonLibrary.Pagination;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace CommonRepo.Domain.Interfaces
{
    /// <summary>
    /// Defines a generic repository interface for performing common data operations.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface IRepository<TEntity> where TEntity : class
    {
        // Basic Queries

        /// <summary>
        /// Retrieves all entities that match the specified filter.
        /// </summary>
        /// <param name="filter">The filter expression to apply. If null, all entities are returned.</param>
        /// <returns>An enumerable collection of entities.</returns>
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter = null);

        /// <summary>
        /// Retrieves a single entity that matches the specified filter.
        /// </summary>
        /// <param name="filter">The filter expression to apply.</param>
        /// <param name="tracked">Specifies whether to track the entity in the context.</param>
        /// <param name="includeProperties">Comma-separated list of related entities to include in the result.</param>
        /// <param name="orderBy">The order in which to return the entities. Defaults to "Id DESC".</param>
        /// <returns>The entity that matches the filter, or null if no match is found.</returns>
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter, bool tracked = true, string includeProperties = null, string orderBy = "Id DESC");

        // Advanced Queries

        /// <summary>
        /// Retrieves a paginated list of entities along with the total count based on the specified filter and sorting options.
        /// </summary>
        /// <typeparam name="TF">The type of the filter object.</typeparam>
        /// <param name="objDTO">An object used for filtering the data.</param>
        /// <param name="includeProperties">Comma-separated list of related entities to include in the result.</param>
        /// <param name="pageSize">The number of records per page.</param>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="orderBy">The order in which to return the entities. Defaults to "Id DESC".</param>
        /// <returns>A tuple containing the list of entities and the total count.</returns>
        Task<(List<TEntity>, int)> GetAllAsync<TF>(TF objDTO = null, string includeProperties = null, int pageSize = 0, int pageNumber = 1, string orderBy = "Id DESC") where TF : class;

        /// <summary>
        /// Retrieves a paginated list of entities based on grid filter data and sorting options.
        /// </summary>
        /// <typeparam name="TF">The type of the filter object.</typeparam>
        /// <param name="gridDataFilter">A filter object used for pagination, sorting, and filtering data.</param>
        /// <param name="includeProperties">Comma-separated list of related entities to include in the result.</param>
        /// <returns>A tuple containing the list of entities and the total count.</returns>
        Task<(List<TEntity>, int)> GetAllAsync<TF>(GridDataFilter<TF> gridDataFilter, string includeProperties = null) where TF : class;

        /// <summary>
        /// Retrieves a paginated list of entities that match the specified filter and sorting options.
        /// </summary>
        /// <param name="filter">The filter expression to apply.</param>
        /// <param name="includeProperties">Comma-separated list of related entities to include in the result.</param>
        /// <param name="pageSize">The number of records per page.</param>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="orderBy">The order in which to return the entities. Defaults to "Id DESC".</param>
        /// <returns>A tuple containing the list of entities and the total count.</returns>
        Task<(List<TEntity>, int)> GetAllAsync(Expression<Func<TEntity, bool>> filter = null, string includeProperties = null, int pageSize = 0, int pageNumber = 1, string orderBy = "Id DESC");

        // CRUD Operations

        /// <summary>
        /// Adds a new entity to the database.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>The added entity.</returns>
        Task<TEntity> AddAsync(TEntity entity);

        /// <summary>
        /// Adds a range of entities to the database.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        Task AddRangeAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Updates an existing entity in the database.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>The updated entity.</returns>
        Task<TEntity> UpdateAsync(TEntity entity);

        /// <summary>
        /// Updates entities that match the specified predicate using the provided update expression.
        /// </summary>
        /// <param name="predicate">A lambda expression to filter the entities to update.</param>
        /// <param name="updateExpression">An expression that defines the properties to update.</param>
        /// <returns>The number of entities updated.</returns>
        Task<int> BulkUpdate(Expression<Func<TEntity, bool>> predicate, Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> expression);

        /// <summary>
        /// Adds multiple entities asynchronously to the database.
        /// </summary>
        /// <param name="entities">A collection of entities to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task<int> BulkDelete(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Removes an entity from the database.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        Task RemoveAsync(TEntity entity);

        /// <summary>
        /// Removes an entity from the database by its identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to remove.</param>
        Task RemoveAsync(int id);

        // Counting & Checking Existence

        /// <summary>
        /// Counts the number of entities that match the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match against.</param>
        /// <returns>The number of matching entities.</returns>
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Checks if any entity matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match against.</param>
        /// <returns>True if any entities match the predicate, otherwise false.</returns>
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Checks if all entities match the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match against.</param>
        /// <returns>True if all entities match the predicate, otherwise false.</returns>
        Task<bool> AllAsync(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Retrieves the first entity that matches the specified predicate.
        /// </summary>
        /// <param name="func">The predicate to match against.</param>
        /// <returns>The matching entity, or null if no match is found.</returns>
        Task<TEntity> ExistsAsync(Expression<Func<TEntity, bool>> func);

        // Transactions and Saving

        /// <summary>
        /// Executes multiple operations within a transaction.
        /// </summary>
        /// <param name="operation">The operations to execute within the transaction.</param>
        Task ExecuteInTransactionAsync(Func<Task> operation);

        /// <summary>
        /// Saves changes to the database.
        /// </summary>
        Task SaveAsync();

        // Sequence

        /// <summary>
        /// Retrieves the next value from the specified database sequence.
        /// </summary>
        /// <param name="sequenceName">The name of the sequence.</param>
        /// <returns>The next value in the sequence.</returns>
        Task<long> GetNextSequenceValueAsync(string sequenceName);
    }
}
