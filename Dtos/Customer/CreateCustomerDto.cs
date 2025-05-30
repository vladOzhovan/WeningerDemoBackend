using System.ComponentModel.DataAnnotations;

namespace WeningerDemoProject.Dtos.Customer
{
    public class CreateCustomerDto
    {
        [Required(ErrorMessage = "Customer number is required.")]
        [Range(10000, 99999, ErrorMessage = "Customer number must be a 5-digit number.")]
        public int CustomerNumber { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "First name must be between 2 and 50 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Second name is reauired.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Second name must be between 2 and 50 characters.")]
        public string SecondName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } = string.Empty;
        
        [RegularExpression(@"^\+?[0-9\s\-\(\)]{7,25}$", ErrorMessage = "Invalid phone number format.")]
        public string PhoneNumber { get; set; } = string.Empty;

        public AddressDto Address { get; set; } = new AddressDto();
    }
}
