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
        private readonly ILogger<CustomerRepository> _logger;
        private readonly Random _random;
        public CustomerRepository(AppDbContext context, ILogger<CustomerRepository> logger, Random random)
        {
            _context = context;
            _logger = logger;
            _random = random;
        }

        public async Task<List<Customer>> GetAllAsync(QueryObject query)
        {
            var customers = _context.Customers.Include(c => c.Orders).Include(c => c.Address).AsQueryable();
            customers = customers.ApplySearch(query.Search);
            customers = customers.ApplySorting(query.IsDescending, query.SortBy);
            return await customers.ToListAsync();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            var customerModel = await _context.Customers.Include(c => c.Orders).Include(c => c.Address).
                FirstOrDefaultAsync(c => c.Id == id);

            if (customerModel == null)
                return null;

            return customerModel;
        }

        public async Task<Customer?> GetByNumberAsync(int customerNumber)
        {
            var customerModel = await _context.Customers.Include(c => c.Orders).Include(c => c.Address).
                FirstOrDefaultAsync(c => c.CustomerNumber == customerNumber);

            if (customerModel == null)
                return null;

            return customerModel;
        }

        public async Task<int?> GetCustomerIdByNumberAsync(int customerNumber)
        {
            return await _context.Customers.Where(c => c.CustomerNumber == customerNumber).
                Select(c => (int?)c.Id).FirstOrDefaultAsync();
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

        public async Task<Customer?> UpdateAsync(int id, UpdateCustomerDto dto)
        {
            var customerInDb = await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);

            if (customerInDb == null)
                return null;

            customerInDb.CustomerNumber = dto.CustomerNumber;
            customerInDb.FirstName = dto.FirstName;
            customerInDb.SecondName = dto.SecondName;
            customerInDb.Email = dto.Email;
            customerInDb.PhoneNumber = dto.PhoneNumber;
            customerInDb.Address = new Address
            {
                ZipCode = dto.Adress.ZipCode,
                Country = dto.Adress.Country ?? string.Empty,
                City = dto.Adress.City ?? string.Empty,
                Street = dto.Adress.Street ?? string.Empty,
                HouseNumber = dto.Adress.HouseNumber ?? string.Empty,
                Apartment = dto.Adress.Apartment ?? string.Empty
            };

            await _context.SaveChangesAsync();

            return customerInDb;
        }

        public async Task<bool> CustomerExists(int customerNumber)
        {
            return await _context.Customers.AnyAsync(c => c.CustomerNumber == customerNumber);
        }

        public async Task<List<Customer>> GenerateCustomerListAsync(int count)
        {
            
            var customerList = new List<Customer>();

            var firstNames = new[] { "Alfred", "Alisa", "Arnold", "Amina", "Arno", "Alex" , "Ben", "Bella", "Brayan", 
                "Briana", "Bred", "Candy", "Chriss", "Diana", "Den", "Illiana", "Fred", "Frederik", "Holly", 
                "Jekki", "John", "Julia", "Kianu", "Kriss", "Kayla", "Liam", "Katarina", "Layla", "Liam", 
                "Michael", "Neo", "Rayan", "Rummi", "Sara", "Sabrina", "Thomas", "Uliana" };

            var secondNames = new[] { "Abend", "Affleck", "Agnes", "Bail", "Born", "Beggins", "Backer", "Billa", 
                "Billigan", "Berry", "Chan", "Hichkock", "Hocking", "Johnson", "Jordon",  "Hichkock", "Hocking", 
                "Kennedy", "Konnor", "Linkoln", "Malek", "Nison", "Nickolson", "Pitt", "Pratt", "Riwz", "Schwarzenegger", 
                "Surr", "Sina", "Villian", "Woker", "Xerks" };

            int maxCustomerNumber = await _context.Customers.AnyAsync() 
                ? await _context.Customers.MaxAsync(c => c.CustomerNumber)
                : 10001;

            int firstNumberOfCustomerInList = 10001;

            if (maxCustomerNumber != 0)
                firstNumberOfCustomerInList = maxCustomerNumber + 1;

            for (int i = 0; i < count; i++)
            {

                customerList.Add(new Customer
                {
                    CustomerNumber = firstNumberOfCustomerInList + i,
                    FirstName = firstNames[_random.Next(firstNames.Length)],
                    SecondName = secondNames[_random.Next(secondNames.Length)]
                });
            }

            await _context.Customers.AddRangeAsync(customerList);
            await _context.SaveChangesAsync();
            
            _logger.LogInformation("{Count} test customers generated starting from #{StartNumber}",
                count, firstNumberOfCustomerInList);
            
            return customerList;
        }
    }
}
