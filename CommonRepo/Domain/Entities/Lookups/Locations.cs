namespace CommonRepo.Domain.Entities.Lookups;

public class Locations
{
    public int Id { get; set; }

    public int? ParentId { get; set; }

    public string Code { get; set; }

    public string StatusCode { get; set; }

    public string Type { get; set; }

    public string LocationNameForeign { get; set; }

    public string LocationNameLocalized { get; set; }
}
