using System.ComponentModel.DataAnnotations;

namespace WeningerDemoProject.Dtos.Customer
{
    public class AddressDto
    {
        public int ZipCode { get; set; }
        public string HouseNumber { get; set; } = string.Empty;   
        public string Apartment { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}
