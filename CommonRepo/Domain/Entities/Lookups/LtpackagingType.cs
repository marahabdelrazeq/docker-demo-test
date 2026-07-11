namespace CommonRepo.Domain.Entities.Lookups;

public class LtpackagingType
{
    public int Id { get; set; }

    public int CargoTypeId { get; set; }

    public string Code { get; set; }

    public string NameLocalized { get; set; }

    public string NameForeign { get; set; }

}
