namespace CommonRepo.Domain.Entities.Lookups;

public partial class CargoTypeAndPackagingType
{
    public int Id { get; set; }

    public string CargoTypeCode { get; set; }

    public string CargoTypeNameLocalized { get; set; }

    public string CargoTypeNameForeign { get; set; }

    public int PackagingTypeId { get; set; }

    public string PackagingTypeCode { get; set; }

    public string PackagingTypeNameLocalized { get; set; }

    public string PackagingTypeNameForeign { get; set; }
}
