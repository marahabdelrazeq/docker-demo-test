using StackExchange.Redis;
#pragma warning disable

namespace CommonLibrary.Caching.Redis
{
    public class RedisConnectorHelper
    {
        private readonly ConfigurationOptions _configuration = null;
        private readonly Lazy<ConnectionMultiplexer> _lazyConnection;

        /// <summary>
        /// Initializes a new instance of the RedisConnectorHelper class.
        /// </summary>
        /// <param name="connectionString">The Redis connection string (IP and port).</param>
        /// <param name="cacheType">The type of cache, defaults to Redis.</param>
        public RedisConnectorHelper(string connectionString, string cacheType = "Redis")
        {
            var ip = connectionString.Split(":");

            if (cacheType.ToUpper() == "REDIS")
            {
                _configuration = new ConfigurationOptions
                {
                    EndPoints = { { ip[0], int.Parse(ip[1]) } },
                    AllowAdmin = true,
                    ClientName = "MyRedisClient",
                    ReconnectRetryPolicy = new LinearRetry(5000), // Retry every 5 seconds
                    AbortOnConnectFail = false, // Don't abort on connection failure
                };

                // Initialize the lazy connection
                _lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
                {
                    return ConnectionMultiplexer.Connect(_configuration);
                });
            }
            else
            {
                throw new ArgumentException($"Unsupported cache type: {cacheType}");
            }
        }

        /// <summary>
        /// Gets the active Redis connection.
        /// </summary>
        public ConnectionMultiplexer Connection => _lazyConnection.Value;
    }
}
