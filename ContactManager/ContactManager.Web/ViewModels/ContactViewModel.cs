using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ContactManager.Web.ViewModels
{
    public class ContactViewModel
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public List<AddressViewModel> Addresses { get; set; } = new List<AddressViewModel>();
    }

    public class AddressViewModel
    {
        public int Id { get; set; } // This property will help in identifying existing addresses

        [Required]
        public string Street { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string PostalCode { get; set; }

        // Property to signal if an address should be deleted
        public bool Delete { get; set; }
    }
}
