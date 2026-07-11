using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CommonRepo.Domain.Entities
{
    public class SystemConfiguration
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string DomainUrl { get; set; }

        public string CountryCode { get; set; }

        public string AttachmentUrl { get; set; }

        public string TimeZone { get; set; }

    }
}
