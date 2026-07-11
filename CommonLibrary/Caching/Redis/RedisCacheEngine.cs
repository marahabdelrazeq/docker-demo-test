using StackExchange.Redis;
using System.Net;
using System.Text.Json;

namespace CommonLibrary.Caching.Redis
{
    public class RedisCacheEngine
    {
        private readonly ConnectionMultiplexer _db;
        private readonly RedisConnectorHelper _cacheConnector;

        /// <summary>
        /// Connect with the caching engine (Redis)
        /// </summary>
        /// <param name="connectIP">Connection IP for Redis</param>
        /// <param name="cacheType">Caching type, defaults to Redis</param>
        public RedisCacheEngine(string connectIP, string cacheType = "Redis")
        {
            _cacheConnector = new RedisConnectorHelper(connectIP, cacheType);
            _db = _cacheConnector.Connection;
        }

        /// <summary>
        /// Get data from Redis cache.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="dbNumber">Database number (default is -1).</param>
        /// <returns>The cached value or null if not found.</returns>
        public async Task<string> GetDataAsync(string key, int dbNumber = -1)
        {
            try
            {
                key = key.ToUpper();
                var db = _db.GetDatabase(dbNumber);
                return await db.StringGetAsync(key);
            }
            catch (Exception ex)
            {
                // Log and handle the exception appropriately
                throw new Exception($"Failed to get data for key: {key}", ex);
            }
        }

        /// <summary>
        /// Set key-value pair in Redis cache.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="value">The value to cache.</param>
        /// <param name="dbNumber">Database number (default is -1).</param>
        /// <returns>True if the cache was set successfully, otherwise false.</returns>
        public async Task<bool> SetDataAsync(string key, object value, int dbNumber = -1)
        {
            key = key.ToUpper();
            var db = _db.GetDatabase(dbNumber);
            return await db.StringSetAsync(key, JsonSerializer.Serialize(value));
        }

        /// <summary>
        /// Remove a key from Redis cache.
        /// </summary>
        /// <param name="key">The cache key.</param>
        /// <param name="dbNumber">Database number (default is -1).</param>
        /// <returns>True if the key was removed, otherwise false.</returns>
        public async Task<bool> RemoveDataAsync(string key, int dbNumber = -1)
        {
            key = key.ToUpper();
            var db = _db.GetDatabase(dbNumber);
            return await db.KeyDeleteAsync(key);
        }

        /// <summary>
        /// Flush all Redis databases.
        /// </summary>
        public bool FlushAllDatabases(string connectIP)
        {
            var server = _db.GetServer(connectIP);
            server.FlushAllDatabases();
            return true;
        }

        /// <summary>
        /// Flush a specific Redis database.
        /// </summary>
        public bool FlushDatabase(string connectIP, int dbNumber = -1)
        {
            var server = _db.GetServer(connectIP);
            server.FlushDatabase(dbNumber);
            return true;
        }

        /// <summary>
        /// Get all keys from a Redis database.
        /// </summary>
        public List<RedisKey> GetAllKeys(string connectIP, int dbNumber = -1)
        {
            return _db.GetServer(connectIP).Keys(dbNumber).ToList();
        }

        /// <summary>
        /// Get all keys and their values from a Redis database.
        /// </summary>
        public List<string> GetAllValues(int dbNumber = -1)
        {
            EndPoint endPoint = _db.GetEndPoints().First();
            RedisKey[] keys = _db.GetServer(endPoint).Keys(database: dbNumber).ToArray();
            List<string> result = new();
            foreach (RedisKey key in keys)
            {
                result.Add(key.ToString());
            }
            return result;
        }

        /// <summary>
        /// Check if a key exists in Redis cache.
        /// </summary>
        public async Task<bool> KeyExistsAsync(string key, int dbNumber = -1)
        {
            key = key.ToUpper();
            var db = _db.GetDatabase(dbNumber);
            return await db.KeyExistsAsync(key);
        }

        /// <summary>
        /// Get keys based on a pattern (lookup) from Redis.
        /// </summary>
        public List<string> GetKeysByPattern(int dbNumber = -1, string lookupName = "")
        {
            EndPoint endPoint = _db.GetEndPoints().First();
            var lookupPattern = "*" + lookupName + "*";
            RedisKey[] keys = _db.GetServer(endPoint).Keys(database: dbNumber, pattern: lookupPattern).ToArray();
            return keys.Select(key => key.ToString()).ToList();
        }

        /// <summary>
        /// Get the Redis database instance.
        /// </summary>
        public IDatabase GetDatabase(int dbNumber = -1)
        {
            return _db.GetDatabase(dbNumber);
        }

        /// <summary>
        /// Perform a geo-spatial search using Redis.
        /// </summary>
        public List<GeoRadiusResult> GetGeoSearch(RedisKey redisKey, double longitude, double latitude, double distance, int dbNumber = -1)
        {
            var db = GetDatabase(dbNumber);
            return db.GeoRadius(redisKey, longitude, latitude, distance, GeoUnit.Kilometers, order: Order.Ascending).ToList();
        }
    }
}
