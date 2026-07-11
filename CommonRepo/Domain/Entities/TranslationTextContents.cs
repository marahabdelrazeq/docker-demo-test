using Redis.OM.Modeling;

namespace CommonRepos.Domain.Entities;

[Document(StorageType = StorageType.Hash, Prefixes = new[] { "TranslationTextContents" }, IndexName = "TranslationTextContents")]
public partial class TranslationTextContents
{
    [RedisIdField]
    [Indexed]
    public int Id { get; set; }

    [Indexed]
    public string LanguageCode { get; set; }

    [Indexed]
    public string Property { get; set; }

    [Indexed]
    public string OriginalValue { get; set; }

    [Indexed]
    public string Value { get; set; }

    [Indexed]
    public string EntityName { get; set; }
    
    [Indexed]
    public int? RowId { get; set; }
}
