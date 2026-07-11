using Microsoft.EntityFrameworkCore;

namespace CommonRepo.Domain.Entities.Lookups;

[Keyless]
public class LTPortsOfCountry
{
    public string iso2 { get; set; }

    public string Location { get; set; }

    public string Name2 { get; set; }
}
