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
                CreatedOn = customer.CreatedOn,
                OverallStatus = customer.OverallStatus.ToString(),
                Orders = customer.Orders.Select(o => o.ToOrderDto()).ToList()
            };
        }

        public static Customer ToCustomerFromCreateDto(this CreateCustomerDto customerDto)
        {
            return new Customer
            {
                CustomerNumber = customerDto.CustomerNumber,
                FirstName = customerDto.FirstName,
                SecondName = customerDto.SecondName,
                CreatedOn = customerDto.CreatedOn
            };
        }
    }
}
