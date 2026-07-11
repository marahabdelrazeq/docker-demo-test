using Confluent.Kafka;

namespace CommonRepo.Infrastructure.Configurations
{
    public class KafkaSyncConsumerConfiguration
    {
        private KafkaHandlerSettings? _default = new KafkaHandlerSettings()
        {
            Setting = new ConsumerConfig()
        };

        public KafkaHandlerSettings? Default
        {
            get => _default;

            set => _default = value;
        }
    }

    public class KafkaHandlerSettings
    {
        public string? Topic { get; set; }

        public ConsumerConfig? Setting { get; set; }
    }

}

