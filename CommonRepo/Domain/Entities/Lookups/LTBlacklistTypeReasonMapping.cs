using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonRepo.Domain.Entities.Lookups
{
    public class LTBlacklistTypeReasonMapping
    {
        public int Id { get; set; }

        public string BlacklistTypeCode { get; set; }


        public string BlacklistReasonCode { get; set; }

        public string BlacklistReasonLocalized { get; set; }

        public string BlacklistReasonForeign { get; set; }

    }
}
