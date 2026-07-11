using System;
using System.Collections.Generic;

namespace CommonRepo.Domain.Entities.Subscriptions;

public partial class Subscription
{
    public int Id { get; set; }

    public int? subscription_id { get; set; }

    public string? plate_number { get; set; }

    public string? plate_code_ar { get; set; }

    public string? plate_code_en { get; set; }
     
}
