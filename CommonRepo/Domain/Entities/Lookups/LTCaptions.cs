using Redis.OM.Modeling;

namespace CommonRepo.Domain.Entities.Lookups;
[Document(StorageType = StorageType.Hash, Prefixes = new[] { "LtCaptions" }, IndexName = "LtCaptions")]
public class LTCaptions
{
    [RedisIdField]
    [Indexed]
    public int Id { get; set; }

    [Indexed]
    public string Code { get; set; }
    
    [Indexed]
    public string EntityName { get; set; }

    [Indexed]
    public string LanguageCode { get; set; }
    [Indexed]
    public string Value { get; set; }
    [Indexed]
    public bool AllowInMobile { get; set; }
}
