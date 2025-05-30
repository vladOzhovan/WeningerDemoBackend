using WeningerDemoProject.Dtos.Order;

namespace WeningerDemoProject.Dtos.Customer
{
    public class CustomerDto
    {
        public int Id { get; set; }
        public int CustomerNumber { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string SecondName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public AddressDto Address { get; set; } = new AddressDto();
        public DateTime CreatedOn { get; set; }
        public string OverallStatus { get; set; } = string.Empty;
        public List<OrderDto> Orders { get; set; } = [];
    }
}
