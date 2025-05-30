using WeningerDemoProject.Dtos.Customer;
using WeningerDemoProject.Models;

namespace WeningerDemoProject.Mappers
{
    public static class CustomerMapper
    {
        public static CustomerDto ToCustomerDto(this Customer customer)
        {
            return new CustomerDto
            {
                Id = customer.Id,
                CustomerNumber = customer.CustomerNumber,
                FirstName = customer.FirstName,
                SecondName = customer.SecondName,
                Email = customer.Email,
                PhoneNumber = customer.PhoneNumber,
                Address = new AddressDto
                {
                    ZipCode = customer.Address.ZipCode,
                    Country = customer.Address.Country ?? string.Empty,
                    City = customer.Address.City ?? string.Empty,
                    Street = customer.Address.Street ?? string.Empty,
                    HouseNumber = customer.Address.HouseNumber ?? string.Empty,
                    Apartment = customer.Address.Apartment ?? string.Empty,
                },
                CreatedOn = customer.CreatedOn,
                OverallStatus = customer.OverallStatus.ToString(),
                Orders = customer.Orders.Select(o => o.ToOrderDto()).ToList()
            };
        }

        public static Customer ToCustomerFromCreateDto(this CreateCustomerDto dto)
        {
            return new Customer
            {
                CustomerNumber = dto.CustomerNumber,
                FirstName = dto.FirstName,
                SecondName = dto.SecondName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Address = new Address
                {
                    ZipCode = dto.Address.ZipCode,
                    Country = dto.Address.Country,
                    City = dto.Address.City,
                    Street = dto.Address.Street,
                    HouseNumber = dto.Address.HouseNumber,
                    Apartment = dto.Address.Apartment,
                },
                CreatedOn = DateTime.UtcNow
            };
        }
    }
}
