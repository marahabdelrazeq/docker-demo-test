using System;
using System.Collections.Generic;

namespace CommonRepo.Domain.Entities.Lookups;

public partial class LtcaptionsUi
{
    public long Id { get; set; }

    public string Code { get; set; }

    public string LanguageCode { get; set; }

    public bool AllowInMobile { get; set; }

    public string Value { get; set; }
}
