using CommonLibrary.Pagination;
using CommonLibrary.RequestInformation;
using CommonRepo.Persistence.Context;
 
using CommonRepo.Domain.Interfaces;
using CommonRepo.Persistence.Context;
using FilterBuilder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using CommonRepo.Domain.Interfaces;

namespace CommonRepo.Infrastructure.Repositories
{
    /// <summary>
    /// A generic repository class that provides common data access functionality for entities in a database. 
    /// This repository encapsulates various CRUD operations, pagination, filtering, tracking, ownership handling, 
    /// and transactional operations, with centralized exception handling using `SystemException`.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity that the repository will handle. Must be a reference type.</typeparam>

    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected ApplicationDbContext _context;
        private readonly IRequestInfoService _requestInfoService;
        private readonly string _failureMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{TEntity}"/> class.
        /// </summary>
        /// <param name="context">The DbContext used for accessing the database.</param>
        /// <param name="requestInfoService">A service that provides information about the current request, used for filtering entities based on ownership.</param>
        /// <param name="failureMessage">A custom error message used in exception handling. If not provided, a default message will be used.</param>
        public Repository(ApplicationDbContext context, IRequestInfoService requestInfoService, string failureMessage = null)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _requestInfoService = requestInfoService ?? throw new ArgumentNullException(nameof(requestInfoService));
            _failureMessage = failureMessage ?? "An error occurred during the operation.";
        }

        /// <summary>
        /// Retrieves a single entity that matches the given filter asynchronously.
        /// </summary>
        /// <param name="filter">A lambda expression used to filter the entity.</param>
        /// <param name="tracked">Indicates whether to track the entity or not.</param>
        /// <param name="includeProperties">A comma-separated list of related entities to include in the query.</param>
        /// <param name="orderBy">A string representing the order of the results, defaulting to "Id DESC".</param>
        /// <returns>A task that represents the asynchronous operation, containing the matching entity or null if no match is found.</returns>
        public virtual async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter, bool tracked = true, string includeProperties = null, string orderBy = "Id DESC")
        {
            return await ExecuteWithSystemException(async () =>
            {
                IQueryable<TEntity> query = _context.Set<TEntity>();

                // Apply tracking, include properties, and request information filters
                query = ApplyTracking(query, tracked);
                query = IncludeRelatedEntities(query, includeProperties);
                query = ApplyOwnershipFilter(query, filter);

                // Apply ordering and return the result
                return await query.OrderBy(orderBy).FirstOrDefaultAsync();
            });
        }

        /// <summary>
        /// Retrieves all entities that match the given filter asynchronously.
        /// </summary>
        /// <param name="filter">A lambda expression used to filter the entities. If null, all entities are returned.</param>
        /// <returns>An enumerable list of matching entities.</returns>
        public virtual IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter = null)
        {
            return ExecuteWithSystemException(() =>
            {
                IQueryable<TEntity> query = _context.Set<TEntity>();
                query = ApplyOwnershipFilter(query, filter);
                return query.ToList();
            });
        }

        /// <summary>
        /// Retrieves all entities that match the given filter asynchronously.
        /// </summary>
        /// <param name="filter">A lambda expression used to filter the entities. If null, all entities are returned.</param>
        /// <returns>A task that represents the asynchronous operation, containing an enumerable list of matching entities.</returns>
        public virtual async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            return await ExecuteWithSystemException(async () =>
            {
                var query = GetAllQueryAsync(filter);
                return await query.ToListAsync();
            });
        }

        /// <summary>
        /// Retrieves a paginated and filtered list of entities with total record count, optionally including related entities and sorting.
        /// </summary>
        /// <typeparam name="TF">The type used for filtering data.</typeparam>
        /// <param name="objDTO">The filter data object.</param>
        /// <param name="includeProperties">A comma-separated list of related entities to include in the query.</param>
        /// <param name="pageSize">The number of entities to retrieve per page.</param>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="orderBy">A string representing the order of the results, defaulting to "Id DESC".</param>
        /// <returns>A task representing the asynchronous operation, containing a tuple with a list of entities and the total count.</returns>
        public virtual async Task<(List<TEntity>, int)> GetAllAsync<TF>(TF objDTO = null, string includeProperties = null, int pageSize = 0, int pageNumber = 1, string orderBy = "Id DESC") where TF : class
        {
            return await ExecuteWithSystemException(async () =>
            {
                var filter = FilterExpression.Build<TEntity, TF>(new[] { objDTO })!;
                return await GetAllAsync(filter, includeProperties, pageSize, pageNumber, orderBy);
            });
        }

        /// <summary>
        /// Retrieves a paginated and filtered list of entities with total record count, optionally including related entities and sorting.
        /// </summary>
        /// <param name="filter">A lambda expression used to filter the entities. If null, all entities are returned.</param>
        /// <param name="includeProperties">A comma-separated list of related entities to include in the query.</param>
        /// <param name="pageSize">The number of entities to retrieve per page.</param>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="orderBy">A string representing the order of the results, defaulting to "Id DESC".</param>
        /// <returns>A task representing the asynchronous operation, containing a tuple with a list of entities and the total count.</returns>
        public virtual async Task<(List<TEntity>, int)> GetAllAsync(Expression<Func<TEntity, bool>> filter = null, string includeProperties = null, int pageSize = 0, int pageNumber = 1, string orderBy = "Id DESC")
        {
            return await ExecuteWithSystemException(async () =>
            {
                var query = GetAllQueryAsync(filter).OrderBy(new ParsingConfig(), orderBy).AsQueryable();
                var totalRecord = query.Count();

                if (pageSize > 0)
                {
                    if (pageSize > 100) pageSize = 100;
                    query = query.Skip(pageSize * (pageNumber - 1)).Take(pageSize);
                }

                query = IncludeRelatedEntities(query, includeProperties);
                return (await query.ToListAsync(), totalRecord);
            });
        }

        /// <summary>
        /// Retrieves a paginated and filtered list of entities with total record count, optionally including related entities and sorting.
        /// </summary>
        /// <param name="filter">A lambda expression used to filter the entities. If null, all entities are returned.</param>
        /// <param name="includeProperties">A comma-separated list of related entities to include in the query.</param>
        /// <param name="pageSize">The number of entities to retrieve per page.</param>
        /// <param name="pageNumber">The page number to retrieve.</param>
        /// <param name="orderBy">A string representing the order of the results, defaulting to "Id DESC".</param>
        /// <returns>A task representing the asynchronous operation, containing a tuple with a list of entities and the total count.</returns>
        public virtual async Task<(List<TEntity>, int)> GetAllAsync<TF>(GridDataFilter<TF> gridDataFilter, string includeProperties = null) where TF : class
        {
            return await GetAllAsync(gridDataFilter.FilterData, includeProperties, gridDataFilter.PageSize, gridDataFilter.PageIndex, gridDataFilter.Sort);
        }

        /// <summary>
        /// Adds a new entity asynchronously to the database.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A task that represents the asynchronous operation, containing the added entity.</returns>
        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            return await ExecuteWithSystemException(async () =>
            {
                var result = await _context.Set<TEntity>().AddAsync(entity);
                await SaveAsync();
                return result.Entity;
            });
        }

        /// <summary>
        /// Adds multiple entities asynchronously to the database.
        /// </summary>
        /// <param name="entities">A collection of entities to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await ExecuteWithSystemException(async () =>
            {
                await _context.Set<TEntity>().AddRangeAsync(entities);
                await SaveAsync();
            });
        }

        /// <summary>
        /// Updates an existing entity asynchronously in the database.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        /// <returns>A task that represents the asynchronous operation, containing the updated entity.</returns>
        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            return await ExecuteWithSystemException(async () =>
            {
                _context.Update(entity);
                await SaveAsync();
                return entity;
            });
        }

        /// <summary>
        /// Deletes entities that match the specified predicate.
        /// </summary>
        /// <param name="predicate">A lambda expression to filter the entities to delete.</param>
        /// <returns>The number of entities deleted.</returns>
        public virtual async Task<int> BulkDelete(Expression<Func<TEntity, bool>> predicate)
        {
            return await ExecuteWithSystemException(async () =>
            {
                return await _context.Set<TEntity>().Where(predicate).ExecuteDeleteAsync();
            });
        }

        /// <summary>
        /// Updates entities that match the specified predicate using the provided update expression.
        /// </summary>
        /// <param name="predicate">A lambda expression to filter the entities to update.</param>
        /// <param name="updateExpression">An expression that defines the properties to update.</param>
        /// <returns>The number of entities updated.</returns>

        public virtual async Task<int> BulkUpdate(Expression<Func<TEntity, bool>> predicate, Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> expression)
        {
            return await ExecuteWithSystemException(async () =>
            {
                return await _context.Set<TEntity>().Where(predicate).ExecuteUpdateAsync(expression);
            });
        }


        /// <summary>
        /// Removes an existing entity asynchronously from the database.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public virtual async Task RemoveAsync(TEntity entity)
        {
            await ExecuteWithSystemException(async () =>
            {
                _context.Set<TEntity>().Remove(entity);
                await SaveAsync();
            });
        }

        /// <summary>
        /// Removes an existing entity asynchronously from the database by id.
        /// </summary>
        /// <param name="id">The entity id to remove.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public virtual async Task RemoveAsync(int id)
        {
            await ExecuteWithSystemException(async () =>
            {
                var entity = await _context.Set<TEntity>().FindAsync(id);
                if (entity != null)
                {
                    await RemoveAsync(entity);
                }
            });
        }

        /// <summary>
        /// Checks whether an entity that matches the given predicate exists asynchronously.
        /// </summary>
        /// <param name="predicate">A lambda expression used to filter the entities.</param>
        /// <returns>A task that represents the asynchronous operation, containing the entity if found, or null if not.</returns>
        public virtual async Task<TEntity> ExistsAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await ExecuteWithSystemException(async () =>
            {
                return await _context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(predicate);
            });
        }

        /// <summary>
        /// Counts the number of entities that match the given predicate asynchronously.
        /// </summary>
        /// <param name="predicate">A lambda expression used to filter the entities.</param>
        /// <returns>A task that represents the asynchronous operation, containing the count of matching entities.</returns>
        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await ExecuteWithSystemException(async () =>
            {
                return await _context.Set<TEntity>().CountAsync(predicate);
            });
        }


        /// <summary>
        /// Checks asynchronously if any entities match the given predicate.
        /// </summary>
        /// <param name="predicate">A lambda expression used to filter the entities.</param>
        /// <returns>A task that represents the asynchronous operation, containing true if any entities match the predicate, otherwise false.</returns>
        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await ExecuteWithSystemException(async () =>
            {
                return await _context.Set<TEntity>().AnyAsync(predicate);
            });
        }

        /// <summary>
        /// Checks asynchronously if all entities match the given predicate.
        /// </summary>
        /// <param name="predicate">A lambda expression used to filter the entities.</param>
        /// <returns>A task that represents the asynchronous operation, containing true if all entities match the predicate, otherwise false.</returns>
        public virtual async Task<bool> AllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await ExecuteWithSystemException(async () =>
            {
                return await _context.Set<TEntity>().AllAsync(predicate);
            });
        }

        /// <summary>
        /// Executes multiple operations within a transaction.
        /// </summary>
        /// <param name="operation">The operations to execute within the transaction.</param>
        public virtual async Task ExecuteInTransactionAsync(Func<Task> operation)
        {
            await ExecuteWithSystemException(async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    await operation();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            });
        }

        /// <summary>
        /// Saves the changes asynchronously to the database.
        /// </summary>
        public virtual async Task SaveAsync()
        {
            await ExecuteWithSystemException(async () =>
            {
                await _context.SaveChangesAsync();
            });
        }

        public virtual async Task<long> GetNextSequenceValueAsync(string sequenceName)
        {
            return await ExecuteWithSystemException(async () =>
            {

                return await _context.GetNextSequenceValueAsync(sequenceName);


            });
        }

        #region  Helper functions
        private IQueryable<TEntity> ApplyTracking(IQueryable<TEntity> query, bool tracked)
        {
            return tracked ? query : query.AsNoTracking();
        }

        private IQueryable<TEntity> IncludeRelatedEntities(IQueryable<TEntity> query, string includeProperties)
        {
            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query;
        }

        private IQueryable<TEntity> ApplyOwnershipFilter(IQueryable<TEntity> query, Expression<Func<TEntity, bool>> filter)
        {
            filter ??= x => true;
            var requestInfo = _requestInfoService.GetRequestInfo();
            filter = filter?.ApplyOwnership(requestInfo) ?? (x => true);
            return query.Where(filter);
        }

        private IQueryable<TEntity> GetAllQueryAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = _context.Set<TEntity>();
            query = ApplyOwnershipFilter(query, filter);
            return query;
        }
        #endregion

        #region  Centralized exception handling
        protected virtual async Task<T> ExecuteWithSystemException<T>(Func<Task<T>> action)
        {
            try
            {
                return await action();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.InnerException?.Message ?? ex.Message, ex);
            }
        }

        protected virtual T ExecuteWithSystemException<T>(Func<T> action)
        {
            try
            {
                return action();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.InnerException?.Message ?? ex.Message, ex);
            }
        }

        protected virtual async Task ExecuteWithSystemException(Func<Task> action)
        {
            try
            {
                await action();
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.InnerException?.Message ?? ex.Message, ex);
            }
        }
        #endregion
    }
}
