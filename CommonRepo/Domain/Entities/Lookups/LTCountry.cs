namespace CommonRepo.Domain.Entities.Lookups;
public class LTCountry
{
    public long Id { get; set; }

    public string Code { get; set; }
    public string iso2 { get; set; }

    public string NumericCode { get; set; }

    public string AreaCode { get; set; }

    public string Name { get; set; }

    public string CaptionCode { get; set; }

    public int SortOrder { get; set; }

    public string StatusCode { get; set; }

    public string SMSProvider { get; set; }

    public bool IsGcc { get; set; }

    public string TimeZone { get; set; }
}