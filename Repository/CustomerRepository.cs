using Microsoft.EntityFrameworkCore;
using WeningerDemoProject.Data;
using WeningerDemoProject.Dtos.Customer;
using WeningerDemoProject.Helpers;
using WeningerDemoProject.Interfaces;
using WeningerDemoProject.Models;

namespace WeningerDemoProject.Repository
{

    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;
        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Customer> CreateAsync(Customer customerModel)
        {
            await _context.Customers.AddAsync(customerModel);
            await _context.SaveChangesAsync();
            return customerModel;
        }

        public async Task<Customer?> DeleteAsync(int id)
        {
            var customerModel = await _context.Customers.FirstOrDefaultAsync(x => x.Id == id);
            
            if (customerModel == null)
                return null;
            
            _context.Customers.Remove(customerModel);
            await _context.SaveChangesAsync();
            
            return customerModel;
        }

        public async Task<Customer?> UpdateAsync(int id, UpdateCustomerDto customerDto)
        {
            var customerInDb = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);

            if (customerInDb == null)
                return null;

            customerInDb.CustomerNumber = customerDto.CustomerNumber;
            customerInDb.FirstName = customerDto.FirstName;
            customerInDb.SecondName = customerDto.SecondName;

            await _context.SaveChangesAsync();

            return customerInDb;
        }

        public async Task<List<Customer>> GetAllAsync(QueryObject query)
        {
            var customers = _context.Customers.Include(c => c.Orders).AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.CustomerNumber?.ToString()))
                customers = customers.Where(c => c.CustomerNumber.ToString().ToLower().Contains(query.CustomerNumber.ToString().ToLower()));

            if (!string.IsNullOrWhiteSpace(query.FirstName))
                customers = customers.Where(c => c.FirstName.ToLower().Contains(query.FirstName.ToLower()));

            if (!string.IsNullOrWhiteSpace(query.SecondName))
                customers = customers.Where(c => EF.Functions.Like(c.SecondName, $"%{query.SecondName}%"));

            if (!string.IsNullOrWhiteSpace(query.SecondName))
                customers = customers.Where(c => c.SecondName.ToLower().Contains(query.SecondName.ToLower()));


            if (!string.IsNullOrWhiteSpace(query.SortBy))
            {
                if (query.SortBy.Equals("Name", StringComparison.OrdinalIgnoreCase))
                {
                    customers = query.IsDescending ? customers.OrderByDescending(c => c.SecondName) : 
                        customers.OrderBy(c => c.SecondName);
                }
            }

            var skipNumber = (query.PageNumber - 1) * query.PageSize;

            return await customers.Skip(skipNumber).Take(query.PageSize).ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            var customerModel = await _context.Customers.Include(c => c.Orders).FirstOrDefaultAsync(c => c.Id == id);

            if (customerModel == null)
                return null;

            return customerModel;
        }

        public async Task<Customer?> GetByNumberAsync(int customerNumber)
        {
            var customerModel = await _context.Customers.Include(c => c.Orders).FirstOrDefaultAsync(c => c.CustomerNumber == customerNumber);

            if (customerModel == null)
                return null;

            return customerModel;
        }

        public async Task<bool> CustomerExists(int customerNumber)
        {
            return await _context.Customers.AnyAsync(c => c.CustomerNumber == customerNumber);
        }

        public async Task<int?> GetCustomerIdByNumberAsync(int customerNumber)
        {
            return await _context.Customers.Where(c => c.CustomerNumber == customerNumber).
                Select(c => (int?)c.Id).FirstOrDefaultAsync();
        }
    }
}
