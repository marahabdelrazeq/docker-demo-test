using System.Globalization;
using CommonRepo.Domain.Entities.Waybills;
using docker_demo.DTOs.WaybillDto;

namespace docker_demo.Services.Implementations
{
    public static class WaybillViewMapper
    {
        private static string? D(DateTime? value) => value?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        private static string? T(TimeSpan? value) => value is null ? null : value.Value.ToString(@"hh\:mm", CultureInfo.InvariantCulture);

        public static WaybillPdfRequest ToWaybillPdfRequest(this EWaybillsView v)
        {
            return new WaybillPdfRequest
            {
                Meta = new MetaInfo
                {
                    PrintDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                    Via = v.CreatedByNameLocalized ?? v.CreatedByNameForeign,
                    CreatedDate = D(v.CreatedDate),
                },
                Document = new DocumentInfo
                {
                    CompanyName = v.ClearingCompanyNameLocalized ?? v.ClearingCompanyName,
                    DocumentNumber = v.DocumentNumber,
                    InvoiceNumber = v.EWaybillNo,
                    CargoType = v.CargoTypeNameLocalized ?? v.CargoTypeNameForeign,
                },
                Goods = new GoodsInfo
                {
                    FirstContainerNumber = v.FirstContainerNumber,
                    SecondContainerNumber = v.SeconedContainerNumber,
                    GoodsValue = v.ValueOfGoods?.ToString(CultureInfo.InvariantCulture),
                    GoodsDescription = v.CargoDescription,
                },
                Loading = new LoadingInfo
                {
                    Country = v.LoadingCountryNameLocalized,
                    Governorate = v.LoadingGovernorateNameLocalized,
                    Source = v.LoadingSourceNameLocalized,
                    Location = v.LoadingFromLocalized,
                    Date = D(v.LoadingDate),
                    Time = T(v.LoadingTime),
                    Address = v.LoadingAddress,
                },
                Unloading = new UnloadingInfo
                {
                    Country = v.DischargingCountryNameLocalized,
                    Governorate = v.DischargingGovernorateNameLocalized,
                    City = v.DischargingCityNameLocalized,
                    Location = v.DischargingFromLocalized,
                    Date = D(v.DischargingDate),
                    Time = T(v.DischargingTime),
                    Address = v.DischargingAddress,
                },
                Tractor = new TractorInfo
                {
                    CarrierCompany = v.TransporterNameLocalized,
                    PlateNumber = v.TruckPlateNumber,
                    PlateCode = v.TruckPlateCode,
                    UsageType = v.TruckUsageTypeLocalized,
                },
                Trailer = new TrailerInfo
                {
                    PlateNumber = v.TrailerPlateNumber,
                    PlateCode = v.TrailerPlateCode,
                    UsageType = v.TrailerUsageTypeLocalized,
                },
                CarrierInsurance = new InsuranceInfo
                {
                    InsuranceType = v.InsuranceTypeTrucking,
                    InsuranceCompany = v.InsuranceCompanyTrucking,
                    PolicyNumber = v.InsuranceContractNoTrucking,
                    AmountOmr = v.InsuranceAmountTrucking?.ToString(CultureInfo.InvariantCulture),
                    ExpiryDate = D(v.InsuranceExpiryDateTrucking),
                },
                ClearanceInsurance = new InsuranceInfo
                {
                    InsuranceType = v.InsuranceTypeClearing,
                    InsuranceCompany = v.InsuranceCompanyClearing,
                    PolicyNumber = v.InsuranceContractNoClearing,
                    AmountOmr = v.InsuranceAmountClearing?.ToString(CultureInfo.InvariantCulture),
                    ExpiryDate = D(v.InsuranceExpiryDateClearing),
                },
                Driver = new DriverInfo
                {
                    LicenseNumber = null,
                    Name = v.DriverName,
                    Nationality = v.DriverNationality,
                    Phone = string.IsNullOrEmpty(v.DriverMobileNumber) ? null : $"{v.DriverAreaCode}{v.DriverMobileNumber}",
                },
                Wages = new WagesInfo
                {
                    PriceOmr = v.Price?.ToString(CultureInfo.InvariantCulture),
                    PaymentMethod = v.PaymentMethod,
                    AdvancePaymentOmr = v.DownPayment?.ToString(CultureInfo.InvariantCulture),
                },
                Delay = new DelayInfo
                {
                    CarrierDelayAmount = v.DemurrageTrucking?.ToString(CultureInfo.InvariantCulture),
                    ClearanceDelayAmount = v.DemurrageClearing?.ToString(CultureInfo.InvariantCulture),
                },
            };
        }
    }
}
