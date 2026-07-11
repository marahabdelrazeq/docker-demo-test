using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonRepo.Domain.Entities.Lookups
{
    public class Cities
    {
        public int Id { get; set; }
        public int GovernorateId { get; set; }
        public string NameAr { get; set; } = null!;
        public string NameEn { get; set; } = null!;

        //public Governorates Governorate { get; set; } = null!;

    }
}
