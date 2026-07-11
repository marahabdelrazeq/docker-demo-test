using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonRepo.Domain.Entities.Lookups
{
    public class LTServicePricing
    {
        public int Id { get; set; }

        public int LocationId { get; set; }

        public string ServiceCode { get; set; }
        public string ServiceType { get; set; }

        public string TruckType { get; set; }

        public double TaxPercentage { get; set; }

        public double AdditionalFees { get; set; }

        public double Price { get; set; }

        public bool PriceIncludingTax { get; set; }

        public string Currency { get; set; }
        public string StatusCode { get; set; }
    }
}
