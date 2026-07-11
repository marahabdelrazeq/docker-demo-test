namespace CommonRepo.Infrastructure.Messaging.Models
{
    [Serializable]
    public class ClassMetadata
    {
        public string ClassName { get; set; }
        public Dictionary<string, Type> Properties { get; set; }
        public Dictionary<string, Attribute[]> PropertyAttributes { get; set; }
        public Attribute[] ClassAttributes { get; set; }
    }
}