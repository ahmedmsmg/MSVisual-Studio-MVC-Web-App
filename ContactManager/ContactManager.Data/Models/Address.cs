using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContactManager.Data.Models
{
    public class Address
    {
        [Key]
        public int AddressId { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string PostalCode { get; set; }

        public int ContactId { get; set; } // Foreign key

        [ForeignKey(nameof(ContactId))]
        public Contact Contact { get; set; }
    }
}
