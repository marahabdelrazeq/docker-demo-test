using CommonLibrary.Caching.Redis;

namespace CommonLibrary.Caching.Factory
{
    public class CacheProviderFactory<T> where T : class
    {
        public static RedisCacheProvider<T> CreateRedisCacheProvider(string connectionString)
        {
            return new RedisCacheProvider<T>(connectionString);
        }

        public static RedisCacheEngine CreateRedisCacheEngine(string connectionString)
        {
            return new RedisCacheEngine(connectionString);
        }
    }
}
