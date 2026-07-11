using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonRepo.Domain.Entities.Lookups
{
    public class LTBanks
    {
        public int id   { get; set; }
        public string Code { get; set; }
        public string NameLocalized { get; set; }
        public string NameForeign { get; set; }
        public string StatusCode { get; set; }
    }
}
