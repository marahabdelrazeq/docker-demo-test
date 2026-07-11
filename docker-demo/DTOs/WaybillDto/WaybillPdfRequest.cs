namespace docker_demo.DTOs.WaybillDto
{
    public class WaybillPdfRequest
    {
        public MetaInfo Meta { get; set; } = new();
        public DocumentInfo Document { get; set; } = new();
        public GoodsInfo Goods { get; set; } = new();
        public LoadingInfo Loading { get; set; } = new();
        public UnloadingInfo Unloading { get; set; } = new();
        public TractorInfo Tractor { get; set; } = new();
        public TrailerInfo Trailer { get; set; } = new();
        public InsuranceInfo CarrierInsurance { get; set; } = new();
        public InsuranceInfo ClearanceInsurance { get; set; } = new();
        public DriverInfo Driver { get; set; } = new();
        public WagesInfo Wages { get; set; } = new();
        public DelayInfo Delay { get; set; } = new();
    }

    public class MetaInfo
    {
        public string? PrintDate { get; set; }
        public string? Via { get; set; }
        public string? CreatedDate { get; set; }
    }

    public class DocumentInfo
    {
        public string? CompanyName { get; set; }
        public string? DocumentNumber { get; set; }
        public string? InvoiceNumber { get; set; }
        public string? CargoType { get; set; }
    }

    public class GoodsInfo
    {
        public string? FirstContainerNumber { get; set; }
        public string? SecondContainerNumber { get; set; }
        public string? GoodsValue { get; set; }
        public string? GoodsDescription { get; set; }
    }

    public class LoadingInfo
    {
        public string? Country { get; set; }
        public string? Governorate { get; set; }
        public string? Source { get; set; }
        public string? Location { get; set; }
        public string? Date { get; set; }
        public string? Time { get; set; }
        public string? Address { get; set; }
    }

    public class UnloadingInfo
    {
        public string? Country { get; set; }
        public string? Governorate { get; set; }
        public string? City { get; set; }
        public string? Location { get; set; }
        public string? Date { get; set; }
        public string? Time { get; set; }
        public string? Address { get; set; }
    }

    public class TractorInfo
    {
        public string? CarrierCompany { get; set; }
        public string? PlateNumber { get; set; }
        public string? PlateCode { get; set; }
        public string? UsageType { get; set; }
    }

    public class TrailerInfo
    {
        public string? PlateNumber { get; set; }
        public string? PlateCode { get; set; }
        public string? UsageType { get; set; }
    }

    public class InsuranceInfo
    {
        public string? InsuranceType { get; set; }
        public string? InsuranceCompany { get; set; }
        public string? PolicyNumber { get; set; }
        public string? AmountOmr { get; set; }
        public string? ExpiryDate { get; set; }
    }

    public class DriverInfo
    {
        public string? LicenseNumber { get; set; }
        public string? Name { get; set; }
        public string? Nationality { get; set; }
        public string? Phone { get; set; }
    }

    public class WagesInfo
    {
        public string? PriceOmr { get; set; }
        public string? PaymentMethod { get; set; }
        public string? AdvancePaymentOmr { get; set; }
    }

    public class DelayInfo
    {
        public string? CarrierDelayAmount { get; set; }
        public string? ClearanceDelayAmount { get; set; }
    }
}
