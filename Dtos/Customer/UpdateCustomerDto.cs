using System.ComponentModel.DataAnnotations;
using WeningerDemoProject.Models;
using WeningerDemoProject.Validators;

namespace WeningerDemoProject.Dtos.Customer
{
    public class UpdateCustomerDto
    {
        [Range(10000, 99999, ErrorMessage = "Customer number must be a 5-digit number.")]
        public int CustomerNumber { get; set; }

        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(50, MinimumLength = 2, ErrorMessage = "Second name must be between 2 and 50 characters.")]
        public string SecondName { get; set; } = string.Empty;

        [EmailAddressOrEmpty]
        public string Email { get; set; } = string.Empty;

        [RegularExpression(@"^\+?[0-9\s\-\(\)]{7,25}$", ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; } = string.Empty;

        public AddressDto Address { get; set; } = new AddressDto();
    }
}
