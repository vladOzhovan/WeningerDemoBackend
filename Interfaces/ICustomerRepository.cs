using WeningerDemoProject.Dtos.Customer;
using WeningerDemoProject.Helpers;
using WeningerDemoProject.Models;

namespace WeningerDemoProject.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer> CreateAsync(Customer customerModel);
        Task<Customer?> UpdateAsync(int id, UpdateCustomerDto customerDto);
        Task<Customer?> DeleteAsync(int id);
        Task<List<Customer>> GetAllAsync(QueryObject query);
        Task<Customer?> GetByIdAsync(int id);
        Task<Customer?> GetByNumberAsync(int customerNumber);
        Task<bool> CustomerExists(int id);
        Task<int?> GetCustomerIdByNumberAsync(int customerNumber);
    }
}
