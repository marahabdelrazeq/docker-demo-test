namespace CommonRepo.Domain.Interfaces
{
    /// <summary>
    /// A generic interface that extends the base repository interface to include cache-specific operations.
    /// This interface provides methods for handling caching mechanisms in addition to the standard repository CRUD operations.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity that the repository will handle. Must be a reference type.</typeparam>
    public interface ICacheRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Updates the cache with the specified entity.
        /// </summary>
        /// <param name="T">The entity to update in the cache.</param>
        void UpdateCache(TEntity T);

        void InsertDataCacheAsync(TEntity T);

        void RemoveCache(TEntity T);

        void RemoveCacheAll();
    }
}
