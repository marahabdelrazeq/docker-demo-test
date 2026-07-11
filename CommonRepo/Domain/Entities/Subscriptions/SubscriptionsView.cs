using System;
using System.Collections.Generic;

namespace docker_demo.Models;

public  class SubscriptionsView
{
    public long? Id { get; set; }

    public int? SubscriptionId { get; set; }

    public int? PermitId { get; set; }

    public string? PlateNumber { get; set; }

    public string? PlateCodeAr { get; set; }

    public string? PlateCodeEn { get; set; }

    public string? IdxPlateNumberCodeAr { get; set; }

    public string? IdxPlateNumberCodeEn { get; set; }

    public string? PermitNumber { get; set; }

    public string? PermitStatusCode { get; set; }

    public string? PermitStatusCodeAr { get; set; }

    public string? PermitStatusCodeEn { get; set; }

    public string? TrailerType { get; set; }

    public string? TrailerTypeAr { get; set; }

    public string? TrailerTypeEn { get; set; }

    public string? DriverName { get; set; }

    public string? DriverNationality { get; set; }

    public string? DriverNationalityAr { get; set; }

    public string? DriverNationalityEn { get; set; }

    public string? DriverDocumentNationality { get; set; }

    public string? DriverDocumentNationalityAr { get; set; }

    public string? DriverDocumentNationalityEn { get; set; }

    public string? DriverMobileNumber { get; set; }

    public DateTime CreationDate { get; set; }

    public DateTime? ServiceExpiryDate { get; set; }

    public string? SubscriptionCreationDateGrid { get; set; }

    public string? ServiceExpiryDateGrid { get; set; }

    public DateTime? SubscriptionCreationDate { get; set; }

    public string? PermitCreationDateGrid { get; set; }

    public DateTime? PermitCreationDate { get; set; }

    public int? DurationId { get; set; }

    public string? DurationAr { get; set; }

    public string? FleetEntityNameThumbnail { get; set; }

    public string? DurationEn { get; set; }

    public int? LocationId { get; set; }

    public string? FleetEntityName { get; set; }

    public string? FleetNationality { get; set; }

    public int? PackageId { get; set; }

    public string? InvestorNameAr { get; set; }

    public string? InvestorNameEn { get; set; }

    public int? FreezoneGinWeighAmount { get; set; }

    public bool? FreezoneIsDirect { get; set; }

    public string? LocationName { get; set; }
}
