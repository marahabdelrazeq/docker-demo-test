namespace CommonRepo.Infrastructure.Messaging.Models
{
    public class RedisTopics
    {
        public string AddTopic { get; set; } = "RedisSync";
        public string UpdateTopic { get; set; } = "RedisSync";
        public string DeleteTopic { get; set; } = "RedisSync";
    }
}