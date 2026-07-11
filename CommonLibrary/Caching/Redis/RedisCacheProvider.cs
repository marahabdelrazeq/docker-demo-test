using Redis.OM;
using Redis.OM.Contracts;
using Redis.OM.Searching;

namespace CommonLibrary.Caching.Redis
{
    public class RedisCacheProvider<T> where T : class
    {
        private readonly IRedisConnection _connection;
        private readonly RedisConnectionProvider _provider;
        private readonly IRedisCollection<T> _collection;

        /// <summary>
        /// Initializes the RedisCacheProvider with a connection to Redis.
        /// </summary>
        /// <param name="connectIp">The Redis connection IP address.</param>
        public RedisCacheProvider(string connectIp)
        {
            _provider = CreateProvider(connectIp);
            _connection = _provider.Connection;
            _collection = InitializeCollection();
        }

        /// <summary>
        /// Creates and returns a RedisConnectionProvider based on the provided IP address.
        /// </summary>
        /// <param name="connectIp">The Redis connection IP address.</param>
        /// <returns>A RedisConnectionProvider instance.</returns>
        public RedisConnectionProvider CreateProvider(string connectIp)
        {
            return new RedisConnectionProvider($"redis://{connectIp}");
        }

        /// <summary>
        /// Sets the Redis index (can be extended for actual index creation).
        /// </summary>
        public bool SetIndex()
        {
            // You can implement the logic for creating indexes on Redis fields here.
            return true;
        }

        /// <summary>
        /// Retrieves the Redis collection of type T.
        /// </summary>
        public IRedisCollection<T> GetCollection()
        {
            return _collection;
        }

        /// <summary>
        /// Initializes and sets the Redis collection of type T.
        /// </summary>
        private IRedisCollection<T> InitializeCollection()
        {
            return _provider.RedisCollection<T>();
        }

        /// <summary>
        /// Inserts an object of type T into the Redis collection.
        /// </summary>
        /// <param name="obj">The object to be inserted into the Redis collection.</param>
        public async Task InsertToCollectionAsync(T obj)
        {
            var collection = GetCollection();
            await collection.InsertAsync(obj);
        }

        /// <summary>
        /// Retrieves a value by prefix and ID from the Redis collection asynchronously.
        /// </summary>
        /// <param name="prefix">The prefix for the ID.</param>
        /// <param name="id">The ID of the object.</param>
        /// <returns>The object found or null if not found.</returns>
        public async Task<T> GetValueAsync(string prefix, long id)
        {
            var collection = GetCollection();
            return await collection.FindByIdAsync($"{prefix}:{id}");
        }
    }
}
