using Newtonsoft.Json;

namespace CommonRepo.Infrastructure.Messaging.Models
{
    public class MessageBrokerDTO
    {
        [JsonProperty]
        public string Entity { get; set; }
        [JsonProperty]
        public ClassMetadata EntityMetadata { get; set; }
        [JsonProperty]
        public MessageOperation Operation { get; set; }
    }
}
