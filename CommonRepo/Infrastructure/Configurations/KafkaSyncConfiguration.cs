using Confluent.Kafka;
using CommonRepo.Infrastructure.Messaging.Models;

namespace CommonRepo.Infrastructure.Configurations
{
    public class KafkaSyncConfiguration
    {
        public bool Enable { get; set; } = true;
        public ProducerConfig Config { get; set; } = new ProducerConfig { BootstrapServers = "192.168.6.150:9092" };
        public RedisTopics Topics { get; set; } = new RedisTopics();
    }
}
