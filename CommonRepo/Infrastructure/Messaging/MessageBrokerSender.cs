using Newtonsoft.Json;
using CommonRepo.Infrastructure.Messaging.Models;

namespace CommonRepo.Infrastructure.Messaging
{
    public class MessageBrokerSender
    {
        public MessageBrokerDTO messageBrokerDTO { get; set; } = new MessageBrokerDTO();

        public MessageBrokerSender(dynamic entity, MessageOperation operation)
        {
            messageBrokerDTO.Entity = JsonConvert.SerializeObject(entity, new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                TypeNameHandling = TypeNameHandling.Auto,
                Error = HandleJsonDeserializationError
            });
            Type entityType = entity.GetType();
            messageBrokerDTO.EntityMetadata = new ClassMetadata
            {
                ClassName = entityType.Name,
                Properties = entityType.GetProperties().ToDictionary(p => p.Name, p => p.PropertyType),
                PropertyAttributes = entityType.GetProperties()
                                    .ToDictionary(
                                        p => p.Name,
                                        p => p.GetCustomAttributes(false).Cast<Attribute>().ToArray()
                                    ),
                ClassAttributes = entityType.GetCustomAttributes(true).OfType<Attribute>().Where(x => x.GetType().Name == "DocumentAttribute").ToArray()
            };
            messageBrokerDTO.Operation = operation;
        }
        public string Serialize()
        {
            return JsonConvert.SerializeObject(messageBrokerDTO, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
                Error = HandleJsonDeserializationError
            });
        }
        private void HandleJsonDeserializationError(object sender, Newtonsoft.Json.Serialization.ErrorEventArgs args)
        {
            Console.WriteLine(args.ErrorContext.Error.Message);
            args.ErrorContext.Handled = true;
        }
    }
}
