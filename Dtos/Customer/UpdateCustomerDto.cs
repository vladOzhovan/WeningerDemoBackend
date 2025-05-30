using WeningerDemoProject.Models;

namespace WeningerDemoProject.Dtos.Customer
{
    public class UpdateCustomerDto
    {
        public int CustomerNumber { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public Address Adress { get; set; } = new Address();
    }
}
