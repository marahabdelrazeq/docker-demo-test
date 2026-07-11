using StackExchange.Redis;

namespace CommonRepo.Infrastructure.Configurations
{
    public class RedisConfiguration
    {
        private ConfigurationOptions _config = new ConfigurationOptions();

        public List<string> EndPoints { get; set; } = new();
        public ConfigurationOptions Config
        {
            get
            {
                foreach (var endpoint in EndPoints)
                {
                    if (!_config.EndPoints.Any(x => x.ToString().Contains(endpoint)))
                    {
                        _config.EndPoints.Add(endpoint);
                    }
                }

                return _config;
            }
            set => _config = value;
        }
    }
}