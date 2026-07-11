using Confluent.Kafka;
using CommonRepo.Infrastructure.Messaging.Models;

namespace CommonRepo.Infrastructure.Configurations
{
    public class KafkaSyncProducerConfiguration
    {
        public bool Enable { get; set; } = false;
        public ProducerConfig Config { get; set; } = new ProducerConfig { BootstrapServers = "localhost:29092" };
        public RedisTopics Topics { get; set; } = new RedisTopics();
    }
}
